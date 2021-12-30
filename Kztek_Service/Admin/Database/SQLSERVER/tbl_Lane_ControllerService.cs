using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Kztek_Service.Admin.Database.SQLSERVER
{

    public class tbl_Lane_ControllerService : Itbl_Lane_ControllerService
    {
        private Itbl_Lane_ControllerRepository tbl_Lane_ControllerRepository;
        public tbl_Lane_ControllerService(Itbl_Lane_ControllerRepository tbl_Lane_ControllerRepository)
        {
            this.tbl_Lane_ControllerRepository = tbl_Lane_ControllerRepository;
        }

        public async Task<MessageReport> Create(tbl_Lane_Controller obj)
        {
            return await tbl_Lane_ControllerRepository.Add(obj);
        }

        public async Task<MessageReport> DeleteById(string id)
        {
            var result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:ERR"));

            var obj = GetById(id);
            if (obj.Result != null)
            {
                return await tbl_Lane_ControllerRepository.Remove(obj.Result);
            }
            else
            {
                result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:NON_RECORD"));
            }

            return await Task.FromResult(result);
        }
        public async Task<GridModel<tbl_Lane_Controller>> GetAllCustomPagingByFirst(string key, string pc, int pageNumber, int pageSize)
        {
            var query = from n in tbl_Lane_ControllerRepository.Table
                        select n;




            var pageList = query.ToPagedList(pageNumber, pageSize);

            var model = GridModelHelper<tbl_Lane_Controller>.GetPage(pageList.OrderByDescending(n => n.lane_ID).ToList(), pageNumber, pageSize, pageList.TotalItemCount, pageList.PageCount);

            return await Task.FromResult(model);
        }

        public async Task<tbl_Lane_Controller> GetByLaneController(string laneid, string controllerid)
        {
            var query = from n in tbl_Lane_ControllerRepository.Table
                        where n.lane_ID == laneid && n.controller_ID == controllerid
                        select n;

            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<tbl_Lane_Controller> GetById(string id)
        {
            return await tbl_Lane_ControllerRepository.GetOneById(id);
        }


        public async Task<MessageReport> Update(tbl_Lane_Controller obj)
        {
            return await tbl_Lane_ControllerRepository.Update(obj);
        }
    }
}
