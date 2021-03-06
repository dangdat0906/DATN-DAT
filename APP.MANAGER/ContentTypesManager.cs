using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using APP.MODELS;
namespace APP.MANAGER
{
    public interface IContentTypesManager
    {
        Task CreateList(long contentId, List<Types> list);
        Task DeleteList(long contentId);
    }
    public class ContentTypesManager : IContentTypesManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TypesManager> _logger;
        public ContentTypesManager(IUnitOfWork unitOfWork, ILogger<TypesManager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task CreateList(long contentId, List<Types> list)
        {
            try
            {
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        var contentTheLoai = new MODELS.ContentTypes();
                        contentTheLoai.ContentId = contentId;
                        contentTheLoai.TheLoaiId = item.Id;
                        await _unitOfWork.ContentTypesRepository.Add(contentTheLoai);
                        await _unitOfWork.SaveChange();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteList(long contentId)
        {
            try
            {
                var data = (await _unitOfWork.ContentTypesRepository.FindBy(x => x.ContentId == contentId)).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        await _unitOfWork.ContentTypesRepository.Delete(item);
                        await _unitOfWork.SaveChange();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
