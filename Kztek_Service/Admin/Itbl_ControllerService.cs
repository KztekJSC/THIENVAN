using Kztek_Core.Models;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin
{
    public interface Itbl_ControllerService
    {
        Task<tbl_Controller> GetById(string id);

        Task<MessageReport> Create(tbl_Controller obj);

        Task<MessageReport> Update(tbl_Controller obj);

        Task<MessageReport> DeleteById(string id);

        Task<IEnumerable<tbl_Controller>> GetAllActiveByPC(string id);

        Task<GridModel<tbl_Controller>> GetAllCustomPagingByFirst(string key, string pc, int pageNumber, int pageSize);

        Task<tbl_Controller> GetByName(string name);

        Task<tbl_Controller> GetByName_Id(string name, string id);
        Task<List<tbl_Controller>> GetAllActive();
    }
}
