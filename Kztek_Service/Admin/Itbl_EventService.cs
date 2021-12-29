using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin
{
    public interface Itbl_EventService
    {
        Task<GridModel<tbl_Event_Submit>> ReportEventIn( int pageIndex, int pageSize);
        Task<List<tbl_Event_Submit>> ReportALlEventIn(string key,string fromdate);
        Task<List<tbl_Event_Submit>> ReportALlEventService(string key, string fromdate);
        Task<tbl_Event> GetById(string id);
        Task<bool> Update2(tbl_Event model);
        Task<MessageReport> Update(tbl_Event model);
        Task<List<tbl_Event_Submit>> GetTopService();
        Task<List<tbl_Event_Submit>> ReportEventInToday();
    }
}
