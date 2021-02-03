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
    public interface IRolesManger
    {
        Task<Roles> Create(Roles inputModel);
        Task Update(Roles inputModel);
        Task<List<Roles>> GetList(string name, int status, int pageSize = 10, int pageNumber = 0);
        Task<Roles> FindById(long id);
        Task<List<Roles>> LookUp();
        Task<Roles> FindByName(string name);
        Task Delete(long id);
    }
    public class RolesManager :IRolesManger
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RolesManager> _logger;
        public RolesManager(IUnitOfWork unitOfWork, ILogger<RolesManager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Roles> Create(Roles inputModel)
        {
            try
            {
                Roles data =  await _unitOfWork.RolesRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(long id)
        {
            try
            {
                var data = await _unitOfWork.RolesRepository.Get(x => x.Id == id);
                data.Status = (byte)RolesEnum.Delete;
                await _unitOfWork.RolesRepository.Update(data);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Roles> FindById(long id)
        {
            return await _unitOfWork.RolesRepository.Get(x => x.Id == id);
        }

        public async Task<Roles> FindByName(string name)
        {
            return await _unitOfWork.RolesRepository.Get(x => x.Name.Equals(name));

        }

        public async Task<List<Roles>> GetList(string name = "", int status = -1, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = (await _unitOfWork.RolesRepository.FindBy(x => (x.Status == status || status == (int)StatusEnum.All)&&
                                                                    (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed) 
                                                                    && (string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name)))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Roles>> LookUp()
        {
            try
            {
                return (await _unitOfWork.RolesRepository.FindBy(x => x.Status == (int)StatusEnum.Active)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Update(Roles inputModel)
        {
            try
            {
                await _unitOfWork.RolesRepository.Update(inputModel);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
