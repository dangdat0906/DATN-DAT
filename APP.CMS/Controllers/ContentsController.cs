using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PageList;
using APP.CMS.Models;
using APP.MODELS;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace APP.CMS.Controllers
{
    [Route("bai-viet")]
    public class ContentsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public ContentsController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> Get_List(string title, int status,long categoryid)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            long createdBy = 0;
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            var account = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            //var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);

            ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;  
            if(currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0
                && (currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0)
                &&(currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0)       
                &&(currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0)
                )
            {
                createdBy = -1;
            }
            else if (currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0
                &&(currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0)
                ) // Tạo mới
            {
                createdBy = account.Id;
                
            }

            else if (currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0) // Phê duyệt
            {
                createdBy = -1;
                
            }
            else if (currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0)
            {
                createdBy = -1;
            }
            try
            {
                var data = await HttpHelper.GetData<List<Contents>>($"{_domain}/api/contents/get-list", $"createdBy={createdBy}&title={title}&status={status}&categoryid={categoryid}");
                return PartialView("_List", data);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpGet("get-list-media")]
        public async Task<IActionResult> Get_List_Media(int status, int type, string folder, int pageNumber, int pageSize = 10)
        {
            try
            {
                var data = await HttpHelper.GetData<List<Medias>>($"{_domain}/api/media/get-list", $"status={status}&type={type}&folder={folder}");
                ViewBag.dataCount = Math.Ceiling((double)data.Count() / pageSize);
                var pagingData = data.ToPagedList(pageNumber, pageSize);
                return PartialView("_ListMedia", pagingData);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("tao-moi")]
        public async Task<IActionResult> Create()
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            ViewBag.Title = "Tạo mới bài viết";
            return View("Create");
        }
        [HttpGet("language-lookup")]
        public async Task<IActionResult> Language_Lookup()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/languages/look-up");
                return Json(new { Result = false, data = data });
            }
            catch (Exception ex)
            {
                

                return Json(new { Message = ex.Message });
            }
        }
        [HttpGet("loai-bai-viet")] 
        public async Task<IActionResult> Choose_ContentType()
        {
            return PartialView("_ChooseContentType");
        }
        [HttpGet("thu-vien-media")]
        public async Task<IActionResult> Media_Gallery()
        {
            return PartialView("_MediaGallery");
        }

        [HttpGet("tao-moi-group")]
        public async Task<IActionResult> CreateGroup()
        {
            ViewBag.Title = "Tạo mới nhóm tin";
            return PartialView("_CreateGroup");
        }
        [HttpGet("open-group")]
        public async Task<IActionResult> OpenGroup()
        {
            return PartialView("_OpenGroup");
        }
        [CustomAuthen]
        [HttpGet("phe-duyet")]
        public async Task<IActionResult> Approve (long id)
        {
            try
            {
                string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
                var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
                var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
                ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
                var data = await HttpHelper.GetData<Contents>($"{_domain}/api/contents/find-by-id", $"id={id}");
                return View("Approve",data);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("sua")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
                var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
                var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
                ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
                var data = await HttpHelper.GetData<Contents>($"{_domain}/api/contents/find-by-id", $"id={id}");
                return View("Edit", data);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [CustomAuthen]
        [HttpGet("cap-nhat")]
        public async Task<IActionResult> Update(long id)
        {
            try
            {
                string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
                var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
                var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
                ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
                ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
                var data = await HttpHelper.GetData<Contents>($"{_domain}/api/contents/find-by-id", $"id={id}");
                return View("Update", data);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(Groups inputModel)
        {
            var account = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            //inputModel.CreatedBy = account.Id;
            try
            {
                if (inputModel.Id == 0)
                {
                    await HttpHelper.PostData<Groups>(inputModel, $"{_domain}/api/contents/create-group");
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công" });
                }
                else
                {
                    throw new Exception($"Thêm mới dữ liệu thất bại");
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpPost("create")]
        public async Task<IActionResult> Create(Contents inputModel)
        {
            var account = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            //var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);
            inputModel.CreatedBy = account.Id;
            //inputModel.LangCode = langcode;
            try
            {
                await HttpHelper.PostData<Contents>(inputModel, $"{_domain}/api/contents/create");
                return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("update-status")]
        public async Task<IActionResult> Update_Status(long id,int status)
        {
            var account = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            try
            {
                var inputModel = await HttpHelper.GetData<Contents>($"{_domain}/api/contents/find-by-id-not-delete", $"id={id}");
                inputModel.UpdatedBy = account.Id;
                inputModel.Status = (byte)status;
                await HttpHelper.PostData<Contents>(inputModel, $"{_domain}/api/contents/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch(Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        
        [CustomAuthen]
        [HttpPost("update")]
        public async Task<IActionResult> Update(Contents inputModel)
        {
            var account = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            //var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);
            inputModel.UpdatedBy = account.Id;
            //inputModel.LangCode = langcode;
            try
            {
                await HttpHelper.PostData<Contents>(inputModel, $"{_domain}/api/contents/update");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("danh-sach")]
        public async Task<IActionResult> Index()
        {
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower() == path.ToLower()).ToList();
            ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            ViewBag.Title = "Danh sách bài viết";
            var values = Enum.GetValues(typeof(ContentStatusEnum));
            foreach (var item in values)
            {
                var a = Extensions.GetEnumDescription((ContentStatusEnum)item);
            }
            return View();
        }
        [HttpGet("get-list-content-group")]
        public async Task<IActionResult> Get_List_Content_Group(long contentId) //Get list of Content_Group by contentId
        {
            try
            {
                var values = await HttpHelper.GetData<List<Content_Groups>>($"{_domain}/api/contents/get-list-content-group/", $"contentId={contentId}");
                List<long> data = new List<long>();
                foreach (var item in values)
                {
                    data.Add(item.GroupId);
                }
                return Json(new { Result = false, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpGet("get-list-content-category")]
        public async Task<IActionResult> Get_List_Content_Category(long contentId)//Get list of Content_Category by contentId
        {
            try
            {
                var values = await HttpHelper.GetData<List<Content_Categories>>($"{_domain}/api/contents/get-list-content-category/", $"contentId={contentId}");
                List<long> data = new List<long>();
                foreach (var item in values)
                {
                    data.Add(item.CategoryId);
                }
                return Json(new { Result = false, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpGet("status-lookup")]
        public async Task<IActionResult> Status_Lookup() //Get list of Content.Status from Enum
        {
            var values = Enum.GetValues(typeof(ContentStatusEnum));
            List<LookupModels> data = new List<LookupModels>();
            foreach (var item in values)
            {
                data.Add(new LookupModels()
                {
                    Title = Extensions.GetEnumDescription((ContentStatusEnum)item),
                    Value = (int)(ContentStatusEnum)item
                });
            }
            return Json(new { Result = false, data = data });
        }
        //[HttpGet("find-by-url")]
        //public async Task<IActionResult> Find_By_Url(string url)
        //{
        //    try
        //    {
        //        var data = await _contentsManager.Find_By_Url(url);
        //        return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        [HttpGet("contenttype-lookup")]
        public async Task<IActionResult> ContentType_Lookup() //Get list of Content.ContentType from Enum
        {
            var data = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/the-loai/look-up");      
            return Json(new { Result = false, data = data });
        }
        [HttpGet("group-lookup")]
        public async Task<IActionResult> Group_Lookup() //Get list of Group from api/group/look-up
        {
            try
            {
                //var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);
                var data = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/contents/group-lookup");
                return Json(new { Result = false, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpGet("news-sources")]
        public async Task<IActionResult> News_Sources() 
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/nguon-tin/look-up");
                return Json(new { Result = false, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpGet("category-getlistchild")]
        public async Task<IActionResult> Category_Lookup() //Get list of Category from api/category/get-list-child
        {
            try
            {
                
                //var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);
                var data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-child");
                var result = data./*Where(c => c.LangCode == langcode).*/OrderBy(c => c.DisplayOrder).ToList();
                
                return Json(new { Result = false, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpGet("show-title-image")]
        public async Task<IActionResult> Show_Title_Image()
        {
            return PartialView("_ShowTitleImage");
        }
        [CustomAuthen]
        [HttpPost("delete-or-restore")]
        public async Task<IActionResult> Delete(Contents inputmodel)
        {
            var account = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            inputmodel.CreatedBy = account.Id;
            try
            {
                if (inputmodel.Status == (byte)StatusEnum.Removed)
                {
                    await HttpHelper.PostData<Contents>(inputmodel, $"{_domain}/api/contents/delete");
                    return Json(new { Result = true });
                }
                else
                {
                    await HttpHelper.PostData<Contents>(inputmodel, $"{_domain}/api/contents/update-status");
                    return Json(new { Result = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
    }
}