using Kztek_Library.Configs;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Admin;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kztek_Web.Areas.Admin.Controllers
{
    [Area(AreaConfig.Admin)]
    public class tblControllerController : Controller
    {
        private Itbl_ControllerService _tbl_ControllerService;
        public tblControllerController(Itbl_ControllerService _tbl_ControllerService)
        {
            this._tbl_ControllerService = _tbl_ControllerService;
        }

        #region DDL
        public async Task<SelectListModel_Chosen> GetControllerType_Chosen(string selecteds)  //bind ServicePackageId to dropdownlist
        {

            var model = new SelectListModel_Chosen
            {
                Data = await StaticList.LineTypes1(),
                Placeholder = await LanguageHelper.GetLanguageText("STATICLIST:DEFAULT"),
                IdSelectList = "controller_Type",
                isMultiSelect = false,
                Selecteds = selecteds
            };

            return model;
        }

        #endregion

        #region Danh sách
        [CheckSessionCookie(AreaConfig.Admin)]
        public async Task<IActionResult> Index(string key, string pc, int page = 1, string group = "", string selectedId = "")
        {

            var gridmodel = await _tbl_ControllerService.GetAllCustomPagingByFirst(key, pc, page, 20);
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("tblController", this.HttpContext);
            //ViewBag.PCs = await GetListPC_Chosen(pc, "pc");
            //ViewBag.LaneTypes = await GetLaneType_Chosen("");
            ViewBag.keyValue = key;
            ViewBag.pcValue = pc;
            ViewBag.groupValue = group;
            ViewBag.selectedIdValue = selectedId;

            return View(gridmodel);
        }


        #endregion

        #region Thêm mới

        /// <summary>
        /// Giao diện thêm mới
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <returns></returns>     
        [CheckSessionCookie(AreaConfig.Admin)]
        [HttpGet]
        public async Task<IActionResult> Create(tbl_Controller model, string c)
        {
            model = model == null ? new tbl_Controller() : model;

            ViewBag.CommunicationType = StaticList.Communication1();

            ViewBag.Read = StaticList.ReaderTypes1();

            ViewBag.LineType = await GetControllerType_Chosen(model.controller_Type.ToString());

            return await Task.FromResult(View(model));
        }
        /// <summary>
        /// Thực hiện thêm mới
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="obj">Đối tượng</param>
        /// <param name="SaveAndCountinue">Tiếp tục thêm</param>
        /// <returns></returns>    
        [CheckSessionCookie(AreaConfig.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(tbl_Controller model, bool SaveAndCountinue = false)
        {
            ViewBag.CommunicationType = StaticList.Communication1();

            ViewBag.Read = StaticList.ReaderTypes1();

            ViewBag.LineType = await GetControllerType_Chosen(model.controller_Type.ToString());

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.controller_Name))
            {
                ModelState.AddModelError("LaneName", await LanguageHelper.GetLanguageText("MESSAGE:FEE:enter_name"));
                return View(model);
            }

            var existed = await _tbl_ControllerService.GetByName(model.controller_Name);
            if (existed != null)
            {
                ModelState.AddModelError("LaneName", await LanguageHelper.GetLanguageText("MESSAGE:FEE:exists_name"));
                return View(model);
            }

            model.id = Guid.NewGuid().ToString();
            //Thực hiện thêm mới
            var result = await _tbl_ControllerService.Create(model);
            if (result.isSuccess)
            {
                //await LogHelper.WriteLog(model.id.ToString(), ActionConfig.Create, JsonConvert.SerializeObject(model), HttpContext);
                if (SaveAndCountinue)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("Create");
                }

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }
        #endregion

        #region Cập nhật

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="id">Id bản ghi</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <returns></returns>
        [CheckSessionCookie(AreaConfig.Admin)]
        [HttpGet]
        public async Task<IActionResult> Update(string id, int pageNumber = 1)
        {
            var model = await _tbl_ControllerService.GetById(id);

            ViewBag.CommunicationType = StaticList.Communication1();

            ViewBag.Read = StaticList.ReaderTypes1();

            ViewBag.LineType = await GetControllerType_Chosen(model.controller_Type.ToString());

            return View(model);
        }

        /// <summary>
        /// Thực hiện cập nhật
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="obj">Đối tượng</param>
        /// <param name="objId">Id bản ghi</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <returns></returns>
        [CheckSessionCookie(AreaConfig.Admin)]
        [HttpPost]
        public async Task<IActionResult> Update(tbl_Controller model, int pageNumber = 1)
        {
            ViewBag.CommunicationType = StaticList.Communication1();

            ViewBag.Read = StaticList.ReaderTypes1();

            ViewBag.LineType = await GetControllerType_Chosen(model.controller_Type.ToString());

            var oldObj = await _tbl_ControllerService.GetById(model.id.ToString());
            if (oldObj == null)
            {
                ViewBag.Error = await LanguageHelper.GetLanguageText("MESSAGE:RECORD:NOTEXISTS");
                return View(model);
            }

            //
            if (string.IsNullOrWhiteSpace(model.controller_Name))
            {
                ModelState.AddModelError("controller_Name", await LanguageHelper.GetLanguageText("MESSAGE:FEE:enter_name"));
                return View(oldObj);
            }

            //
            var existed = await _tbl_ControllerService.GetByName_Id(model.controller_Name, model.id.ToString());
            if (existed != null)
            {
                ModelState.AddModelError("controller_Name", await LanguageHelper.GetLanguageText("MESSAGE:FEE:exists_name"));
                return View(oldObj);
            }

            if (!ModelState.IsValid)
            {
                return View(oldObj);
            }

            //Gán giá trị

            oldObj.id = model.id;
            oldObj.controller_Name = model.controller_Name;
            oldObj.controller_Code = model.controller_Code;
            oldObj.controller_Address = model.controller_Address;
            oldObj.com_Port = model.com_Port;
            oldObj.description = model.description;
            oldObj.Relays_Number = model.Relays_Number;
            oldObj.Readers_Number = model.Readers_Number;
            oldObj.Inputs_Number = model.Inputs_Number;
            oldObj.baud_Rate = model.baud_Rate;
            oldObj.comm_Type = model.comm_Type;
            oldObj.controller_Type = model.controller_Type;
            oldObj.Inactive = model.Inactive;

            //Thực hiện cập nhật
            var result = await _tbl_ControllerService.Update(oldObj);


            if (result.isSuccess)
            {

                await LogHelper.WriteLog(oldObj.id.ToString(), ActionConfig.Update, JsonConvert.SerializeObject(oldObj), HttpContext);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }

        #endregion Cập nhật

        #region Xóa

        /// <summary>
        /// Xóa
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="id">Id bản ghi</param>
        /// <returns></returns>

        [CheckSessionCookie(AreaConfig.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _tbl_ControllerService.DeleteById(id);
            if (result.isSuccess)
            {
                await LogHelper.WriteLog(id, ActionConfig.Delete, id, HttpContext);
            }

            return Json(result);
        }


        #endregion Xóa
    }
}
