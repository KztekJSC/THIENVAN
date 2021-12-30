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
    public class tbl_ControllerService : Itbl_ControllerService
    {
        private Itbl_ControllerRepository _tbl_ControllerRepository;
        public tbl_ControllerService(Itbl_ControllerRepository _tbl_ControllerRepository)
        {
            this._tbl_ControllerRepository = _tbl_ControllerRepository;
        }
        public async Task<MessageReport> Create(tbl_Controller obj)
        {
            return await _tbl_ControllerRepository.Add(obj);
        }

        public async Task<MessageReport> DeleteById(string id)
        {
            var result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:ERR"));

            var obj = GetById(id);
            if (obj.Result != null)
            {
                return await _tbl_ControllerRepository.Remove(obj.Result);
            }
            else
            {
                result = new MessageReport(false, await LanguageHelper.GetLanguageText("MESSAGEREPORT:NON_RECORD"));
            }

            return await Task.FromResult(result);
        }

        public async Task<List<tbl_Controller>> GetAllActive()
        {
            var query = from n in _tbl_ControllerRepository.Table
                        select n;

            return await Task.FromResult(query.ToList());
        }

        public Task<IEnumerable<tbl_Controller>> GetAllActiveByPC(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<GridModel<tbl_Controller>> GetAllCustomPagingByFirst(string key, string pc, int pageNumber, int pageSize)
        {
            var query = from n in _tbl_ControllerRepository.Table
                        select n;
          
            if (!string.IsNullOrWhiteSpace(key))
            {
                query = query.Where(n => n.controller_Name.Contains(key));
            }

            var pageList = query.ToPagedList(pageNumber, pageSize);

            var model = GridModelHelper<tbl_Controller>.GetPage(pageList.OrderByDescending(n => n.controller_Name).ToList(), pageNumber, pageSize, pageList.TotalItemCount, pageList.PageCount);

            return await Task.FromResult(model);
        }

        public async Task<tbl_Controller> GetById(string id)
        {
            return await _tbl_ControllerRepository.GetOneById(id);
        }

        public async Task<tbl_Controller> GetByName(string name)
        {
            var query = from n in _tbl_ControllerRepository.Table
                        where n.controller_Name == name
                        select n;

            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<tbl_Controller> GetByName_Id(string name, string id)
        {
            var query = from n in _tbl_ControllerRepository.Table
                        where n.controller_Name == name && n.id.ToString().ToLower() != id
                        select n;

            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<MessageReport> Update(tbl_Controller obj)
        {
            return await _tbl_ControllerRepository.Update(obj);
        }
    
    }
}
