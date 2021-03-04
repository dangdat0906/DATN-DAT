using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using APP.CMS.Models;
using APP.MODELS;
using APP.UTILS;

namespace APP.CMS.Controllers
{
    [Route("chuyen-muc")]
    public class CategoryController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHostingEnvironment _hostingEnvironment;
        public CategoryController(IConfiguration config, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
            this._hostingEnvironment = hostingEnvironment;
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
            ViewBag.Title = "Danh sách chuyên mục";
            return View();
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name, long parentId, int status, int pageSize = 0, int pageNumber = 10)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var data = await HttpHelper.GetData<List<CategoriesViewModel>>($"{_domain}/api/chuyen-muc/get-list", $"name={name}&parentId={parentId}&status={status}&langcode={langcode}");
            return PartialView("_List", data);
        }


        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("them-moi")]
        public async Task<IActionResult> Create()
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            ViewBag.Title = "Thêm mới chuyên mục";
            return View("_Create");
        }
        [CustomAuthen(nameof(RolesEnum.Update))]
        [HttpGet("sua")]
        public async Task<IActionResult> Update(long id)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            ViewBag.Title = "Chỉnh sửa chuyên mục";
            var data = await HttpHelper.GetData<Categories>($"{_domain}/api/chuyen-muc/find-by-id", $"id={id}");
            return View("_Update", data);
        }
        [CustomAuthen]
        [HttpPost("create-or-update")]
        public async Task<IActionResult> CreateOrUpdate(Categories inputModel, List<ConfigCategories> configs)
        {
            try
            {
                var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);
                if (inputModel.Id == 0)
                {
                    inputModel.Config = configs;
                    inputModel.LangCode = langcode;
                    var data = await HttpHelper.PostData<Categories>(inputModel, $"{_domain}/api/chuyen-muc/create");
                    CreateFolder(data.Code);
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công!" });
                }
                else
                {
                    inputModel.Config = configs;
                    inputModel.LangCode = langcode;
                    var data = await HttpHelper.PostData<Categories>(inputModel, $"{_domain}/api/chuyen-muc/update");
                    //RenameFolder(data.Code, inputModel.Code);
                    return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        public void CreateFolder(string categoryName)
        {

            var folder = $"{this._hostingEnvironment.WebRootPath}/{_config["Medias"].ToString()}/{categoryName}";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        public void RenameFolder(string categoryNameOld, string categoryNameNew)
        {

            var folderOld = $"{this._hostingEnvironment.WebRootPath}/{_config["Medias"].ToString()}/{categoryNameOld}";
            var folderNew = $"{this._hostingEnvironment.WebRootPath}/{_config["Medias"].ToString()}/{categoryNameNew}";
            if (!Directory.Exists(folderNew))
            {
                Directory.Move(folderOld, folderNew);
            }
        }

        [HttpGet("get-parent-id")]
        public async Task<IActionResult> GetParentId()
        {
            try
            {
                var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);
                var data = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/chuyen-muc/look-up",$"langCode={langcode}");

                data.Insert(0, new LookupModels() { Value = 0, Title = "Tất cả" });
                return Json(new { Data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        [HttpGet("create-config")]
        public async Task<IActionResult> CreateConfig()
        {
            return PartialView("_CreateConfig");
        }

        [HttpPost("create-config")]
        public async Task<IActionResult> CreateConfig(ConfigCategories inputModel)
        {
            try
            {
                await HttpHelper.PostData<ConfigCategories>(inputModel, $"{_domain}/api/config-category/create");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }

        }
        [HttpGet("displaytypeLook-lookup")]
        public async Task<IActionResult> DisplayTypeLook_Lookup()
        {
            var values = Enum.GetValues(typeof(ConfigDisplayTypeEnum));
            List<LookupModels> data = new List<LookupModels>();
            foreach (var item in values)
            {
                data.Add(new LookupModels()
                {
                    Title = Extensions.GetEnumDescription((ConfigDisplayTypeEnum)item),
                    Value = (int)(ConfigDisplayTypeEnum)item
                });
            }
            return Json(new { Result = true, data = data });
        }
        [HttpGet("displaytype-lookup")]
        public async Task<IActionResult> DisplayType_Lookup()
        {
            var values = Enum.GetValues(typeof(ConfigTypeEnum));
            List<LookupModels> data = new List<LookupModels>();
            foreach (var item in values)
            {
                data.Add(new LookupModels()
                {
                    Title = Extensions.GetEnumDescription((ConfigTypeEnum)item),
                    Value = (int)(ConfigTypeEnum)item
                });
            }
            return Json(new { Result = false, data = data });
        }
        [HttpGet("position-lookup")]
        public async Task<IActionResult> ConfigPosition_Lookup()
        {
            var values = Enum.GetValues(typeof(ConfigPositionEnum));
            List<LookupModels> data = new List<LookupModels>();
            foreach (var item in values)
            {
                data.Add(new LookupModels()
                {
                    Title = Extensions.GetEnumDescription((ConfigPositionEnum)item),
                    Value = (int)(ConfigPositionEnum)item
                });
            }
            return Json(new { Result = false, data = data });
        }
        [HttpGet("get-config")]
        public async Task<IActionResult> GetConfig(long categoryId)
        {
            try
            {
                var data = await HttpHelper.GetData<List<ConfigCategories>>($"{_domain}/api/config-category/find-by-category-id", $"id={categoryId}");
                return Json(new { Result = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpPost("delete-or-restore")]
        public async Task<IActionResult> Delete(Categories inputmodel)
        {
            try
            {
                if (inputmodel.Status == (byte)StatusEnum.Removed)
                {
                    await HttpHelper.PostData<Categories>(inputmodel, $"{_domain}/api/chuyen-muc/delete");
                    return Json(new { Result = true });
                }
                else
                {
                    await HttpHelper.PostData<Categories>(inputmodel, $"{_domain}/api/chuyen-muc/update-status");
                    return Json(new { Result = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus(Categories inputModel)
        {
            try
            {
                await HttpHelper.PostData<Categories>(inputModel, $"{_domain}/api/chuyen-muc/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("get-list-thong-ke-bai-viet-theo-danh-muc")]
        public async Task<IActionResult> GetCategoryContent(string tuNgay = "", string denNgay = "")
        {
            try
            {
                var data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/thong-ke-bai-viet-theo-danh-muc",$"tungay={tuNgay}&denngay={denNgay}");
                if (data == null)
                {
                    return View("Error401", "error");
                }    
                

                return PartialView("_ListCategoryContent",data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("thong-ke-bai-viet-theo-danh-muc")]
        public async Task<IActionResult> CategoryContent(string tuNgay = "", string denNgay = "")
        {
            ViewBag.Title = "Thống kê bài viết theo chuyên mục";
            var acc = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            ViewData["user"] = acc.FullName;
            return View();
        }

        [CustomAuthen]
        [HttpPost("tao-word")]
        public async Task<IActionResult> CreateFileWord(string html)
        {
            try
            {
                return File(HtmlToWord.HtmlToWordMethod(html), "application/force-download", "thongkebaiviettheochuyenmuc.doc");
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }
    }
}
