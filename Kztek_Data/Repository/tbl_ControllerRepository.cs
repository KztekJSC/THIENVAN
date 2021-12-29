using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Kztek_Data.Repository
{
    public interface Itbl_ControllerRepository : IRepository<tbl_Controller>
    {
    }

    public class tbl_ControllerRepository : Repository<tbl_Controller>, Itbl_ControllerRepository
    {
        public tbl_ControllerRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}
