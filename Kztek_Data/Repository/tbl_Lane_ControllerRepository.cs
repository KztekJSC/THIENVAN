using System;
using Kztek_Data;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Kztek_Data.Repository
{
    public interface Itbl_Lane_ControllerRepository : IRepository<tbl_Lane_Controller>
    {
    }

    public class tbl_Lane_ControllerRepository : Repository<tbl_Lane_Controller>, Itbl_Lane_ControllerRepository
    {
        public tbl_Lane_ControllerRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}
