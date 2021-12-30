using Kztek_Core.Models;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Database
{
    public interface Itbl_Lane_ControllerService
    {
        Task<GridModel<tbl_Lane_Controller>> GetAllCustomPagingByFirst(string key, string pc, int page, int v);

        Task<MessageReport> Create(tbl_Lane_Controller obj);

        Task<MessageReport> Update(tbl_Lane_Controller obj);

        Task<tbl_Lane_Controller> GetById(string id);

        Task<tbl_Lane_Controller> GetByLaneController(string laneid, string controllerid);
        Task<MessageReport> DeleteById(string id);


    }
}
