using Kztek_Core.Models;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Database
{
    public interface Itbl_Lane_LedService
    {
        Task<GridModel<tbl_Lane_Led>> GetAllCustomPagingByFirst(string key, string pc, int page, int v);

        Task<MessageReport> Create(tbl_Lane_Led obj);

        Task<MessageReport> Update(tbl_Lane_Led obj);

        Task<tbl_Lane_Led> GetById(string id);

        Task<tbl_Lane_Led_Custom> GetByCustomId(string id);
        Task<MessageReport> DeleteById(string id);

        Task<tbl_Lane_Led> getByLED_ID(string LED_ID);
        Task<tbl_Lane_Led> GetByLaneLed(string laneid, string ledid);
    }
}
