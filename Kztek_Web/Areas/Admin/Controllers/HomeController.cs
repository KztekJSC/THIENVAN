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

namespace Kztek_Web.Areas.Admin.Controllers
{
    [Area(AreaConfig.Admin)]
    public class HomeController : Controller
    {
        private Itbl_EventService _tbl_EventService;
        private ItblPCService _tblPCService;
        private ItblLaneService _tblLaneService;
        private Itbl_Lane_PCService _tbl_Lane_PCService;
        public HomeController(Itbl_EventService _tbl_EventService, ItblPCService _tblPCService, ItblLaneService _tblLaneService, Itbl_Lane_PCService _tbl_Lane_PCService)
        {
            this._tbl_EventService = _tbl_EventService;
            this._tblPCService = _tblPCService;
            this._tblLaneService = _tblLaneService;
            this._tbl_Lane_PCService = _tbl_Lane_PCService;
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
        public async Task<IActionResult> Partial_TopService()
        {
            var list = await _tbl_EventService.GetTopService();

            return PartialView(list);
        }


        public async Task<IActionResult> DashboardPartial(string pcid)
        {
            var tree = new PC_Tree
            {
                Controllers = new List<tbl_Lane_Controller_Custom>(),
                LanePCs = new List<tbl_Lane_PC_Custom>(),
                objPC = new tblPC()
            };

            tree.objPC = await _tblPCService.GetById(pcid);

            tree.LanePCs = await _tblPCService.GetLanePCs(pcid);

            if(tree.LanePCs.Count > 0)
            {
                var laneids = tree.LanePCs.Select(n => n.lane_ID);

                tree.Controllers = await _tblPCService.GetLaneControllers(laneids);
            }

            return PartialView(tree);
        }
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
    }
}
