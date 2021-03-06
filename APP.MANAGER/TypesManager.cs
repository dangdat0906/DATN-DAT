using Microsoft.Extensions.Logging;
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
    public interface ITypesManager
    {
        Task<Types> Create(Types inputModel);
        Task Update(Types inputModel);
        Task Delete(long id);
        Task<Types> Find_By_Id(long id);
        Task<Types> Find_By_Name(string inputName);
        Task<List<Types>> Look_up();
        Task<List<Types>> Get_List(string name, int status, int pageSize, int pageNumber);
    }
    public class TypesManager: ITypesManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Types> _logger;
        public TypesManager(IUnitOfWork unitOfWork, ILogger<Types> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Types> Create(Types inputModel)
        {
            try
            {
                var result = await _unitOfWork.TypesRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Types inputModel)
        {
            try
            {
                await _unitOfWork.TypesRepository.Update(inputModel);
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
            var item = await _unitOfWork.TypesRepository.Get(c => c.Id == id);
            item.Status = (byte)RolesEnum.Delete;
            await _unitOfWork.TypesRepository.Update(item);
            await _unitOfWork.SaveChange();
        }

        public async Task<Types> Find_By_Id(long id)
        {
            return await _unitOfWork.TypesRepository.Get(c => c.Id == id);
        }

        public async Task<Types> Find_By_Name(string inputName)
        {
            return await _unitOfWork.TypesRepository.Get(c => c.Name.ToLower() == inputName.Trim().ToLower());
        }

        public async Task<List<Types>> Get_List(string name, int status, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = (await _unitOfWork.TypesRepository.FindBy(x => (x.Status == status || status == (int)StatusEnum.All) &&
                                                                    (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                    && ((string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name))))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Types>> Look_up()
        {
            try
            {
                var data = (await _unitOfWork.TypesRepository.FindBy(c => c.Status == (int)StatusEnum.Active)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
