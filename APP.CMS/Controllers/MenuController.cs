using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.MODELS;
using APP.UTILS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace APP.CMS.Controllers
{
    [Route("menu")]
    public class MenuController : Controller
    {

        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public MenuController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
        }
        //[CustomAuthen]
        [HttpGet("danh-sach")]
        public async Task<IActionResult> Index()
        {
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission); // Lấy quyền từ sesion
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower() == path.ToLower()).ToList();
            //ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0; //lấy quyền của user gắn vào viewbag
            //ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
            //ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
            ViewBag.Title = "Danh sách menu";
            return View();
        }
        //[CustomAuthen]
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name, long parentId, int status, int pageSize = 0, int pageNumber = 10)
        {
            var data = await HttpHelper.GetData<List<MenuViewModels>>($"{_domain}/api/menu/get-list",$"name={name}&parentId={parentId}&status={status}");
            return PartialView("_List", data);
        }
        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            return PartialView("_Create");
        }
       [CustomAuthen(nameof(RolesEnum.Update))]
        [HttpGet("update")]
        public async Task<IActionResult> Update(long id)
        {
            var data = await HttpHelper.GetData<Menus>($"{_domain}/api/menu/find-by-id", $"id={id}");
            return PartialView("_Update", data);
        }
        //[CustomAuthen]
        [HttpPost("create-or-update")]
        public async Task<IActionResult> CreateOrUpdate(Menus inputModel)
        {
            
         
            try
            {
                if (inputModel.Id == 0)
                {
                    await HttpHelper.PostData<Menus>(inputModel, $"{_domain}/api/menu/create");
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công!" });
                }
                else
                {
                    await HttpHelper.PostData<Menus>(inputModel, $"{_domain}/api/menu/update");
                    return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }       
        [HttpGet("get-parent-id")]
        public async Task<IActionResult> GetParentId()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/menu/look-up");
                data.Insert(0, new LookViewModels() { Value = 0, Title = "Tất cả" });
                return Json(new { Data = data});
            }
            catch(Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [HttpGet("get-list-menu")]
        public async Task<IActionResult> GetListMenu()
        {
            try
            {
                var data = await HttpHelper.GetData<List<Menus>>($"{_domain}/api/menu/get-list-child");               
                return Json(new { Data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        [CustomAuthen(nameof(RolesEnum.Delete))]
        [HttpPost("delete-or-restore")]
        public async Task<IActionResult> Delete(Menus inputmodel)
        {
            try
            {
                if(inputmodel.Status == (byte)StatusEnum.Removed)
                {
                    await HttpHelper.PostData<Menus>(inputmodel, $"{_domain}/api/menu/delete");
                    return Json(new { Result = true });
                }
                else
                {
                    await HttpHelper.PostData<Menus>(inputmodel, $"{_domain}/api/menu/update-status");
                    return Json(new { Result = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        //[CustomAuthen]
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus(Menus inputModel)
        {
            try
            {
                await HttpHelper.PostData<Menus>(inputModel, $"{_domain}/api/menu/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
    }

}
