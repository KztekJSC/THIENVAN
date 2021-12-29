using Kztek_Core.Models;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Admin;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Database.SQLSERVER
{
    public class ReportService : IReportService
    {
        public async Task<SelectListModel_Chosen> GetDriveStatus(string id = "", string placeholder = "", string selecteds = "")
        {
            var data =  StaticList.ListStatus();
            var cus = new List<SelectListModel>();
            var lst = data;
            if (lst != null && lst.Count > 0)
            {
                cus.Add(new SelectListModel()
                {
                    ItemText = "---- Lựa chọn ----",
                    ItemValue = "00"
                }) ;

                cus.AddRange(data.Select(n => new SelectListModel()
                {
                    ItemText = n.ItemText,
                    ItemValue = n.ItemValue
                }));
            }

            var model = new SelectListModel_Chosen()
            {
                IdSelectList = "StatusID",
                Selecteds = selecteds,
                Placeholder = placeholder,
                Data = cus.ToList(),
                isMultiSelect = false
            };
            return model;
        }

        public async Task<GridModel<tbl_Event>> GetPagingInOut(string key,string sort, int pageNumber, int pageSize, string statusid, string isCheckByTime = "0", string fdate = "", string tdate ="")
        {

            var sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine(string.Format("SELECT ROW_NUMBER () OVER ( ORDER BY {0} desc) as RowNumber,a.*",sort));
            sb.AppendLine("FROM(");
            sb.AppendLine("  select * from [tbl_Event]");
            sb.AppendLine("WHere 1 =1");
            if (!string.IsNullOrEmpty(key))
            {
                sb.AppendLine(string.Format("and ([plate_Number1] LIKE '%{0}%' OR [plate_Number2] LIKE '%{0}%' OR [customer_Name] LIKE '%{0}%'  OR [driver_Name] LIKE '%{0}%')", key));
            }

            switch (isCheckByTime)
            {

                case "0"://Giờ vào
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND event_DateTime_Gate >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND event_DateTime_Gate < '{0}'", todate));
                    }
                    break;
                
                case "1"://ggiờ đăng kí dịch vụ
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        //query = query.Where(n => n.ExpireDate >= fdate);
                        sb.AppendLine(string.Format("AND event_DateTime_Service >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        //query = query.Where(n => n.ExpireDate < tdate);
                        sb.AppendLine(string.Format("AND [event_DateTime_Service] < '{0}'", todate));
                    }
                    break;
                case "2"://iờ xuât hàng
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND [event_DateTime_Warehouse] >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND [event_DateTime_Warehouse] < '{0}'", todate));
                    }
                    break;
                //case "4"://Không check thời gian
                   
                //    break;

                default:
                    break;
            }
            //event Code
            if (!string.IsNullOrWhiteSpace(statusid) && statusid != "00")
            {
                var t = statusid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Any())
                {
                    var count = 0;
                  
                    sb.AppendLine("and ([event_Code] IN ( ");

                    foreach (var item in t)
                    {
                        count++;

                        sb.AppendLine(string.Format("'{0}'{1}", item, count == t.Length ? "" : ","));
                    }

                    sb.AppendLine(" )) ");

                   
                }
            }
            sb.AppendLine(")as a");
            sb.AppendLine(") as C1");        
            sb.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageNumber, pageSize));
            var listData = DatabaseHelper.ExcuteCommandToList<tbl_Event>(sb.ToString());


            // Tính tổng
            sb.Clear();
            sb.AppendLine("SELECT COUNT(*) TotalCount");

            sb.AppendLine("FROM [tbl_Event] where 1 = 1");
            {
                sb.AppendLine(string.Format("and ([plate_Number1] LIKE '%{0}%' OR [plate_Number2] LIKE '%{0}%' OR [customer_Name] LIKE '%{0}%'  OR [driver_Name] LIKE '%{0}%')", key));
            }


            switch (isCheckByTime)
            {

                case "0"://Giờ vào
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND event_DateTime_Gate >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND event_DateTime_Gate < '{0}'", todate));
                    }
                    break;

                case "1"://ggiờ đăng kí dịch vụ
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        //query = query.Where(n => n.ExpireDate >= fdate);
                        sb.AppendLine(string.Format("AND event_DateTime_Service >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        //query = query.Where(n => n.ExpireDate < tdate);
                        sb.AppendLine(string.Format("AND [event_DateTime_Service] < '{0}'", todate));
                    }
                    break;
                case "2"://iờ xuât hàng
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND [event_DateTime_Warehouse] >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND [event_DateTime_Warehouse] < '{0}'", todate));
                    }
                    break;
                //case "4"://Không check thời gian

                //    break;

                default:
                    break;
            }
            //event Code
            if (!string.IsNullOrWhiteSpace(statusid) && statusid != "00")
            {
                var t = statusid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Any())
                {
                    var count = 0;

                    sb.AppendLine("and ([event_Code] IN ( ");

                    foreach (var item in t)
                    {
                        count++;

                        sb.AppendLine(string.Format("'{0}'{1}", item, count == t.Length ? "" : ","));
                    }

                    sb.AppendLine(" )) ");


                }
            }
            var _total = DatabaseHelper.ExcuteCommandToModel<TotalPagingModel>(sb.ToString());

            var model = GridModelHelper<tbl_Event>.GetPage(listData, pageNumber, pageSize, _total.TotalCount);

            return await Task.FromResult(model);

        }

        public async Task<List<tbl_Event_Custom>> GetPagingInOut_Excel(string key, string sort, int pageNumber, int pageSize, string statusid, string isCheckByTime = "0", string fdate = "", string tdate = "")
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format(" select ROW_NUMBER() OVER(ORDER BY {0} desc) AS RowNumber, [customer_Name] ,[driver_Name] , [plate_Number1] ",sort));
            sb.AppendLine("  ,[plate_Number2] ,[commodity_Name] ");
            sb.AppendLine("  ,[event_DateTime_Gate] ");
            sb.AppendLine("    ,[event_DateTime_Service] ");
            sb.AppendLine(" ,[event_DateTime_Warehouse] , [event_Code]   from [tbl_Event]");
            sb.AppendLine("WHERE 1 = 1");
            if (!string.IsNullOrEmpty(key))
            {
                sb.AppendLine(string.Format("and ([plate_Number1] LIKE '%{0}%' OR [plate_Number2] LIKE '%{0}%' OR [customer_Name] LIKE '%{0}%'  OR [driver_Name] LIKE '%{0}%')", key));
            }
            switch (isCheckByTime)
            {

                case "0"://Giờ vào
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND event_DateTime_Gate >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND event_DateTime_Gate < '{0}'", todate));
                    }
                    break;

                case "1"://ggiờ đăng kí dịch vụ
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        //query = query.Where(n => n.ExpireDate >= fdate);
                        sb.AppendLine(string.Format("AND event_DateTime_Service >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        //query = query.Where(n => n.ExpireDate < tdate);
                        sb.AppendLine(string.Format("AND [event_DateTime_Service] < '{0}'", todate));
                    }
                    break;
                case "2"://iờ xuât hàng
                    if (!string.IsNullOrWhiteSpace(fdate))
                    {
                        var fromdate = Convert.ToDateTime(fdate).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND [event_DateTime_Warehouse] >= '{0}'", fromdate));
                    }

                    if (!string.IsNullOrWhiteSpace(tdate))
                    {
                        var todate = Convert.ToDateTime(tdate).AddDays(1).ToString("yyyy/MM/dd");

                        sb.AppendLine(string.Format("AND [event_DateTime_Warehouse] < '{0}'", todate));
                    }
                    break;
                //case "4"://Không check thời gian

                //    break;

                default:
                    break;
            }
            //event Code
            if (!string.IsNullOrWhiteSpace(statusid) && statusid != "00")
            {
                var t = statusid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Any())
                {
                    var count = 0;

                    sb.AppendLine("and ([event_Code] IN ( ");

                    foreach (var item in t)
                    {
                        count++;

                        sb.AppendLine(string.Format("'{0}'{1}", item, count == t.Length ? "" : ","));
                    }

                    sb.AppendLine(" )) ");


                }
            }
            var lst = new List<tbl_Event_Custom>();
            var listData = DatabaseHelper.ExcuteCommandToList<tbl_Event_Excel>(sb.ToString());
            if (listData.Count > 0)
            {
                foreach (var item in listData)
                {
                    var obj = new tbl_Event_Custom();
                    obj.RowNumber = item.RowNumber;
                    obj.customer_Name = item.customer_Name;
                    obj.driver_Name = item.driver_Name;
                    obj.plate_Number1 = item.plate_Number1;
                    obj.plate_Number2 = item.plate_Number2;
                    obj.commodity_Name = item.commodity_Name;
                    if (item.event_DateTime_Gate.ToString("dd/MM/yyyy") == "01/01/0001" || item.event_DateTime_Gate.ToString("dd/MM/yyyy") == "01/01/1900")
                    {
                        obj.event_DateTime_Gate = "";
                    }
                    else
                    {
                        obj.event_DateTime_Gate = item.event_DateTime_Gate.ToString();
                    }
                    if (item.event_DateTime_Service.ToString("dd/MM/yyyy") == "01/01/0001" || item.event_DateTime_Service.ToString("dd/MM/yyyy") == "01/01/1900")
                    {
                        obj.event_DateTime_Service = "";
                    }
                    else
                    {
                        obj.event_DateTime_Service = item.event_DateTime_Service.ToString();
                    }

                    if (item.event_DateTime_Warehouse.ToString("dd/MM/yyyy") == "01/01/0001" || item.event_DateTime_Warehouse.ToString("dd/MM/yyyy") == "01/01/1900")
                    {
                        obj.event_DateTime_Warehouse = "";
                    }
                    else
                    {
                        obj.event_DateTime_Warehouse = item.event_DateTime_Warehouse.ToString();
                    }

                    if (item.event_Code == 0)
                    {
                        obj.event_Code_Name = "Xe mới vào cổng ";
                    }
                    else if (item.event_Code == 1)
                    {
                        obj.event_Code_Name = "Xe đã làm thủ tục giấy tờ";
                    }
                    else if (item.event_Code == 2)
                    {
                        obj.event_Code_Name = "Xe đã xuất hàng";
                    }
                    lst.Add(obj);
                }
            }
         
            return lst;
        }
    }
}
