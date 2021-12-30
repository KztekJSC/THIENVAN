using System;
using Kztek_Data;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Kztek_Data.Repository
{
    public interface Itbl_Lane_LedRepository : IRepository<tbl_Lane_Led>
    {
    }

    public class tbl_Lane_LedRepository : Repository<tbl_Lane_Led>, Itbl_Lane_LedRepository
    {
        public tbl_Lane_LedRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}
