using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kztek_Web.Models;
using Kztek_Service.Admin;
using Kztek_Library.Models;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Kztek_Library.Configs;
using Kztek_Library.Helpers;
using Microsoft.AspNetCore.SignalR;
using Kztek_Web.Hubs;
using System.Data.SqlClient;
using Kztek_Model.Models;
using Newtonsoft.Json;
using System.Data;
using Kztek_Service.Admin.Database;
using Kztek_Core.Models;

namespace Kztek_Web.Areas.Admin.Controllers
{
    [Area(AreaConfig.Admin)]
    public class HomeController : Controller
    {
        private ItblPCService _tblPCService;
        private ItblLaneService _tblLaneService;
        private Itbl_Lane_PCService _tbl_Lane_PCService;
        private Itbl_Lane_LedService _tbl_Lane_LedService;
        private ItblLedService _tblLedService;
        private Itbl_Lane_ControllerService _tbl_Lane_ControllerService;
        private Itbl_ControllerService _tbl_ControllerService;
        public HomeController(ItblPCService _tblPCService, ItblLaneService _tblLaneService, Itbl_Lane_PCService _tbl_Lane_PCService, Itbl_ControllerService _tbl_ControllerService, Itbl_Lane_ControllerService _tbl_Lane_ControllerService, Itbl_Lane_LedService _tbl_Lane_LedService, ItblLedService _tblLedService)
        {
            this._tblPCService = _tblPCService;
            this._tblLaneService = _tblLaneService;
            this._tbl_Lane_PCService = _tbl_Lane_PCService;
            this._tbl_ControllerService = _tbl_ControllerService;
            this._tbl_Lane_ControllerService = _tbl_Lane_ControllerService;
            this._tbl_Lane_LedService = _tbl_Lane_LedService;
            this._tblLedService = _tblLedService;
        }

        [CheckSessionCookie(AreaConfig.Admin)]
        public async Task<IActionResult> Index(int page = 1)
        {
            //kiểm tra session xem có lưu ngôn ngữ không nếu có thì lấy không mặc định là "vi"
            string sessionValue = HttpContext.Session.GetString(SessionConfig.Kz_Language);

            if (string.IsNullOrWhiteSpace(sessionValue))
                sessionValue = HttpContext.Request.Cookies[CookieConfig.Kz_LanguageCookie];

            sessionValue = String.IsNullOrEmpty(sessionValue) ? "vi" : sessionValue;

             LanguageHelper.GetLang(sessionValue);


            return View();
        }

        #region Tree PC

        #region Danh sách pc, cây thiết bị
        /// <summary>
        /// Cây làn, bđk, led của pc
        /// </summary>
        /// <param name="pcid"></param>
        /// <returns></returns>
        public async Task<IActionResult> DashboardPartial(string pcid)
        {
            var tree = new PC_Tree
            {
                Controllers = new List<tbl_Lane_Controller_Custom>(),
                LanePCs = new List<tbl_Lane_PC_Custom>(),
                objPC = new tblPC(),
                LaneLeds = new List<tbl_Lane_Led_Custom>()
            };

            tree.objPC = await _tblPCService.GetById(pcid);

            tree.LanePCs = await _tblPCService.GetLanePCs(pcid);

            if(tree.LanePCs.Count > 0)
            {
                var laneids = tree.LanePCs.Select(n => n.lane_ID);

                tree.Controllers = await _tblPCService.GetLaneControllers(laneids);

                tree.LaneLeds = await _tblPCService.GetLaneLeds(laneids);
            }

            return PartialView(tree);
        }

        /// <summary>
        /// Danh sách PC
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Partial_PC()
        {
            var list = await _tblPCService.GetAllActive();

            //Khai báo danh sách
            var lst = new List<SelectListModel>()
            {
                new SelectListModel { ItemValue = "", ItemText = "- Chọn PC -" }
            };

            //Kiểm tra có dữ liệu chưa
            if (list.Any())
            {
                foreach (var item in list.ToList())
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    lst.Add(new SelectListModel { ItemValue = item.id, ItemText = item.pc_Name });

                }
            }

            var a = new SelectListModel_Chosen
            {
                Placeholder = "",
                IdSelectList = "ddlPC",
                Selecteds = "",
                Data = lst
            };

            return PartialView(a);
        }
        #endregion

        #region Thêm làn
        public async Task<IActionResult> Modal_AddLane()
        {
            var list = await _tblLaneService.GetAllActive();

            //Khai báo danh sách
            var lst = new List<SelectListModel>()
            {
                new SelectListModel { ItemValue = "", ItemText = "- Chọn làn -" }
            };

            //Kiểm tra có dữ liệu chưa
            if (list.Any())
            {
                foreach (var item in list.ToList())
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    lst.Add(new SelectListModel { ItemValue = item.id, ItemText = item.lane_Name });

                }
            }

            var a = new SelectListModel_Chosen
            {
                Placeholder = "",
                IdSelectList = "ddlLane",
                Selecteds = "",
                Data = lst
            };

            return PartialView(a);
        }

        public async Task<IActionResult> SaveLane(string id, string laneid)
        {
            var result = new MessageReport(false,"Có lỗi xảy ra");

            if (string.IsNullOrEmpty(laneid))
            {
                result = new MessageReport(false, "Vui lòng chọn làn!");
                return Json(result);
            }

            var objMap = await _tbl_Lane_PCService.GetByPCLane(id, laneid);

            if(objMap != null)
            {
                result = new MessageReport(false, "Thông tin đã tồn tại!");
                return Json(result);
            }

            //tạo làn pc
            var newLanePC = new tbl_Lane_PC
            {
                id = Guid.NewGuid().ToString(),
                lane_ID = laneid,
                pc_ID = id,
                lane_order = 0,
            };

            result = await _tbl_Lane_PCService.Create(newLanePC);

            return Json(result);
        }
        #endregion

        #region Thêm BĐK
        public async Task<IActionResult> Modal_AddController(string laneid)
        {
            ViewBag.LaneId = laneid;

            var list = await _tbl_ControllerService.GetAllActive();

            //Khai báo danh sách
            var lst = new List<SelectListModel>()
            {
                new SelectListModel { ItemValue = "", ItemText = "- Chọn BĐK -" }
            };

            //Kiểm tra có dữ liệu chưa
            if (list.Any())
            {
                foreach (var item in list.ToList())
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    lst.Add(new SelectListModel { ItemValue = item.id, ItemText = item.controller_Name });

                }
            }

            var a = new SelectListModel_Chosen
            {
                Placeholder = "",
                IdSelectList = "ddlBDK",
                Selecteds = "",
                Data = lst
            };

            return PartialView(a);
        }

        public async Task<IActionResult> SaveController(string laneid, string conid)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            if (string.IsNullOrEmpty(conid))
            {
                result = new MessageReport(false, "Vui lòng chọn bộ điều khiển!");
                return Json(result);
            }

            var objMap = await _tbl_Lane_ControllerService.GetByLaneController(laneid, conid);

            if (objMap != null)
            {
                result = new MessageReport(false, "Thông tin đã tồn tại!");
                return Json(result);
            }

            //tạo làn controller
            var newLaneCon = new tbl_Lane_Controller
            {
                id = Guid.NewGuid().ToString(),
                controller_ID = conid,
                lane_ID = laneid
            };

            result = await _tbl_Lane_ControllerService.Create(newLaneCon);

            return Json(result);
        }

        public async Task<IActionResult> DeleteController(string conid)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            result = await _tbl_Lane_ControllerService.DeleteById(conid);

            return Json(result);
        }
        #endregion

        #region Thêm led
        public async Task<IActionResult> Modal_AddLed(string laneid)
        {
            ViewBag.LaneId = laneid;

            var list = await _tblLedService.GetAll();

            //Khai báo danh sách
            var lst = new List<SelectListModel>()
            {
                new SelectListModel { ItemValue = "", ItemText = "- Chọn led -" }
            };

            //Kiểm tra có dữ liệu chưa
            if (list.Any())
            {
                foreach (var item in list.ToList())
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    lst.Add(new SelectListModel { ItemValue = item.ID, ItemText = item.Name });

                }
            }

            var a = new SelectListModel_Chosen
            {
                Placeholder = "",
                IdSelectList = "ddlLed",
                Selecteds = "",
                Data = lst
            };

            return PartialView(a);
            
        }

        public async Task<IActionResult> SaveLed(string laneid, string ledid)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            if (string.IsNullOrEmpty(ledid))
            {
                result = new MessageReport(false, "Vui lòng chọn led!");
                return Json(result);
            }

            var objMap = await _tbl_Lane_LedService.GetByLaneLed(laneid, ledid);

            if (objMap != null)
            {
                result = new MessageReport(false, "Thông tin đã tồn tại!");
                return Json(result);
            }


            //tạo làn led
            var newLaneLed = new tbl_Lane_Led
            {
                id = Guid.NewGuid().ToString(),
                LED_ID = ledid,
                lane_ID = laneid
            };

            result = await _tbl_Lane_LedService.Create(newLaneLed);

            return Json(result);
        }

        public async Task<IActionResult> DeleteLed(string ledid)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            result = await _tbl_Lane_LedService.DeleteById(ledid);

            return Json(result);
        }
        #endregion

        #region Cấu hình bộ điều khiển
        public async Task<IActionResult> Modal_Config(string conid,string laneid)
        {
            var objCon = await _tbl_ControllerService.GetById(conid);

            var objLaneCon = await _tbl_Lane_ControllerService.GetByLaneController(laneid, conid);

            var model = new PC_Tree
            {
                Multi1 = objCon != null 
                ? await GetMulti(objCon.Readers_Number,"MulReader", objLaneCon != null ? objLaneCon.reader_Index : "") 
                : new SelectListModel_Multi(),
                Multi2 = objCon != null 
                ? await GetMulti(objCon.Inputs_Number, "MulInput", objLaneCon != null ? objLaneCon.input_Index : "") 
                : new SelectListModel_Multi(),
                Multi3 = objCon != null 
                ? await GetMulti(objCon.Relays_Number, "MulRelay", objLaneCon != null ? objLaneCon.barrie_Index : "") 
                : new SelectListModel_Multi(),
                Data1 = conid,
                Data2 = laneid
            };

            return PartialView(model);
        }

        public async Task<IActionResult> SaveConfig(tbl_Lane_Controller model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var objLaneCon = await _tbl_Lane_ControllerService.GetByLaneController(model.lane_ID, model.controller_ID);

            if (objLaneCon == null)
            {
                result = new MessageReport(false, "Thông tin không tồn tại!");
                return Json(result);
            }

            objLaneCon.reader_Index = !string.IsNullOrEmpty(model.reader_Index) ? model.reader_Index : "";

            objLaneCon.barrie_Index = !string.IsNullOrEmpty(model.barrie_Index) ? model.barrie_Index : "";

            objLaneCon.input_Index = !string.IsNullOrEmpty(model.input_Index) ? model.input_Index : "";

            result = await _tbl_Lane_ControllerService.Update(objLaneCon);

            return Json(result);
        }
        #endregion

        #region DDL
        public async Task<SelectListModel_Multi> GetMulti(int number,string id="",string select = "")
        {
            //Khai báo danh sách
            var lst = new List<SelectListModel>();

            //Kiểm tra có dữ liệu chưa
            for (int i = 1; i <= number; i++)
            {
                //Nếu có thì duyệt tiếp để lưu vào list
                lst.Add(new SelectListModel { ItemValue = i.ToString(), ItemText = i.ToString() });

            }

            var a = new SelectListModel_Multi
            {
                Placeholder = "",
                IdSelectList = id,
                Selecteds = select,
                Data = lst               
            };

            return a;

        }
        #endregion

        #endregion
    }
}
