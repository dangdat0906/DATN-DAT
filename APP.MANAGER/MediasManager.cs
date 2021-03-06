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
    public interface IMediasManager
    {
        Task<Medias> Create(Medias inputModel);
        Task Update(Medias inputModel);
        Task Delete(Medias item);
        Task<Medias> Find_By_Id(long id);
        Task<Medias> Find_By_Url(string inputCode);
        Task<List<Medias>> Get_List_By_Folder(string inputFolder);
        Task<List<Medias>> Look_up();
        Task<List<Medias>> Get_List(int status,int type, string folder, int pageSize, int pageNumber);
    }
    public class MediasManager : IMediasManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Medias> _logger;
        public MediasManager(IUnitOfWork unitOfWork, ILogger<Medias> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Medias> Create(Medias inputModel)
        {
            try
            {
                var result = await _unitOfWork.MediasRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Medias inputModel)
        {
            await _unitOfWork.CreateTransaction();
            try
            {
                await _unitOfWork.MediasRepository.Update(inputModel);
                await _unitOfWork.Commit();
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();
                throw ex;
            }
        }
        public async Task<Medias> Find_By_Id(long id)
        {
            return await _unitOfWork.MediasRepository.Get(c => c.Id == id);
        }
        public async Task<Medias> Find_By_Url(string inputCode)
        {
            return await _unitOfWork.MediasRepository.Get(c => c.Url.Trim() == inputCode.Trim());
        }
        public async Task<List<Medias>> Look_up()
        {
            return (await _unitOfWork.MediasRepository.FindBy(c => c.Status == (int)StatusEnum.Active)).ToList();
        }
        public async Task<List<Medias>> Get_List(int status,int type, string folder, int pageSize = 30, int pageNumber = 1)
        {
            try
            {
                var data = (await _unitOfWork.MediasRepository.FindBy(x => (x.Status == status || status == (int)StatusEnum.All)
                                                                     && (x.Status == status || status == (int)StatusEnum.All)
                                                                     && (string.IsNullOrEmpty(folder) || x.Folder.Trim().ToLower() == folder.Trim().ToLower() )
                                                                     && (x.Type == type)
                                                                     ))
                                                                     .OrderByDescending(d => d.CreatedDate)
                                                                     .ToList();
                return (List<Medias>)data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Delete(Medias item)
        {
            await _unitOfWork.MediasRepository.Delete(item);
            await _unitOfWork.SaveChange();
        }
        public async Task<List<Medias>> Get_List_By_Folder(string inputFolder)
        {
            var listdata = (await _unitOfWork.MediasRepository.FindBy(x => x.Folder == inputFolder)).ToList();
            return listdata;
        }
    }
}