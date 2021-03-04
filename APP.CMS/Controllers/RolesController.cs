using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.MODELS;
using APP.UTILS;
namespace APP.CMS.Controllers
{
    [Route("nhom-quyen")]
    public class RolesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public RolesController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
        }
        [CustomAuthen]
        [HttpGet("danh-sach")]
        public IActionResult Index()
        {
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower() == path.ToLower()).ToList();
            ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
            ViewBag.Title = "Danh sách nhóm quyền";
            return View();
        }
        [CustomAuthen]
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name, int status, int pageSize = 0, int pageNumber = 10)
        {
            var data = await HttpHelper.GetData<List<Roles>>($"{_domain}/api/nhom-quyen/get-list", $"name={name}&status={status}");
            return PartialView("_List", data);
        }
        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("them-moi")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Thêm mới nhóm quyền";
            return View("_Create");
        }
        [CustomAuthen(nameof(RolesEnum.Update))]
        [HttpGet("sua")]
        public async Task<IActionResult> Update(long id)
        {
            var data = await HttpHelper.GetData<Roles>($"{_domain}/api/nhom-quyen/find-by-id", $"id={id}");         
            return View("_Update", data);
        }

        [CustomAuthen]
        [HttpPost("create-or-update")]
        public async Task<IActionResult> CreateOrUpdate(Roles inputModel)
        {
            try
            {
                if (inputModel.Id == 0)
                {
                    await HttpHelper.PostData<Roles>(inputModel, $"{_domain}/api/nhom-quyen/create");
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công!" });
                }
                else
                {
                    await HttpHelper.PostData<Roles>(inputModel, $"{_domain}/api/nhom-quyen/update");
                    return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [HttpGet("get-list-child")]
        public async Task<IActionResult> GetListChild()
        {
            try { 
                var data = await HttpHelper.GetData<List<Menus>>($"{_domain}/api/menu/get-list-child");
                 return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpGet("get-permission-by-roleid")]
        public async Task<IActionResult> GetPermission(long roleId)
        {
            try
            {
                var data = await HttpHelper.GetData<List<Role_Permissions>>($"{_domain}/api/nhom-quyen/get-permisstion-by-roleid",$"roleId={roleId}");
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpGet("get-role")]
        public async Task<IActionResult> GetRole()
        {
            var values = Enum.GetValues(typeof(RolesEnum));
            List<LookViewModels> data = new List<LookViewModels>();
            foreach (var item in values)
            {
                data.Add(new LookViewModels()
                {
                    Title = Extensions.GetEnumDescription((RolesEnum)item),
                    Value = item.ToString()
                });
            }
            return Json(new { Result = false, data = data });
        }
       [CustomAuthen(nameof(RolesEnum.Delete))]
        [HttpPost("delete-or-restore")]
        public async Task<IActionResult> Delete(Roles inputmodel)
        {
            try
            {
                if (inputmodel.Status == (byte)StatusEnum.Removed)
                {
                    await HttpHelper.PostData<Roles>(inputmodel, $"{_domain}/api/nhom-quyen/delete");
                    return Json(new { Result = true });
                }
                else
                {
                    await HttpHelper.PostData<Roles>(inputmodel, $"{_domain}/api/nhom-quyen/update-status");
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
        public async Task<IActionResult> UpdateStatus(Roles inputModel)
        {
            try
            {
                await HttpHelper.PostData<Roles>(inputModel, $"{_domain}/api/nhom-quyen/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
    }
}
