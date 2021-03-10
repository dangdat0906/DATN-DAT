using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using APP.MODELS;
using APP.UTILS;

namespace APP.CMS.Controllers
{
    [Route("he-so-the-loai")]
    public class TheLoai_HeSoController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public TheLoai_HeSoController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
        }
        [CustomAuthen]
        [HttpGet("danh-sach")]
        public async Task<IActionResult> Index()
        {
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission); // Lấy quyền từ sesion
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower() == path.ToLower()).ToList();
            ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0; //lấy quyền của user gắn vào viewbag
            ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
            ViewBag.Title = "Danh sách hệ số nhuận bút thể loại tin";
            ViewData["listType"] = await HttpHelper.GetData<List<Types>>($"{_domain}/api/theloai-heso/get-list-type");
            return View();
        }
        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("tao-moi")]
        public async Task<IActionResult> Create()
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            ViewData["listType"] = await HttpHelper.GetData<List<Types>>($"{_domain}/api/theloai-heso/get-list-type");
            return PartialView("_Create");
        }
        [CustomAuthen(nameof(RolesEnum.Update))]
        [HttpGet("cap-nhat")]
        public async Task<IActionResult> Update(long id)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var data = await HttpHelper.GetData<TheLoai_HeSo>($"{_domain}/api/theloai-heso/find-by-id", $"id={id}");
            ViewData["listType"] = await HttpHelper.GetData<List<Types>>($"{_domain}/api/theloai-heso/get-list-type");
            return PartialView("_Update", data);
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> Get_List(string month, long typeId, int status)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var data = await HttpHelper.GetData<List<TheLoai_HeSo>>($"{_domain}/api/theloai-heso/get-list",$"month={month}&typeId={typeId}&status={status}");
            return PartialView("_List", data);
        }
        [CustomAuthen]
        [HttpPost("create-or-update")]
        public async Task<IActionResult> Create_Or_Update(string inputObj)
        {
            try
            {
                var inputModel = JsonConvert.DeserializeObject<TheLoai_HeSo>(inputObj);
                if (inputModel.Id == 0)
                {
                    await HttpHelper.PostData<TheLoai_HeSo>(inputModel, $"{_domain}/api/theloai-heso/create");
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công" });
                }
                else
                {
                    await HttpHelper.PostData<TheLoai_HeSo>(inputModel, $"{_domain}/api/theloai-heso/update");
                    return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpPost("delete-or-restore")]
        public async Task<IActionResult> Delete(TheLoai_HeSo inputmodel)
        {
            try
            {
                if (inputmodel.Status == (byte)StatusEnum.Removed)
                {
                    await HttpHelper.PostData<TheLoai_HeSo>(inputmodel, $"{_domain}/api/theloai-heso/delete");
                    return Json(new { Result = true });
                }
                else
                {
                    await HttpHelper.PostData<TheLoai_HeSo>(inputmodel, $"{_domain}/api/theloai-heso/update-status");
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
        public async Task<IActionResult> UpdateStatus(TheLoai_HeSo inputModel)
        {
            try
            {
                await HttpHelper.PostData<TheLoai_HeSo>(inputModel, $"{_domain}/api/theloai-heso/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
    }
}
