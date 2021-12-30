using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Kztek_Service.Admin.Database.SQLSERVER
{
    public class tblPCService : ItblPCService
    {
        private ItblPCRepository _tblPCRepository;
       


        public tblPCService(ItblPCRepository _tblPCRepository)
        {
            this._tblPCRepository = _tblPCRepository;
          

        }
        public async Task<IEnumerable<tblPC>> GetAllActive()
        {
            var query = from n in _tblPCRepository.Table
                       
                        select n;

            return await Task.FromResult(query.ToList());
        }

        public async Task<IEnumerable<tblPC>> GetAllByGateId(string id)
        {
            //var query = from n in _tblPCRepository.Table
            //            where n.GateID == id
            //            select n;

            //return await Task.FromResult(query.ToList());
            return null;
        }

        public async Task<GridModel<tblPC>> GetAllCustomPagingByFirst(string key, string gate, int pageNumber, int pageSize)
        {
            //var query = (from n in _tblPCRepository.Table


            //             select new tblPCCustomViewModel()
            //             {
            //                 ComputerName = n.ComputerName,
            //                 Description = n.Description,
            //                 GateID = n.GateID,

            //                 Inactive = n.Inactive,
            //                 IPAddress = n.IPAddress,
            //                 PCID = n.PCID.ToString(),
            //                 SortOrder = n.SortOrder
            //             });
            var query = from n in _tblPCRepository.Table
                        select n;
            if (!string.IsNullOrWhiteSpace(key))
            {
                query = query.Where(n => n.pc_Name.Contains(key.Trim()) );
            }

          

            var pageList = query.ToPagedList(pageNumber, pageSize);

            var model = GridModelHelper<tblPC>.GetPage(pageList.OrderByDescending(n => n.pc_Code).ToList(), pageNumber, pageSize, pageList.TotalItemCount, pageList.PageCount);

            return await Task.FromResult(model);

        }

        public async Task<MessageReport> Create(tblPC model)
        {
            return await _tblPCRepository.Add(model);
        }

        public async Task<MessageReport> DeleteById(string id)
        {
            var result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:ERR"));

            var obj = GetById(id);
            if (obj.Result != null)
            {
                return await _tblPCRepository.Remove(obj.Result);
            }
            else
            {
                result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:NON_RECORD"));
            }

            return await Task.FromResult(result);
        }

        public async Task<tblPC> GetById(string id)
        {
            return await _tblPCRepository.GetOneById(id);
        }

        public async Task<MessageReport> Update(tblPC model)
        {
            return await _tblPCRepository.Update(model);
        }

        public async Task<tblPC> GetByName(string name)
        {
            var query = from n in _tblPCRepository.Table
                        where n.pc_Name == name
                        select n;


            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<tblPC> GetByName_Id(string name, string id)
        {
            var query = from n in _tblPCRepository.Table
                        where n.pc_Name == name && n.id.ToString() != id
                        select n;


            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<List<tbl_Lane_PC_Custom>> GetLanePCs(string pcid)
        {
            var query = new StringBuilder();

            query.AppendLine("select lp.lane_ID,lp.lane_order,la.lane_Name from tbl_Lane_PC lp");

            query.AppendLine("left join tbl_Lane la on lp.lane_ID = la.id");

            query.AppendLine(string.Format("where lp.pc_ID = '{0}'", pcid));

            var list = DatabaseHelper.ExcuteCommandToList<tbl_Lane_PC_Custom>(query.ToString());

            return await Task.FromResult(list);
        }

        public async Task<List<tbl_Lane_Controller_Custom>> GetLaneControllers(IEnumerable<string> laneids)
        {
            var query = new StringBuilder();

            query.AppendLine("select lc.id,lc.controller_ID,lc.lane_ID,c.controller_Name,lc.barrie_Index,lc.input_Index,lc.reader_Index from tbl_Lane_Controller lc ");

            query.AppendLine("left join tbl_Controller c on lc.controller_ID = c.id");

            if (laneids != null && laneids.Count() > 0)
            {
                var count = 0;

                query.AppendLine("where lc.lane_ID IN ( ");

                foreach (var item in laneids)
                {
                    count++;

                    query.AppendLine(string.Format("'{0}'{1}", item, count == laneids.Count() ? "" : ","));
                }

                query.AppendLine(" ) ");


            }

            var list = DatabaseHelper.ExcuteCommandToList<tbl_Lane_Controller_Custom>(query.ToString());

            return await Task.FromResult(list);
        }

        public async Task<List<tbl_Lane_Led_Custom>> GetLaneLeds(IEnumerable<string> laneids)
        {
            var query = new StringBuilder();

            query.AppendLine("select lc.id,lc.LED_ID,lc.lane_ID,c.Name as LED_Name from tbl_Lane_Led lc ");

            query.AppendLine("left join tbl_LED c on lc.LED_ID = c.id");

            if (laneids != null && laneids.Count() > 0)
            {
                var count = 0;

                query.AppendLine("where lc.lane_ID IN ( ");

                foreach (var item in laneids)
                {
                    count++;

                    query.AppendLine(string.Format("'{0}'{1}", item, count == laneids.Count() ? "" : ","));
                }

                query.AppendLine(" ) ");


            }

            var list = DatabaseHelper.ExcuteCommandToList<tbl_Lane_Led_Custom>(query.ToString());

            return await Task.FromResult(list);
        }
    }
}
