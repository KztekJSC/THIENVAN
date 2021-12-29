using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Database.SQLSERVER
{
    public class tbl_EventService: Itbl_EventService
    {
        private Itbl_EventRepository _tbl_EventRepository;
        private IUserService _UserService;
        private ItblLaneService _tblLaneService;
        private ItblPCService _tblPCService;

        public tbl_EventService(IUserService _UserService, ItblLaneService _tblLaneService,  ItblPCService _tblPCService, Itbl_EventRepository _tbl_EventRepository)
        {
            this._UserService = _UserService;
            this._tblLaneService = _tblLaneService;
            this._tblPCService = _tblPCService;

            this._tbl_EventRepository = _tbl_EventRepository;
        }
        public async Task<tbl_Event> GetById(string id)
        {
            var query = new StringBuilder();
            query.AppendLine(string.Format("SELECT * FROM tbl_Event where id ='{0}'", id));


            var dataSet = DatabaseHelper.ExcuteCommandToDataSet(query.ToString());

            List<tbl_Event> events = new List<tbl_Event>();

            events = (from DataRow dr in dataSet.Tables[0].Rows
                           select new tbl_Event()
                           {
                               id = dr["id"].ToString(),
                               event_Code = !string.IsNullOrEmpty(dr["event_Code"].ToString()) ? Convert.ToInt32(dr["event_Code"]) : 0,
                               event_Number = !string.IsNullOrEmpty(dr["event_Number"].ToString()) ? Convert.ToInt32(dr["event_Number"]) : 0,
                               plate_Number1 = dr["plate_Number1"].ToString(),
                               plate_Number2 = dr["plate_Number2"].ToString(),
                               weight = !string.IsNullOrEmpty(dr["weight"].ToString()) ? Convert.ToInt32(dr["weight"]) : 0,
                               customer_Name = dr["customer_Name"].ToString(),
                               driver_Name = dr["driver_Name"].ToString(),
                               commodity_Name = dr["commodity_Name"].ToString(),
                               description = dr["description"].ToString(),
                               imgPath_Lpr1 = dr["imgPath_Lpr1"].ToString(),
                               imgPath_Lpr2 = dr["imgPath_Lpr2"].ToString(),
                               imgPath_Panorama1 = dr["imgPath_Panorama1"].ToString(),
                               imgPath_Panorama2 = dr["imgPath_Panorama2"].ToString(),

                               event_DateTime_Gate = !string.IsNullOrEmpty(dr["event_DateTime_Gate"].ToString()) ? Convert.ToDateTime(dr["event_DateTime_Gate"].ToString()) : new DateTime(1900, 1, 1),

                               event_DateTime_Service = !string.IsNullOrEmpty(dr["event_DateTime_Service"].ToString()) ? Convert.ToDateTime(dr["event_DateTime_Service"].ToString()) : new DateTime(1900, 1, 1),

                               event_DateTime_Warehouse = !string.IsNullOrEmpty(dr["event_DateTime_Warehouse"].ToString()) ? Convert.ToDateTime(dr["event_DateTime_Warehouse"].ToString()) : new DateTime(1900,1,1),

                               abnormal_event = !string.IsNullOrEmpty(dr["abnormal_event"].ToString()) ? Convert.ToBoolean( dr["abnormal_event"].ToString()) : false,
                           }).ToList();

            return await Task.FromResult(events.FirstOrDefault());
        }

        public async Task<bool> Update2(tbl_Event model)
        {
            var query = new StringBuilder();
            query.AppendLine(string.Format("Update tbl_Event set event_Code ={0} ,event_DateTime_Service = '{1}' where id ='{2}'", model.event_Code, Convert.ToDateTime(model.event_DateTime_Service).ToString("yyyy/MM/dd HH:mm:ss"),model.id.ToString()));

         var a =  DatabaseHelper.ExcuteCommandToBool(query.ToString());

            return await Task.FromResult(a);
        }

        public async Task<MessageReport> Update(tbl_Event model)
        {       
            return await _tbl_EventRepository.Update(model);
        }

        #region ReportEventIn
        public async Task<GridModel<tbl_Event_Submit>> ReportEventIn( int pageIndex, int pageSize)
        {



            var query = new StringBuilder();
            query.AppendLine("SELECT * FROM(");
            query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[event_Number] asc) as RowNumber,a.*");
            query.AppendLine("FROM(");
            query.AppendLine("SELECT *");

            query.AppendLine("FROM dbo.[tbl_Event] e WITH(NOLOCK)");

            query.AppendLine("WHERE e.[event_Code] = '0'");
         

         


            query.AppendLine(") as a");
            query.AppendLine(") as TEMP");
            query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

            //--Count Total
            query.AppendLine("SELECT COUNT(Id) as totalCount");
            query.AppendLine("FROM ( SELECT Id FROM dbo.[tbl_Event]");
            query.AppendLine("e WITH(NOLOCK)");

            query.AppendLine("WHERE e.[event_Code] = '0'");

         


            query.AppendLine(") as e");

            var dataSet = DatabaseHelper.ExcuteCommandToDataSet(query.ToString());

            var total = dataSet.Tables.Count > 1 ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["totalCount"].ToString()) : 0;

            var list = SqlHelper.ConvertTo<tbl_Event_Submit>(dataSet.Tables[0]);         

            var model = GridModelHelper<tbl_Event_Submit>.GetPage(list, pageIndex, pageSize, total);

            return await Task.FromResult(model);
        }

        public async Task<List<tbl_Event_Submit>> ReportALlEventIn(string key,string fromdates)
        {


            
            var tdate = Convert.ToDateTime(fromdates);
            var query = new StringBuilder();
            query.AppendLine("SELECT * FROM(");
            query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[event_Number] asc) as RowNumber,a.*");
            query.AppendLine("FROM(");
            query.AppendLine("SELECT *");

            query.AppendLine("FROM dbo.[tbl_Event] e WITH(NOLOCK)");

            query.AppendLine("WHERE e.[event_Code] = '0'");

            if (!string.IsNullOrEmpty(key))
            {
                key = key.Trim();

                query.AppendLine(string.Format("and (e.[plate_Number1] LIKE '%{0}%' OR [customer_Name] LIKE N'%{0}%'  OR [commodity_Name] LIKE N'%{0}%')", key));
            }
            query.AppendLine(string.Format(" and  FORMAT(e.event_DateTime_Gate,'yyyyMMdd') = '{0}'   ", tdate.ToString("yyyyMMdd")));

            query.AppendLine(") as a");
            query.AppendLine(") as TEMP");
           

            var dataSet = DatabaseHelper.ExcuteCommandToDataSet(query.ToString());

            var total = dataSet.Tables.Count > 1 ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["totalCount"].ToString()) : 0;

            var list = SqlHelper.ConvertTo<tbl_Event_Submit>(dataSet.Tables[0]);

            return await Task.FromResult(list);
        }

        public async Task<List<tbl_Event_Submit>> ReportEventInToday()
        {



            var query = new StringBuilder();
            query.AppendLine("SELECT * FROM(");
            query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[event_Number] asc) as RowNumber,a.*");
            query.AppendLine("FROM(");
            query.AppendLine("SELECT *");

            query.AppendLine("FROM dbo.[tbl_Event] e WITH(NOLOCK)");

            query.AppendLine(string.Format("WHERE e.[event_Code] = '0' AND FORMAT(e.event_DateTime_Gate, 'yyyyMMdd') = '{0}'",DateTime.Now.ToString("yyyyMMdd")));

            query.AppendLine(") as a");
            query.AppendLine(") as TEMP");


            var dataSet = DatabaseHelper.ExcuteCommandToDataSet(query.ToString());

            var total = dataSet.Tables.Count > 1 ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["totalCount"].ToString()) : 0;

            var list = SqlHelper.ConvertTo<tbl_Event_Submit>(dataSet.Tables[0]);

            return await Task.FromResult(list);
        }

        public async Task<List<tbl_Event_Submit>> GetTopService()
        {

            var query = new StringBuilder();

            query.AppendLine("SELECT top 3 *");

            query.AppendLine("FROM dbo.[tbl_Event] e WITH(NOLOCK)");

            query.AppendLine("WHERE e.[event_Code] = '1' order by e.event_DateTime_Service desc");

          


            var dataSet = DatabaseHelper.ExcuteCommandToDataSet(query.ToString());

            var total = dataSet.Tables.Count > 1 ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["totalCount"].ToString()) : 0;

            var list = SqlHelper.ConvertTo<tbl_Event_Submit>(dataSet.Tables[0]);

            return await Task.FromResult(list);
        }

        #endregion

        public async Task<List<tbl_Event_Submit>> ReportALlEventService(string key, string fromdate)
        {


            var tdate = Convert.ToDateTime(fromdate);
            var query = new StringBuilder();
            query.AppendLine("SELECT * FROM(");
            query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[event_Number] asc) as RowNumber,a.*");
            query.AppendLine("FROM(");
            query.AppendLine("SELECT *");

            query.AppendLine("FROM dbo.[tbl_Event] e WITH(NOLOCK)");

            query.AppendLine("WHERE e.[event_Code] = '1'");

            if (!string.IsNullOrEmpty(key))
            {
                key = key.Trim();

                query.AppendLine(string.Format("and (e.[plate_Number1] LIKE '%{0}%' OR [customer_Name] LIKE N'%{0}%'  OR [commodity_Name] LIKE N'%{0}%')", key));
            }
            query.AppendLine(string.Format(" and  FORMAT(e.event_DateTime_Service,'yyyyMMdd') = '{0}'   ", tdate.ToString("yyyyMMdd")));
            query.AppendLine(") as a");
            query.AppendLine(") as TEMP");


            var dataSet = DatabaseHelper.ExcuteCommandToDataSet(query.ToString());

            var total = dataSet.Tables.Count > 1 ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["totalCount"].ToString()) : 0;

            var list = SqlHelper.ConvertTo<tbl_Event_Submit>(dataSet.Tables[0]);

            return await Task.FromResult(list);
        }
    }
}
