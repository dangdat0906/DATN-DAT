using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.MANAGER
{
    public interface ITitleImagesManager
    {
        Task<TitleImages> Create(TitleImages inputModel);
        Task Update(TitleImages inputModel);
        Task Delete(long id);
        Task<TitleImages> Find_By_Id(long contentId,long titleId);  
        Task<List<TitleImages>> Get_List(long contentId = 0, long groupId = 0);
    }
    public class TitleImagesManager: ITitleImagesManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TitleImages> _logger;
        public TitleImagesManager(IUnitOfWork unitOfWork, ILogger<TitleImages> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<TitleImages> Create(TitleImages inputModel)
        {
            try
            {
                var result = await _unitOfWork.TitleImagesRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(TitleImages inputModel)
        {
            await _unitOfWork.CreateTransaction();
            try
            {
                await _unitOfWork.TitleImagesRepository.Update(inputModel);
                await _unitOfWork.Commit();
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
            var item = await _unitOfWork.TitleImagesRepository.Get(c => c.ContentId == id);
            await _unitOfWork.TitleImagesRepository.Delete(item);
            await _unitOfWork.Commit();
        }
        public async Task<TitleImages> Find_By_Id(long contentId, long titleId)
        {
            return await _unitOfWork.TitleImagesRepository.Get(c => (c.ContentId == contentId && c.Id == titleId));
        }

        public async Task<List<TitleImages>> Get_List(long contentId=0, long titleId=0)
        {
            try
            {
                var data = (await _unitOfWork.TitleImagesRepository.FindBy(c => (c.ContentId == contentId || contentId == 0)
                                                                           && (c.Id == titleId || titleId == 0))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
