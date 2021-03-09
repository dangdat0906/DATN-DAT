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
    public interface IContentsManager
    {
        Task<Contents> Create(Contents inputModel);
        Task Update(Contents inputModel);
        Task Delete(long id);
        Task<Contents> Find_By_Id(long id);
        Task<Contents> Find_By_IdDeleted(long id);
        Task<Contents> Find_By_Approved_Id(long id);
        Task<Contents> Find_By_Title(string inputTile);
        Task<Contents> Find_By_Url(string url,string langCode = "VIE");
        Task<List<Contents>> Look_up();
        Task<List<Contents>> Get_List(long createdBy, string title, long categoryId, int status,string langCode, int pageSize, int pageNumber);
        Task<List<Contents>> Get_List_Published(string langCode = "VIE");
        Task<List<Contents>> Get_List_Tophot(string langCode,int take);
        Task<CategoriesViewModel> GetTopNewsByCategoryId(string categoryUrl, int contentNumber);
        Task<(List<Contents> contents, int totalRow)> GetContentPagingById(string categoryUrl, int pageSize, int pageNumber);
        Task<List<Groups>> GetGroupsByContent(long contentId);
        Task<(List<Contents> contents, int totalRow)> GetContentPagingByTag(long tagId, int pageSize, int pageNumber);
        Task<Groups> Find_Group_By_Id(long groupId);
        Task<List<LookViewModels>> Getstatistical(string  langcode);
        Task<(List<Contents> contents, int totalRow)> Get_Searching_Content(int pageSize, int pageNumber, string txtSearch,string langCode);
    }
    public class ContentsManager : IContentsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Contents> _logger;
        public ContentsManager(IUnitOfWork unitOfWork, ILogger<Contents> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<(List<Contents> contents, int totalRow)> Get_Searching_Content(int pageSize, int pageNumber, string txtSearch,string langCode)
        {
            try
            {
                List<Contents> contents = new List<Contents>();
                var data = (await _unitOfWork.ContentsRepository.FindBy(c => (c.Content.Contains(txtSearch) || c.Title.Contains(txtSearch))
                                                                        && c.Status == (byte)ContentStatusEnum.Approved))
                    .OrderByDescending(c=>c.PublishDate).ToList();
                contents = data != null ? data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList() : contents;
                var totalRow = data.Count();
                return (contents: contents, totalRow: totalRow);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<(List<Contents> contents, int totalRow)> GetContentPagingById(string categoryUrl, int pageSize, int pageNumber)
        {
            var category = await _unitOfWork.CategoryRepository.Get(x => x.Code == categoryUrl);
            var childCategory = (await _unitOfWork.CategoryRepository.FindBy(x => x.ParentId == category.Id)).ToList();
            var contentsCategory = (await _unitOfWork.Content_CategoriesRepository.FindBy(x => x.CategoryId == category.Id)).Distinct().ToList();
            if (childCategory !=null)
            {
                foreach(var item in childCategory)
                {
                    var childContentCategory = (await _unitOfWork.Content_CategoriesRepository.FindBy(x =>x.CategoryId == item.Id)).ToList();
                    if(childContentCategory!=null)
                    {
                        contentsCategory.AddRange(childContentCategory);
                    }    
                }    
            }           
            var topNewsContent = new List<Contents>();
            if (contentsCategory != null)
            {
                foreach (var item in contentsCategory)
                {
                    var data = (await _unitOfWork.ContentsRepository.Get(x => (x.Id == item.ContentId) && x.Status == (byte)ContentStatusEnum.Approved));
                    if (data != null)
                    {
                        topNewsContent.Add(data);
                    }
                }
            }
            var listContent = topNewsContent.OrderByDescending(x => x.PublishDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var totalRow = topNewsContent.Count();

            return (contents: listContent, totalRow: totalRow);
        }
        public async Task<(List<Contents> contents, int totalRow)> GetContentPagingByTag(long tagId, int pageSize, int pageNumber)
        {
            var group = await _unitOfWork.GroupsRepository.Get(x => x.Id == tagId);
            if (group == null)
            {
                throw new Exception("404");
            }
            var contentGroup = (await _unitOfWork.Content_GroupsRepository.FindBy(x => x.GroupId == group.Id)).Distinct().ToList();
            var listContent = new List<Contents>();
            if (contentGroup != null)
            {
                foreach (var item in contentGroup)
                {
                    var data = (await _unitOfWork.ContentsRepository.Get(x => (x.Id == item.ContentId) && x.Status == (byte)ContentStatusEnum.Approved));
                    if (data != null)
                    {
                        listContent.Add(data);
                    }
                }
            }
             listContent =  listContent.OrderByDescending(x => x.CreatedDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var totalRow = listContent.Count();

            return (contents: listContent, totalRow: totalRow);
        }
        public async Task<Contents> Create(Contents inputModel)
        {
            try
            {
                var result = await _unitOfWork.ContentsRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Contents inputModel)
        {
            await _unitOfWork.CreateTransaction();
            try
            {
                await _unitOfWork.ContentsRepository.Update(inputModel);
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
            var item = await _unitOfWork.ContentsRepository.Get(c => c.Id == id);
            item.Status = (byte)StatusEnum.Removed;
            await _unitOfWork.ContentsRepository.Update(item);
            await _unitOfWork.SaveChange();
        }

        public async Task<Contents> Find_By_Id(long id)
        {
            return await _unitOfWork.ContentsRepository.Get(c => c.Id == id && c.Status != (int)ContentStatusEnum.Delete);
        }
        public async Task<Contents> Find_By_IdDeleted(long id)
        {
            return await _unitOfWork.ContentsRepository.Get(c => c.Id == id);
        }
        public async Task<Contents> Find_By_Approved_Id(long id)
        {
            return await _unitOfWork.ContentsRepository.Get(c => c.Id == id && c.Status == (int)ContentStatusEnum.Approved);
        }

        public async Task<Contents> Find_By_Title(string inputTitle)
        {
            return await _unitOfWork.ContentsRepository.Get(c => c.Title.ToLower() == inputTitle.Trim().ToLower() && c.Status != (int)ContentStatusEnum.Delete);
        }

        public async Task<List<Contents>> Get_List(long createdBy, string title = "", long categoryId = 0, int status = -1, string langCode="", int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                if (categoryId == 0)
                {

                    List<Contents> data = new List<Contents>();
                    if (createdBy == -1)
                    {
                        data = (await _unitOfWork.ContentsRepository.FindBy(x => (x.Status == status || status == (int)ContentStatusEnum.All) &&
                                                                        (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                        && ((string.IsNullOrEmpty(title) || x.Title.ToLower().Contains(title)))
                                                                        //&& x.LangCode == langCode
                                                                        ))
                                                                        .OrderByDescending(d => d.CreatedDate).ToList();
                    }
                    else
                    {
                        data = (await _unitOfWork.ContentsRepository.FindBy(x => (x.Status == status || status == (int)ContentStatusEnum.All) &&
                                                                        (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                        && ((string.IsNullOrEmpty(title) || x.Title.ToLower().Contains(title))
                                                                        //&& (x.CreatedBy == createdBy) && x.LangCode == langCode
                                                                        )))
                                                                        .OrderByDescending(d => d.CreatedDate).ToList();
                    }
                    data = data/*.Where(x => x.LangCode == langCode)*/.ToList();
                    return data;
                }
                else
                {
                    List<Content_Categories> categoryContens = (await _unitOfWork.Content_CategoriesRepository.FindBy(x => x.CategoryId == categoryId)).ToList();
                    List<Contents> data = new List<Contents>();
                    if (categoryContens != null)
                    {
                        if (createdBy == -1)
                        {
                            data = (await _unitOfWork.ContentsRepository.FindBy(x => (x.Status == status || status == (int)ContentStatusEnum.All) &&
                                                                            (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                            && ((string.IsNullOrEmpty(title) || x.Title.ToLower().Contains(title)))
                                                                            //&& x.LangCode == langCode
                                                                            ))
                                                                            .OrderByDescending(d => d.CreatedDate).ToList();
                        }
                        else
                        {
                            data = (await _unitOfWork.ContentsRepository.FindBy(x => (x.Status == status || status == (int)ContentStatusEnum.All) &&
                                                                            (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                            && ((string.IsNullOrEmpty(title) || x.Title.ToLower().Contains(title))
                                                                            //&& (x.CreatedBy == createdBy) && x.LangCode == langCode
                                                                            )))
                                                                            .OrderByDescending(d => d.CreatedDate).ToList();
                        }
                    }
                    List<Contents> result = new List<Contents>();
                    foreach(var item in data)
                    {
                        var content =  categoryContens.Find(x => x.ContentId == item.Id);
                        if (content != null)
                            result.Add(item);    
                    }    
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Contents>> Get_List_Published(string langCode = "VIE")
        {
            try
            {
                var data = (await _unitOfWork.ContentsRepository.FindBy(x => (x.Status == (int)ContentStatusEnum.Approved)
                                                                             /* && x.LangCode == langCode*/)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Contents>> Get_List_Tophot(string langCode,int take)
        {
            try
            {
                return (await _unitOfWork.ContentsRepository.FindBy(c => c.Status == (int)ContentStatusEnum.Approved
                                                                          //&& c.LangCode.ToLower().Trim().Equals(langCode)
                                                                         && c.ShowOnTop == true
                                                                         )) 
                                                                         
                                                                    .OrderByDescending(c=>c.PublishDate).Take(take).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Contents>> Look_up()
        {
            return (await _unitOfWork.ContentsRepository.GetAll()).ToList();
        }

        public async Task<CategoriesViewModel> GetTopNewsByCategoryId(string categoryUrl, int contentNumber)
        {
            try
            {

                var category = await _unitOfWork.CategoryRepository.Get(x => x.Code == categoryUrl);
                if (category == null)
                {
                    throw new Exception("404");
                }
                var childCategory = (await _unitOfWork.CategoryRepository.FindBy(x => x.ParentId == category.Id)).Distinct().ToList();
                var contentsCategory = (await _unitOfWork.Content_CategoriesRepository.FindBy(x => x.CategoryId == category.Id)).ToList();
                if (childCategory != null)
                {
                    foreach (var item in childCategory)
                    {
                        var childContentCategory = (await _unitOfWork.Content_CategoriesRepository.FindBy(x => x.CategoryId == item.Id)).ToList();
                        if (childContentCategory != null)
                        {
                            contentsCategory.AddRange(childContentCategory);
                        }
                    }
                }
                var topNewsContent = new List<Contents>();
                if (contentsCategory != null)
                {
                    var now = DateTime.Now;
                    foreach (var item in contentsCategory)
                    {
                        var data = (await _unitOfWork.ContentsRepository.Get(x => x.Id == item.ContentId && x.Status == (byte)ContentStatusEnum.Approved));
                        if (data != null)
                        {
                            topNewsContent.Add(data);
                        }
                    }
                }
                var result = topNewsContent.OrderByDescending(x => x.PublishDate).ToList().Take(contentNumber).ToList();

                var categoryParent = await _unitOfWork.CategoryRepository.Get(x => x.Id == category.ParentId);

                var categoryViewModel = new CategoriesViewModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Note = category.Note,
                    Code = category.Code,
                    ParentId = category.ParentId,
                    Status = category.Status,
                    ListContents = result,
                    MenuDisplay = category.MenuDisplay,
                    GroupDisplay = category.GroupDisplay,
                    IsMenu = category.IsMenu,
                    DisplayOrder = category.DisplayOrder,
                    ListContentType = category.ListContentType
                };
                if (categoryParent != null)
                {
                    categoryViewModel.ParentCategory = categoryParent;
                }
                return categoryViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<Contents> Find_By_Url(string url, string langCode = "VIE")
        {
            var content = await _unitOfWork.ContentsRepository.Get(c => c.Url == url /*&& c.LangCode == langCode */&& c.Status == (int)ContentStatusEnum.Approved);
            if(content == null)
            {
                throw new Exception("404");
            }    
            var contenCategoryId = await _unitOfWork.Content_CategoriesRepository.Get(x => x.ContentId == content.Id);
            var category = new Categories();
            category = await _unitOfWork.CategoryRepository.Get(x => x.Id == contenCategoryId.CategoryId && x.Status == (int)StatusEnum.Active);
            if (category == null)
            {
                throw new Exception("404");
            }
            content.Categories = category;
            if (category.ParentId != 0)
            {
                var categoryParent = await _unitOfWork.CategoryRepository.Get(x => x.Id == category.ParentId && x.Status == (int)StatusEnum.Active);
                content.CategoryParent = categoryParent;
            }
            return content;
        }

        public async Task<List<Groups>> GetGroupsByContent(long contentId)
        {
            try
            {
                var data = (await _unitOfWork.Content_GroupsRepository.FindBy(x => x.ContentId == contentId)).ToList();
                var listGroup = new List<Groups>();
                if(data!=null)
                {
                    foreach(var item in data)
                    {
                        var group = await _unitOfWork.GroupsRepository.Get(x => x.Id == item.GroupId);
                        if(group !=null)
                        {
                            listGroup.Add(group);
                        }    
                    }    
                }
                return listGroup;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Groups> Find_Group_By_Id(long groupId)
        {
            try
            {
                var data = await _unitOfWork.GroupsRepository.Get(x => x.Id == groupId);
                if(data == null)
                {
                    throw new Exception("404");
                }    
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<LookViewModels>> Getstatistical(string title)
        {
            try
            {
                var data = (await _unitOfWork.ContentsRepository.FindBy(x=>x.Title ==title)).ToList();
                
                var listApproved = data.Where(x => x.Status == (byte)ContentStatusEnum.Approved).ToList();
                var lisApproving = data.Where(x => x.Status == (byte)ContentStatusEnum.Approving).ToList();
                var listDelete = data.Where(x => x.Status == (byte)ContentStatusEnum.Delete).ToList();
                List<LookViewModels> list = new List<LookViewModels>();
                LookViewModels approved = new LookViewModels()
                {
                    Value = listApproved.Count,
                    Title = "Đã duyệt"
                };
                LookViewModels approving = new LookViewModels()
                {
                    Value = lisApproving.Count,
                    Title = "Chờ duyệt"
                };
                LookViewModels delete = new LookViewModels()
                {
                    Value = listDelete.Count,
                    Title = "Đã xóa"
                };
                LookViewModels all = new LookViewModels()
                {
                    Value = data.Count,
                    Title = "Tổng số bài viết"
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
    }
}