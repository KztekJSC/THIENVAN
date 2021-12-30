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

    public class tbl_Lane_LedService : Itbl_Lane_LedService
    {
        private Itbl_Lane_LedRepository tbl_Lane_LedRepository;
        public tbl_Lane_LedService(Itbl_Lane_LedRepository tbl_Lane_LedRepository)
        {
            this.tbl_Lane_LedRepository = tbl_Lane_LedRepository;
        }

        public async Task<MessageReport> Create(tbl_Lane_Led obj)
        {
            return await tbl_Lane_LedRepository.Add(obj);
        }

        public async Task<MessageReport> DeleteById(string id)
        {
            var result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:ERR"));

            var obj = GetById(id);
            if (obj.Result != null)
            {
                return await tbl_Lane_LedRepository.Remove(obj.Result);
            }
            else
            {
                result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:NON_RECORD"));
            }

            return await Task.FromResult(result);
        }
        public async Task<GridModel<tbl_Lane_Led>> GetAllCustomPagingByFirst(string key, string pc, int pageNumber, int pageSize)
        {
            var query = from n in tbl_Lane_LedRepository.Table
                        select n;




            var pageList = query.ToPagedList(pageNumber, pageSize);

            var model = GridModelHelper<tbl_Lane_Led>.GetPage(pageList.OrderByDescending(n => n.lane_ID).ToList(), pageNumber, pageSize, pageList.TotalItemCount, pageList.PageCount);

            return await Task.FromResult(model);
        }

        public async Task<tbl_Lane_Led_Custom> GetByCustomId(string id)
        {
            var obj = await GetById(id);

            var model = new tbl_Lane_Led_Custom()
            {
                id = obj.id.ToString(),
                lane_ID = obj.lane_ID,
                LED_ID = obj.LED_ID

            };
            return model;
        }

        public async Task<tbl_Lane_Led> GetById(string id)
        {
            return await tbl_Lane_LedRepository.GetOneById(id);
        }

        public async Task<tbl_Lane_Led> getByLED_ID(string LED_ID)
        {
            var query = from n in tbl_Lane_LedRepository.Table
                        where n.LED_ID == LED_ID
                        select n;
            return await Task.FromResult(query.FirstOrDefault());
        }
        public async Task<tbl_Lane_Led> GetByLaneLed(string laneid,string ledid)
        {
            var query = from n in tbl_Lane_LedRepository.Table
                        where n.lane_ID == laneid && n.LED_ID == ledid
                        select n;
            return await Task.FromResult(query.FirstOrDefault());
        }
        public async Task<MessageReport> Update(tbl_Lane_Led obj)
        {
            return await tbl_Lane_LedRepository.Update(obj);
        }
    }
}
