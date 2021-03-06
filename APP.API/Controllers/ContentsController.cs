using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.MANAGER;
using APP.MODELS;
using APP.UTILS;

namespace Portal.API.Controllers
{
    [Route("api/contents")]
    [ApiController]
    public class ContentsController : Controller
    {
        private readonly IContentsManager _contentsManager;
        //private readonly IContent_LogsManager _contentLogManager;
        private readonly IContent_GroupsManager _contentGroupsManager;
        private readonly IGroupsManager _groupsManager;
        private readonly IContent_CategoriesManager _contentCategoriesManager;
        private readonly ICategoryManager _categoryManager;
        private readonly ITitleImagesManager _titleImagesManager;
        private readonly ITypesManager _typeManager;
        private readonly IConfiguration _config;
        public ContentsController(IContentsManager contents, /*IContent_LogsManager contentLogManager,*/
            IContent_GroupsManager contentGroupsManager, IContent_CategoriesManager contentCategoriesManager, ICategoryManager categoryManager
            , IGroupsManager groupsManager, ITitleImagesManager titleImagesManager, ITypesManager typeManager, IConfiguration config)
        {
            this._config = config;
            this._contentsManager = contents;
            //this._contentLogManager = contentLogManager;
            this._contentGroupsManager = contentGroupsManager;
            this._groupsManager = groupsManager;
            this._contentCategoriesManager = contentCategoriesManager;
            //this._titleImagesManager = titleImagesManager;
            this._typeManager = typeManager;
            this._categoryManager = categoryManager;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Contents inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Title))
                {
                    throw new Exception($"Tên nội dung {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputModel.Title.Length > 250)
                {
                    throw new Exception($"Tên nội dung {MessageConst.LENGTH_ERROR}");
                }
                var exist = await _contentsManager.Find_By_Title(inputModel.Title);
                if (exist != null)
                {
                    throw new Exception($"Tên nội dung {MessageConst.EXIST}");
                }
                inputModel.CreatedDate = DateTime.Now;
                inputModel.Content = inputModel.Content == null? inputModel.Content.Replace($"{_config["CMSDomain"].ToString()}", $"{_config["WebsiteDomain"].ToString()}"): inputModel.Content;
                inputModel.ContentType = inputModel.ContentType == null ? 1 : inputModel.ContentType;
                var dataCreate = await _contentsManager.Create(inputModel);
               /* await Create_Log(dataCreate); */// create Content_Log
                await Create_TitleImage(inputModel.Id, inputModel.TitleImage, inputModel.TitleImgWidth, inputModel.TitleImgHeight); //create TitleImage
                if (inputModel.GroupID != null)
                {
                    await Create_Content_Group(inputModel.GroupID, dataCreate.Id); // create Content_Group
                }
                if (inputModel.CategoryId != null)
                {
                    await Create_Content_Category(inputModel.CategoryId, dataCreate.Id); // create Content_Category
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private async Task Create_TitleImage(long contentId, string titleImg, int? titleImgWidth, int? titleImgHeight)
        {
            try
            {
                var data = new TitleImages();
                data.ContentId = contentId;
                data.Url = titleImg;
                data.Width = titleImgWidth;
                data.Height = titleImgHeight;
                await _titleImagesManager.Create(data);
            }
            catch (Exception ex)
            {
                StatusCode(500, ex.Message);
            }
        }

        private async Task Create_Content_Group(List<long> groupId, long contentId)
        {
            try
            {
                await _contentGroupsManager.Delete(contentId);
                foreach (var item in groupId)
                {
                    var data = new Content_Groups();
                    data.ContentId = contentId;
                    data.GroupId = item;
                    await _contentGroupsManager.Create(data);
                }
            }
            catch (Exception ex)
            {
                StatusCode(500, ex.Message);
            }
        }
        private async Task Create_Content_Category(List<long> categoryId, long contentId)
        {
            try
            {
                await _contentCategoriesManager.Delete(contentId);
                categoryId = categoryId.Distinct().ToList();
                foreach (var item in categoryId)
                {
                    var data = new Content_Categories();
                    data.ContentId = contentId;
                    data.CategoryId = item;
                    await _contentCategoriesManager.Create(data);
                }
            }
            catch (Exception ex)
            {
                StatusCode(500, ex.Message);
            }
        }
        //private async Task Create_Log(Contents inputModel)
        //{
        //    try
        //    {
        //        //DateTime createdDate;
        //        long createdBy;
        //        if (inputModel.Status == (int)ContentStatusEnum.Approving)
        //        {
        //            //createdDate = (DateTime)inputModel.CreatedDate;
        //            createdBy = inputModel.CreatedBy.HasValue ? inputModel.CreatedBy.Value : 0;
        //        }
        //        else
        //        {
        //            //cho phep null ==> dung hasvalue
        //            //createdDate = inputModel.UpdateDate.Value;
        //            createdBy = inputModel.UpdatedBy.HasValue ? inputModel.UpdatedBy.Value : 0;
        //        }
        //        Content_Logs model = new Content_Logs()
        //        {
        //            Title = inputModel.Title,
        //            Url = inputModel.Url,
        //            Summary = inputModel.Summary,
        //            LangCode = inputModel.LangCode,
        //            Content = inputModel.Content,
        //            Source = inputModel.Source,
        //            Status = inputModel.Status,
        //            PublishDate = inputModel.PublishDate,
        //            CreatedDate = DateTime.Now,
        //            CreatedBy = createdBy,
        //            TitleImage = inputModel.TitleImage,
        //            ShowOnTop = inputModel.ShowOnTop,
        //            ShowOnRightTop = inputModel.ShowOnRightTop,
        //            ContentType = inputModel.ContentType,
        //            ContentID = inputModel.Id,
        //            RemovedDate = inputModel.RemovedDate,
        //            AuthorId = inputModel.AuthorId

        //        };
        //        await _contentLogManager.Create(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusCode(500, ex.Message);
        //    }
        //}
        [HttpPost("create-group")]
        public async Task<IActionResult> Create_Group([FromBody] Groups inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Tên nhóm tin {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputModel.Name.Length > 50)
                {
                    throw new Exception($"Tên nhóm tin {MessageConst.LENGTH_ERROR}");
                }
                if (Validation.HasSpecialChar(inputModel.Name))
                {
                    throw new Exception($"Tên nhóm tin {MessageConst.SPECIAL_CHAR}");
                }
                var exist = await _groupsManager.Find_By_Name(inputModel.Name);
                if (exist != null)
                {
                    throw new Exception($"Tên nhóm tin {MessageConst.EXIST}");
                }
                inputModel.CreatedDate = DateTime.Now;
                await _groupsManager.Create(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Contents inputModel)
        {
            try
            {
                var data = await _contentsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    inputModel.Id = 0;
                    await Create(inputModel);
                }
                if (string.IsNullOrEmpty(inputModel.Title))
                {
                    throw new Exception($"Tên nội dung {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputModel.Title.Length > 250)
                {
                    throw new Exception($"Tên nội dung {MessageConst.LENGTH_ERROR}");
                }
                data.Title = inputModel.Title;
                data.Url = inputModel.Url;
                data.Summary = inputModel.Summary;
                data.LangCode = inputModel.LangCode;
                inputModel.Content = inputModel.Content.Replace($"{_config["CMSDomain"].ToString()}", $"{_config["WebsiteDomain"].ToString()}");
                data.Content = inputModel.Content;
                data.Source = inputModel.Source;
                data.Status = inputModel.Status;
                data.PublishDate = inputModel.PublishDate;
                data.UpdateDate = DateTime.Now;
                data.UpdatedBy = inputModel.UpdatedBy;
                data.TitleImage = inputModel.TitleImage;
                //data.TotalView = ??
                data.ShowOnTop = inputModel.ShowOnTop;
                data.ShowOnRightTop = inputModel.ShowOnRightTop;
                data.NewsSource = inputModel.NewsSource;
                data.AuthorId = inputModel.AuthorId;
                inputModel.ContentType = inputModel.ContentType == null ? 1 : inputModel.ContentType;
                data.ContentType = inputModel.ContentType;
                await _contentsManager.Update(data);
                inputModel.CreatedBy = data.CreatedBy;
                //await Create_Log(inputModel); //create Content_Log 
                await Update_TitleImage(inputModel.Id, inputModel.TitleImage, inputModel.TitleImgWidth, inputModel.TitleImgHeight);
                if (inputModel.GroupID != null)
                {
                    await Create_Content_Group(inputModel.GroupID, inputModel.Id); // update Content_Group
                }
                if (inputModel.CategoryId != null)
                {
                    await Create_Content_Category(inputModel.CategoryId, inputModel.Id); //update Content_Category
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private async Task Update_TitleImage(long contentId, string titleImg, int? titleImgWidth, int? titleImgHeight)
        {
            try
            {
                var data = new TitleImages();
                data.ContentId = contentId;
                data.Url = titleImg;
                data.Width = titleImgWidth;
                data.Height = titleImgHeight;
                await _titleImagesManager.Update(data);
            }
            catch (Exception ex)
            {
                StatusCode(500, ex.Message);
            }
        }
        //private void Update_Content_Group(List<long> groupId, long contentId)
        //{
        //    _contentGroupsManager.Delete(contentId);
        //    //var exist = null;
        //    foreach (var item in groupId)
        //    {
        //            var data = new Content_Groups();
        //            data.ContentId = contentId;
        //            data.GroupId = item;
        //            _contentGroupsManager.Create(data);            
        //    }
        //}

        [HttpGet("group-lookup")]
        public async Task<IActionResult> Group_LookUp()
        {
            try
            {
                var data = await _groupsManager.Look_up();
                return Ok(data.Select(c => new { Value = c.Id, Title = c.Name }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-content-group")]
        public async Task<IActionResult> Get_List_Content_Group(long contentId, long groupId)
        {
            try
            {
                var data = await _contentGroupsManager.Get_List(contentId, groupId);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-content-category")]
        public async Task<IActionResult> Get_List_Content_Category(long contentId, long categoryId)
        {
            try
            {
                var data = await _contentCategoriesManager.Get_List(contentId, categoryId);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet("look-up")]
        public async Task<IActionResult> Look_Up()
        {
            try
            {
                var data = await _contentsManager.Look_up();
                return Ok(data.Select(c => new { Value = c.Id, Text = c.Title }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-id")]
        public async Task<IActionResult> Find_By_ID(long id)
        {
            try
            {
                var data = await _contentsManager.Find_By_Id(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-id-not-delete")]
        public async Task<IActionResult> Find_By_ID_Not_Deltete(long id)
        {
            try
            {
                var data = await _contentsManager.Find_By_IdDeleted(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-url")]
        public async Task<IActionResult> Find_By_Url(string url,string langCode = "VIE")
        {
            try
            {
                var data = await _contentsManager.Find_By_Url(url,langCode);
                return Ok(data);
            }
            catch (Exception ex)
            {
                if (ex.Message == "404")
                {
                    return NotFound("Không tìm thấy bản tin");
                }
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-group-by-id")]
        public async Task<IActionResult> Find_Group_byID(long id)
        {
            try
            {
                var data = await _contentsManager.Find_Group_By_Id(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-group-by-contentid")]
        public async Task<IActionResult> GetGroupByContent(long contentId)
        {
            try
            {
                var data = await _contentsManager.GetGroupsByContent(contentId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-name")]
        public async Task<IActionResult> Find_By_Name(string inputName)
        {
            try
            {
                if (string.IsNullOrEmpty(inputName))
                {
                    throw new Exception($"Tên nội dung {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputName.Length > 50)
                {
                    throw new Exception($"Tên nội dung {MessageConst.LENGTH_ERROR}");
                }
                if (Validation.HasSpecialChar(inputName))
                {
                    throw new Exception($"Tên nội dung {MessageConst.SPECIAL_CHAR}");
                }
                var data = await _contentsManager.Find_By_Title(inputName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(long createdBy, string title, long categoryId = 0, int status = -1,string langcode = "", int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = await _contentsManager.Get_List(createdBy, title, categoryId, status, langcode, pageSize, pageNumber);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("get-top-news-by-category")]
        public async Task<IActionResult> GetTopNews(string categoryUrl, int contentNumber)
        {
            try
            {
                var data = await _contentsManager.GetTopNewsByCategoryId(categoryUrl, contentNumber);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                if (ex.Message == "404")
                {
                    return NotFound("Không tìm thấy bản tin");
                }
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-content-paging-by-categoryid")]
        public async Task<IActionResult> GetContentPaging(string categoryUrl, int pageSize, int pageNumber)
        {
            try
            {
                var data = await _contentsManager.GetContentPagingById(categoryUrl, pageSize, pageNumber);
                var contents = data.contents;
                int totalRow = data.totalRow;
                int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
                var model = new PaginationSet<Contents>()
                {
                    items = data.contents,
                    Page = pageNumber,
                    MaxPage = 4,
                    TotalCount = totalRow,
                    TotalPages = totalPage
                };
                if (contents == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-searching-content")]
        public async Task<IActionResult> Get_Searching_Content(int pageSize, int pageNumber,string txtSearch = "",string langCode = "ENG")
        {
            try
            {
                var data = await _contentsManager.Get_Searching_Content(pageSize,pageNumber,txtSearch, langCode);
                var contents = data.contents;
                int totalRow = data.totalRow;
                int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
                var model = new PaginationSet<Contents>()
                {
                    items = data.contents,
                    Page = pageNumber,
                    MaxPage = 4,
                    TotalCount = totalRow,
                    TotalPages = totalPage
                };
                if (contents == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-content-paging-by-tagid")]
        public async Task<IActionResult> GetContentPagingbyTagid(long tagId, int pageSize, int pageNumber)
        {
            try
            {
                var data = await _contentsManager.GetContentPagingByTag(tagId, pageSize, pageNumber);
                var contents = data.contents;
                int totalRow = data.totalRow;
                int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
                var model = new PaginationSet<Contents>()
                {
                    items = data.contents,
                    Page = pageNumber,
                    MaxPage = 4,
                    TotalCount = totalRow,
                    TotalPages = totalPage
                };
                if (contents == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] Contents inputModel)
        {
            try
            {
                var data = await _contentsManager.Find_By_IdDeleted(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _contentsManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-published")]
        public async Task<IActionResult> GetListPublished(string langCode = "VIE")
        {
            try
            {
                var data = await _contentsManager.Get_List_Published(langCode);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                data = data.OrderByDescending(c => c.PublishDate).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-top-hot")]
        public async Task<IActionResult> Get_List_TopHot(string langCode = "vie",int take = 5)
        {
            try
            {
                var data = await _contentsManager.Get_List_Tophot(langCode,take);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-by-category")]
        public async Task<IActionResult> Get_List_By_Category(long cateID)
        {
            try
            {
                var listContent_Category = await _contentCategoriesManager.Get_List_Content_By_Id(cateID); // danh sach Content_Category theo cateID
                List<Contents> listContent = new List<Contents>();
                if (listContent_Category == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                else
                {
                    foreach (var item in listContent_Category)
                    {
                        if (await _contentsManager.Find_By_Approved_Id(item.ContentId) != null)
                        {
                            listContent.Add(await _contentsManager.Find_By_Approved_Id(item.ContentId));
                        }
                    }
                }
                listContent = listContent.OrderByDescending(c => c.PublishDate).ToList();
                return Ok(listContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-and-childlist-by-category")] // lay danh sach bai viet cua ca category cha va con
        public async Task<IActionResult> Get_List_And_ChildList_By_Category(long cateID)
        {
            try
            {
                List<Contents> listContent = new List<Contents>();
                var listContent_Category = await _contentCategoriesManager.Get_List_Content_By_Id(cateID); // danh sach Content_Category theo cateID
                var childCategory = await _categoryManager.GetChild(cateID); // danh sach childCategory status = active
                if(childCategory != null)
                {
                    foreach(var item in childCategory)
                    {
                        if (await _contentCategoriesManager.Get_List_Content_By_Id(item.Id) != null) // neu trong content_category co childCategoryID
                        {
                            listContent_Category.AddRange(await _contentCategoriesManager.Get_List_Content_By_Id(item.Id)); 
                            //them cac ban ghi co CategoryId = childCategoryID vao listContent_Category
                        }
                        
                    }
                }
                if (listContent_Category == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                else
                {
                    foreach (var item in listContent_Category)
                    {
                        if(await _contentsManager.Find_By_Approved_Id(item.ContentId) != null)
                        {
                            listContent.Add(await _contentsManager.Find_By_Approved_Id(item.ContentId));
                        }
                        
                    }
                }
                listContent = listContent.OrderByDescending(c => c.PublishDate).ToList();
                return Ok(listContent);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] Contents inputModel)
        {
            try
            {
                var data = await _contentsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _contentsManager.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-content-type")]
        public async Task<IActionResult> Get_content_type()
        {
            try
            {
                var data = await _typeManager.Look_up();
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("get-statistical")]
        public async Task<IActionResult> Get_statistical(string langcode)
        {
            try
            {
                var data = await _contentsManager.Getstatistical(langcode);
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}