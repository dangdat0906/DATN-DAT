using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.CMS.Models;
using APP.MODELS;
using APP.UTILS;

namespace APP.CMS.Controllers
{
    [Route("tac-gia")]
    public class AuthorsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public AuthorsController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
        }
        [CustomAuthen]
        [HttpGet("get-list")]
        public async Task<IActionResult> Get_List(string name,long newSourceId, int status)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var data = await HttpHelper.GetData<List<Authors>>($"{_domain}/api/tac-gia/get-list?", $"name={name}&newSourceId={newSourceId}&status={status}");
            return PartialView("_List", data);
        }
        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var listNewsSources = new List<LookupModels>();
            listNewsSources = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/nguon-tin/look-up");
            ViewData["listNewsSources"] = listNewsSources;
            return PartialView("_Create");
        }
        [CustomAuthen(nameof(RolesEnum.Update))]
        [HttpGet("update")]
        public async Task<IActionResult> Update(long id)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var listNewsSources = new List<LookupModels>();
            listNewsSources = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/nguon-tin/look-up");
            ViewData["listNewsSources"] = listNewsSources;
            var data = await HttpHelper.GetData<Authors>($"{_domain}/api/tac-gia/find-by-id", $"id={id}");
            return PartialView("_Update", data);
        }
        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup(long newsSourceId = 0)
        {
            var data = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/tac-gia/look-up", $"newsSourceId={newsSourceId}");
            return Json(new { Result = false, data = data });
        }
        [CustomAuthen]
        [HttpPost("create-or-update")]
        public async Task<IActionResult> Create_Or_Update(Authors inputModel)
        {
            try
            {
                if (inputModel.Id == 0)
                {
                    await HttpHelper.PostData<Authors>(inputModel, $"{_domain}/api/tac-gia/create");
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công" });
                }
                else
                {
                    await HttpHelper.PostData<Authors>(inputModel, $"{_domain}/api/tac-gia/update");
                    return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
                }
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
            var listNewsSources = new List<LookupModels>();
            listNewsSources = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/nguon-tin/look-up");
            ViewData["listNewsSources"] = listNewsSources;
            ViewBag.Title = "Danh sách tác giả";
            return View();
        }
        [CustomAuthen]
        [HttpPost("delete-or-restore")]
        public async Task<IActionResult> Delete(NewsSources inputmodel)
        {
            try
            {
                if (inputmodel.Status == (byte)StatusEnum.Removed)
                {
                    await HttpHelper.PostData<NewsSources>(inputmodel, $"{_domain}/api/tac-gia/delete");
                    return Json(new { Result = true });
                }
                else
                {
                    await HttpHelper.PostData<NewsSources>(inputmodel, $"{_domain}/api/tac-gia/update-status");
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
        public async Task<IActionResult> UpdateStatus(NewsSources inputModel)
        {
            try
            {
                await HttpHelper.PostData<NewsSources>(inputModel, $"{_domain}/api/tac-gia/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
    }
}
