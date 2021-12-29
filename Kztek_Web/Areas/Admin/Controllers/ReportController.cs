

using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Admin;
using Kztek_Web.Attributes;
using Kztek_Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;

namespace Kztek_Web.Areas.Admin.Controllers
{
    [Area(AreaConfig.Admin)]
    public class ReportController : Controller
    {
        #region Service
        private IReportService _ReportService;
        private Itbl_EventService _tbl_EventService;
        public ReportController(IReportService _ReportService, Itbl_EventService _tbl_EventService)
        {
            this._tbl_EventService = _tbl_EventService;
            this._ReportService = _ReportService;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        #region Sự kiện vào
        [CheckSessionCookie(AreaConfig.Admin)]
        public async Task<IActionResult> ReportService(string key ,string fromdate = "")
        {
            if (string.IsNullOrEmpty(fromdate))
            {
                fromdate = DateTime.Now.ToString("yyyy/MM/dd");
            }

            var list = await _tbl_EventService.ReportALlEventIn(key, fromdate);
            ViewBag.keyValue = key;
            ViewBag.fromdateValue = fromdate;
            return View(list);

        }

        public async Task<IActionResult> Partial_EventIn(int page = 1)
        {
            var gridModel = await _tbl_EventService.ReportEventIn(page, 10);
            return PartialView(gridModel);

        }

        public async Task<IActionResult> UpdateEvent(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await _tbl_EventService.GetById(id);

            obj.event_Code = 1;

            obj.event_DateTime_Service = DateTime.Now;

            result = await _tbl_EventService.Update(obj);

            if (result.isSuccess)
            {
                await SendLed(obj, "Led:IPService");

                result = new MessageReport(true, "");

                await SignalrHelper.SqlHub.Clients.All.SendAsync("tbl_Event", "service", obj);
            }
            return Json(result);

        }

        public async Task SendLed(tbl_Event obj,string path)
        {
            var ips = AppSettingHelper.GetStringFromAppSetting(path).Result;

           
        }
        #endregion

        #region Sự kiện chờ vào kho
        [CheckSessionCookie(AreaConfig.Admin)]
        public async Task<IActionResult> ReportWareHouse(string key, string fromdate = "")
        {
            if (string.IsNullOrEmpty(fromdate))
            {
                fromdate = DateTime.Now.ToString("yyyy/MM/dd");
            }
            var list = await _tbl_EventService.ReportALlEventService(key , fromdate);

            ViewBag.keyValue = key;
            ViewBag.fromdateValue = fromdate;

            return View(list);

        }

        public async Task<IActionResult> UpdateEventService(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await _tbl_EventService.GetById(id);

            obj.event_Code = 2;

            obj.event_DateTime_Warehouse = DateTime.Now;

            result = await _tbl_EventService.Update(obj);

            if (result.isSuccess)
            {
                await SendLed(obj, "Led:IPWareHouse");

                result = new MessageReport(true, "");

                await SignalrHelper.SqlHub.Clients.All.SendAsync("tbl_Event", "wareHouse", obj);
            }
            return Json(result);

        }
        #endregion

        [CheckSessionCookie(AreaConfig.Admin)]
        public async Task<ActionResult> ReportVehicleComeInOut(string StatusID = "", string key = "",string isCheckByTime = "0",  string chkExport = "0", string fromdate = "", string todate = "", int page = 1 , string  AreaCode = ""
)
        {
            var sort = "";
            if (isCheckByTime == "0")
            {
                sort = "event_DateTime_Gate";
            }
            else if (isCheckByTime == "1")
            {
                sort = "event_DateTime_Service";
            }
            else if (isCheckByTime == "2")
            {
                sort = "event_DateTime_Warehouse";
            }
            var datefrompicker = "";

            if (string.IsNullOrEmpty(fromdate))
            {
                fromdate = DateTime.Now.ToString("dd/MM/yyyy 00:00:00");
            }

            if (string.IsNullOrEmpty(todate))
            {
                todate = DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            }
            
            if (!string.IsNullOrWhiteSpace(fromdate) && !string.IsNullOrWhiteSpace(todate))
            {
                datefrompicker = fromdate + "-" + todate;
            }
            if (chkExport.Equals( "1"))
            {
                await ExportFile(key,sort, page, 20, StatusID, isCheckByTime, fromdate, todate,this.HttpContext);

                //return View(gridmodel);
            }
         

            #region Giao diện

            var gridModel = await _ReportService.GetPagingInOut(key,sort, page, 20, StatusID, isCheckByTime, fromdate,todate );      
            ViewBag.DriveStatus = await _ReportService.GetDriveStatus(selecteds: StatusID);
            ViewBag.isFilterByTimeIn = isCheckByTime;
            ViewBag.StatusID = StatusID;
            ViewBag.keyValue = key; 
            ViewBag.fromdateValue = fromdate;
           
            ViewBag.todateValue = string.IsNullOrWhiteSpace(todate) ? DateTime.Now.ToString("đd/MM/yyyy 23:59") : todate;
            ViewBag.AreaCodeValue = AreaCode;
            return View(gridModel);
            #endregion
        }
      
     
  
        private async Task<bool> ExportFile(string key,string sort, int page, int pageSize , string status, string isCheckByTime,string fromdate,string todate, HttpContext context)
        {

            //column header
            var Data_ColumnHeader = new List<SelectListModel_Print_Column_Header>();
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "STT" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Tên khách hàng" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Tên lái xe" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Biển số 1" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Biển số 2" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Loại hàng hóa   " });  
              Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Giờ vào cổng   " });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Giờ làm thủ tục" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Giờ xuất hàng" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Trạng thái xe" });
            //
            var printConfig = PrintHelper.Template_Excel_V1(PrintConfig.HeaderType.TwoColumns, "Sự kiện vào ra", DateTime.Now, SessionCookieHelper.CurrentUser(this.HttpContext).Result, "THADOSOFT", Data_ColumnHeader, 4, 5, 5);

            //
            var lstdata = await _ReportService.GetPagingInOut_Excel(key,sort, page, 10, status, isCheckByTime, fromdate, todate);
         
            return await PrintHelper.Excel_Write<tbl_Event_Custom>(context, lstdata, "Event_" + DateTime.Now.ToString("ddMMyyyyHHmmss"), printConfig);
        }
    }


}
