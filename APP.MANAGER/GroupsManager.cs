using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using APP.MODELS;
using APP.REPOSITORY;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.MANAGER
{
    public interface IGroupsManager
    {
        Task<Groups> Create(Groups inputModel);
        Task Update(Groups inputModel);
        Task Delete(long id);
        Task<Groups> Find_By_Id(long id);
        Task<Groups> Find_By_Name(string inputName);
        Task<List<Groups>> Look_up();
        Task<List<Groups>> Get_List(string name, int status, int pageSize, int pageNumber);
    }
    public class GroupsManager : IGroupsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Groups> _logger;
        public GroupsManager(IUnitOfWork unitOfWork, ILogger<Groups> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Groups> Create(Groups inputModel)
        {
            try
            {
                var result = await _unitOfWork.GroupsRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Groups inputModel)
        {
            try
            {
                await _unitOfWork.GroupsRepository.Update(inputModel);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();
                throw ex;
            }
        }
        public async Task Delete(long id)
        {
            var item = await _unitOfWork.GroupsRepository.Get(c => c.Id == id);
            item.Status = (byte)RolesEnum.Delete;
            await _unitOfWork.GroupsRepository.Update(item);
            await _unitOfWork.SaveChange();
        }

        public async Task<Groups> Find_By_Id(long id)
        {
            return await _unitOfWork.GroupsRepository.Get(c => c.Id == id);
        }

        public async Task<Groups> Find_By_Name(string inputName)
        {
            return await _unitOfWork.GroupsRepository.Get(c => c.Name.ToLower() == inputName.Trim().ToLower());
        }

        public async Task<List<Groups>> Get_List(string name,int status, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = (await _unitOfWork.GroupsRepository.FindBy(x => (x.Status == status || status == (int)StatusEnum.All) &&
                                                                    (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed) 
                                                                    && ((string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name))))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Groups>> Look_up()
        {
            try
            {
                var data = (await _unitOfWork.GroupsRepository.FindBy(c => c.Status == (int)StatusEnum.Active)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
