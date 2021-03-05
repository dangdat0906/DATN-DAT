using Microsoft.Extensions.Logging;
using Portal.Models;
using Portal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Manager
{
    public interface IContent_CategoriesManager
    {
        Task<Content_Categories> Create(Content_Categories inputModel);
        Task Update(Content_Categories inputModel);
        Task Delete(long id);
        Task<Content_Categories> Find_By_Id(long id);
        Task<List<Content_Categories>> Get_List(long contentID, long cateID);
        Task<List<Content_Categories>> Get_List_Content_By_Id(long cateID);
    }
    public class Content_CategoriesManager : IContent_CategoriesManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Content_Categories> _logger;
        public Content_CategoriesManager(IUnitOfWork unitOfWork, ILogger<Content_Categories> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Content_Categories> Create(Content_Categories inputModel)
        {
            try
            {
                var result = await _unitOfWork.Content_CategoriesRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Content_Categories inputModel)
        {
            await _unitOfWork.CreateTransaction();
            try
            {
                await _unitOfWork.Content_CategoriesRepository.Update(inputModel);
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
            var list = (await _unitOfWork.Content_CategoriesRepository.FindBy(c => c.ContentId == id)).ToList();
            foreach(var item in list)
            {
                await _unitOfWork.Content_CategoriesRepository.Delete(item);
            }         
            //await _unitOfWork.Commit();
            await _unitOfWork.SaveChange();
        }

        public async Task<Content_Categories> Find_By_Id(long id)
        {
            return await _unitOfWork.Content_CategoriesRepository.Get(c => c.Id == id);
        }

        public async Task<List<Content_Categories>> Get_List(long contentID, long cateID)
        {
            try
            {
                var data = (await _unitOfWork.Content_CategoriesRepository.FindBy(c => (c.ContentId == contentID || contentID == 0)
                                                                           && (c.CategoryId == cateID || cateID == 0))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Content_Categories>> Get_List_Content_By_Id(long cateID)
        {
            try
            {
                var data = (await _unitOfWork.Content_CategoriesRepository.FindBy(c => (c.CategoryId == cateID)
                                                                           )).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}