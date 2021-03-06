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
    public interface IAuthorManager
    {
        Task<Authors> Create(Authors inputModel);
        Task Update(Authors inputModel);
        Task Delete(long id);
        Task<Authors> Find_By_Id(long id);
        Task<Authors> Find_By_Name(string inputName);
        Task<List<Authors>> Look_up(long newsSourceId);
        Task<List<Authors>> Get_List(string name, int status,long newSourceId, int pageSize, int pageNumber);
    }
    public class AuthorManager : IAuthorManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Authors> _logger;
        public AuthorManager(IUnitOfWork unitOfWork, ILogger<Authors> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Authors> Create(Authors inputModel)
        {
            try
            {
                var result = await _unitOfWork.AuthorsRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Authors inputModel)
        {
            try
            {
                await _unitOfWork.AuthorsRepository.Update(inputModel);
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
            var item = await _unitOfWork.AuthorsRepository.Get(c => c.Id == id);
            item.Status = (byte)RolesEnum.Delete;
            await _unitOfWork.AuthorsRepository.Update(item);
            await _unitOfWork.SaveChange();
        }

        public async Task<Authors> Find_By_Id(long id)
        {
            return  _unitOfWork.AuthorsRepository.Find_By_Id_Querry(id);
        }

        public async Task<Authors> Find_By_Name(string inputName)
        {
            return await _unitOfWork.AuthorsRepository.Get(c => c.Name.ToLower() == inputName.Trim().ToLower());
        }

        public async Task<List<Authors>> Get_List(string name, int status, long newSourceId, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = _unitOfWork.AuthorsRepository.GetFullList().Where(x => (x.Status == status || status == (int)StatusEnum.All) &&
                                                                    (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                    && (x.NewsSourcesId == newSourceId || newSourceId == 0)
                                                                    && ((string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name)))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Authors>> Look_up(long newsSourceId)
        {
            try
            {
                if(newsSourceId == 0)
                {
                    var data = (await _unitOfWork.AuthorsRepository.FindBy(c => c.Status == (int)StatusEnum.Active)).ToList();
                    return data;
                }
                else
                {
                    var data = (await _unitOfWork.AuthorsRepository.FindBy(c => c.Status == (int)StatusEnum.Active && c.NewsSourcesId == newsSourceId)).ToList();
                    return data;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
