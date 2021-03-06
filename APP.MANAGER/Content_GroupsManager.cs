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
    public interface IContent_GroupsManager
    {
        Task<Content_Groups> Create(Content_Groups inputModel);
        Task Update(Content_Groups inputModel);
        Task Delete(long id);
        Task<Content_Groups> Find_By_Id(long contentId,long groupId);
        Task<List<Content_Groups>> Get_List(long contentID, long groupID);
    }
    public class Content_GroupsManager : IContent_GroupsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Content_Groups> _logger;
        public Content_GroupsManager(IUnitOfWork unitOfWork, ILogger<Content_Groups> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Content_Groups> Create(Content_Groups inputModel)
        {
            try
            {
                var result = await _unitOfWork.Content_GroupsRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Content_Groups inputModel)
        {
            await _unitOfWork.CreateTransaction();
            try
            {
                await _unitOfWork.Content_GroupsRepository.Update(inputModel);
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
            var item = (await _unitOfWork.Content_GroupsRepository.FindBy(c => c.ContentId == id)).ToList();
            await _unitOfWork.Content_GroupsRepository.BulkDelete(item);
            //await _unitOfWork.Commit();
            await _unitOfWork.SaveChange();
        }

        public async Task<Content_Groups> Find_By_Id(long contentId,long groupId)
        {
            return await _unitOfWork.Content_GroupsRepository.Get(c => (c.ContentId == contentId && c.GroupId == groupId));
        }

        public async Task<List<Content_Groups>> Get_List(long contentID = 0, long groupID = 0)
        {
            try
            {
                var data = (await _unitOfWork.Content_GroupsRepository.FindBy(c => (c.ContentId == contentID || contentID == 0)
                                                                           && (c.GroupId == groupID || groupID == 0))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
