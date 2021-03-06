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
  
    public interface INewsSourcesManager
    {
        Task<NewsSources> Create(NewsSources inputModel);
        Task Update(NewsSources inputModel);
        Task Delete(long id);
        Task<NewsSources> Find_By_Id(long id);
        Task<NewsSources> Find_By_Name(string inputName);
        Task<List<NewsSources>> Look_up();
        Task<List<NewsSources>> Get_List(string name, int status, int pageSize, int pageNumber);
    }
    public class NewsSourcesManager : INewsSourcesManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NewsSources> _logger;
        public NewsSourcesManager(IUnitOfWork unitOfWork, ILogger<NewsSources> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<NewsSources> Create(NewsSources inputModel)
        {
            try
            {
                var result = await _unitOfWork.NewsSourcesRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(NewsSources inputModel)
        {
            try
            {
                await _unitOfWork.NewsSourcesRepository.Update(inputModel);
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
            var item = await _unitOfWork.NewsSourcesRepository.Get(c => c.Id == id);
            item.Status = (byte)RolesEnum.Delete;
            await _unitOfWork.NewsSourcesRepository.Update(item);
            await _unitOfWork.SaveChange();
        }

        public async Task<NewsSources> Find_By_Id(long id)
        {
            return await _unitOfWork.NewsSourcesRepository.Get(c => c.Id == id);
        }

        public async Task<NewsSources> Find_By_Name(string inputName)
        {
            return await _unitOfWork.NewsSourcesRepository.Get(c => c.Name.ToLower() == inputName.Trim().ToLower());
        }

        public async Task<List<NewsSources>> Get_List(string name, int status, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = (await _unitOfWork.NewsSourcesRepository.FindBy(x => (x.Status == status || status == (int)StatusEnum.All) &&
                                                                    (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                    && ((string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name))))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<NewsSources>> Look_up()
        {
            try
            {
                var data = (await _unitOfWork.NewsSourcesRepository.FindBy(c => c.Status == (int)StatusEnum.Active)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
