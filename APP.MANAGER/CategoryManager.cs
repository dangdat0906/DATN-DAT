using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace APP.MANAGER
{
    public interface ICategoryManager
    {
        Task<Categories> Create(Categories inputModel);
        Task Update(Categories inputModel);
        Task<List<CategoriesViewModel>> GetList(string name, long parentId, int status, string langcode, int pageSize = 10, int pageNumber = 0);
        Task<Categories> FindById(long id);
        Task<Categories> FindByIdUpdate(long id);
        Task<List<Categories>> LookUp(string langCode = "VIE");
        Task<Categories> FindByName(string name);
        Task<Categories> FindByCode(string code);
        Task Delete(Categories inputModel);
        Task<Categories> CheckDisplayOrder(int displayOrder);
        Task<List<Categories>> GetListChild(string langCode = "VIE");
        Task<List<Categories>> GetChild(long parentId);
        Task Delete(long id);
        Task<List<LookViewModels>> GetStatistical(string langcode);
        Task<List<Categories>> CategoryContentTotal(string tuNgay = "", string denNgay = "");
        Task<List<Categories>> GetListParent(string langCode = "VIE");
        Task<List<Categories>> GetListHomeEng(string langCode = "ENG");
    }
    public class CategoryManager : ICategoryManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryManager> _logger;
        public CategoryManager(IUnitOfWork unitOfWork, ILogger<CategoryManager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Categories> CheckDisplayOrder(int displayOrder)
        {
            return await _unitOfWork.CategoryRepository.Get(x => x.Status == (int)StatusEnum.Active && x.DisplayOrder == displayOrder);
        }

        public async Task<Categories> Create(Categories inputModel)
        {
            try
            {
                var data = await _unitOfWork.CategoryRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(Categories inputModel)
        {
            try
            {
                await _unitOfWork.CategoryRepository.Delete(inputModel);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Categories> FindById(long id)
        {
            return await _unitOfWork.CategoryRepository.Get(x => (x.Id == id) && (x.Status == (int)StatusEnum.Active));
        }
        public async Task<Categories> FindByIdUpdate(long id)
        {
            return await _unitOfWork.CategoryRepository.Get(x => (x.Id == id));
        }
        public async Task<Categories> FindByName(string name)
        {
            return await _unitOfWork.CategoryRepository.Get(x => x.Name.Equals(name));

        }

        public async Task<List<CategoriesViewModel>> GetList(string name = "", long parentId = 0, int status = -1, string langcode = "", int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = (await _unitOfWork.CategoryRepository.FindBy(x => (x.Status == status || status == (int)StatusEnum.All) &&
                                                                     (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                    && (string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name))
                                                                    )).ToList();

                List<CategoriesViewModel> result = new List<CategoriesViewModel>();
                foreach (var item in data)
                {
                    string parentName = "";
                    long ContentCategoryCount = (await _unitOfWork.Content_CategoriesRepository.FindBy(c => c.CategoryId == item.Id)).ToList().Count();
                    var parentItem = data.Find(c => c.Id == item.ParentId);
                    if (parentItem != null)
                    {
                        parentName = parentItem.Name;
                    }
                    result.Add(new CategoriesViewModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Note = item.Note,
                        ParentId = item.ParentId,
                        LangCode = item.LangCode,
                        Status = item.Status,
                        ParentName = parentName,
                        MenuDisplay = item.MenuDisplay,
                        GroupDisplay = item.GroupDisplay,
                        IsMenu = item.IsMenu,
                        DisplayOrder = item.DisplayOrder,
                        DisplayOnHomeType = item.DisplayOnHomeType,
                        ContentCategoryCount = ContentCategoryCount,
                        ListContentType = item.ListContentType

                    });

                }

                var returnResult = result.Where(x => (x.ParentId == parentId || parentId == 0) && x.LangCode.Trim().ToLower() == langcode.Trim().ToLower()).ToList();
                return returnResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Categories>> GetChild(long parentId)
        {
            var ListCate = (await _unitOfWork.CategoryRepository.FindBy(x => x.ParentId == parentId && x.Status == (int)StatusEnum.Active)).ToList();
            if (ListCate.Count > 0)
            {
                foreach (var item in ListCate)
                {
                    item.ListChild = await GetChild(item.Id);
                }
                return ListCate;
            }
            return null;
        }
        public async Task<List<Categories>> GetListChild(string langCode = "VIE")
        {
            try
            {
                var data = (await _unitOfWork.CategoryRepository.FindBy(x => x.ParentId == 0 && x.Status == (int)StatusEnum.Active && x.LangCode == langCode)).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ListChild = await GetChild(item.Id);
                    }
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Categories>> LookUp(string langCode = "VIE")
        {
            try
            {
                return (await _unitOfWork.CategoryRepository.FindBy(x => x.Status == (int)StatusEnum.Active && x.LangCode == langCode)).OrderBy(c => c.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Categories>> GetListHomeEng(string langCode = "ENG")
        {
            return (await _unitOfWork.CategoryRepository.FindBy(c => c.LangCode == langCode && c.OnHome == true && c.Status == (int)StatusEnum.Active)).OrderBy(c => c.DisplayOrder).ToList();
        }
        public async Task Update(Categories inputModel)
        {
            try
            {
                await _unitOfWork.CategoryRepository.Update(inputModel);
                await _unitOfWork.SaveChange();
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
                var data = await _unitOfWork.CategoryRepository.Get(x => x.Id == id);
                data.Status = (byte)StatusEnum.Removed;
                await _unitOfWork.CategoryRepository.Update(data);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Categories> FindByCode(string code)
        {
            return await _unitOfWork.CategoryRepository.Get(x => x.Code.Equals(code));
        }

        public async Task<List<LookViewModels>> GetStatistical(string langcode)
        {
            try
            {
                var data = (await _unitOfWork.CategoryRepository.FindBy(x => x.LangCode == langcode)).ToList();

                var listApproved = data.Where(x => x.Status == (byte)ContentStatusEnum.Approved).ToList();
                var lisApproving = data.Where(x => x.Status == (byte)ContentStatusEnum.Approving).ToList();
                var listDelete = data.Where(x => x.Status == (byte)ContentStatusEnum.Delete).ToList();
                List<LookViewModels> list = new List<LookViewModels>();
                LookViewModels approved = new LookViewModels()
                {
                    Value = listApproved.Count,
                    Title = "Đang hiển thị"
                };
                LookViewModels approving = new LookViewModels()
                {
                    Value = lisApproving.Count,
                    Title = "Không hiển thị"
                };
                LookViewModels delete = new LookViewModels()
                {
                    Value = listDelete.Count,
                    Title = "Đã xóa"
                };
                LookViewModels all = new LookViewModels()
                {
                    Value = data.Count,
                    Title = "Tổng số danh mục"
                };
                list.Add(approved);
                list.Add(approving);
                list.Add(delete);
                list.Add(all);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Categories>> CategoryContentTotal(string tuNgay = "", string denNgay = "")
        {
            try
            {
               
                var data = (await _unitOfWork.CategoryRepository.FindBy(x => x.ParentId == 0 
                && x.Status != (byte)StatusEnum.Removed)).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ListChild = await GetCategoryContentChild(item.Id,tuNgay,denNgay);
                        var listContent = await getListContentByCategoryId(item.Id,tuNgay,denNgay);
                        item.Contents = listContent;
                    }
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string conver(string date)
        {
            var s = date.Split("/");
            string d = s[2] + s[1] + s[0];
            return d;
        }
        public async Task<List<Contents>> getListContentByCategoryId(long categoryId, string tuNgay = "", string denNgay = "")
        {
            DateTime formdate = DateTime.Now.Date;
            DateTime toDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(tuNgay))
            {

                formdate = DateTime.ParseExact(conver(tuNgay), "yyyyMMdd", CultureInfo.InvariantCulture).Date;

                toDate = DateTime.ParseExact(conver(denNgay), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
            }
            var contentsCategory = (await _unitOfWork.Content_CategoriesRepository.FindBy(x => x.CategoryId == categoryId)).ToList();
            var listContents = new List<Contents>();
            if (contentsCategory != null)
            {
                foreach (var item in contentsCategory)
                {
                    var data = (await _unitOfWork.ContentsRepository.Get(x => x.Id == item.ContentId && (string.IsNullOrEmpty(tuNgay) || (x.PublishDate.Value.Date > formdate && x.PublishDate.Value.Date < toDate))));
                    if (data != null)
                    {
                        listContents.Add(data);
                    }
                }
            }
            return listContents;
        }
        public async Task<List<Categories>> GetCategoryContentChild(long parentId, string tuNgay = "", string denNgay = "")
        {
            
            var ListCate = (await _unitOfWork.CategoryRepository.FindBy(x => x.ParentId == parentId && x.Status != (byte)StatusEnum.Removed)).ToList();
            if (ListCate.Count > 0)
            {
                foreach (var item in ListCate)
                {
                    item.ListChild = await GetChild(item.Id);
                    var listContent = await getListContentByCategoryId(item.Id,tuNgay,denNgay);
                    item.Contents = listContent;
                }
                return ListCate;
            }
            return null;
        }

        public async Task<List<Categories>> GetListParent(string langCode = "VIE")
        {
            try
            {
                return (await _unitOfWork.CategoryRepository.FindBy(x => x.Status == (int)StatusEnum.Active && x.ParentId == 0 &&x.LangCode == langCode)).OrderBy(c => c.DisplayOrder ).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
