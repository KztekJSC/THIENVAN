using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin
{
    public interface IReportService
    {

        public Task<GridModel<tbl_Event>> GetPagingInOut(string key,string sort, int pageNumber, int pageSize, string statusid, string IsFilterByTimeIn, string fdate, string tdate);

        public Task<SelectListModel_Chosen> GetDriveStatus(string id = "", string placeholder = "", string selecteds = "");

        public Task<List<tbl_Event_Custom>> GetPagingInOut_Excel(string key,string sort, int page, int v, string status, string isCheckByTime, string fromdate, string todate);
    }
}
