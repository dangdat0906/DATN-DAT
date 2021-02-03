using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using APP.MODELS;
using APP.UTILS;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Extensions;
using APP.UTILS;
using APP.MODELS;

namespace APP.CMS.Controllers
{
    [Route("tai-khoan")]
    public class AccountsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public AccountsController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
         
            HttpHelper.SetHttpContextAccessor(_httpContextAccessor);
        }

        [HttpGet("dang-xuat")]
        public async Task<IActionResult> Logout()
        {
            UTILS.SessionExtensions.Set<Accounts>(_session, UTILS.SessionExtensions.SessionAccount, null);
            UTILS.SessionExtensions.Set<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission, null);
            return View("_Login");
        }
        [HttpGet("dang-nhap")]
        public async Task<IActionResult> Login()
        {
            var acc = new Accounts();
            if (Request.Cookies["UserName"] != null && Request.Cookies["Pass"] != null && Request.Cookies["CheckRemember"] != null)
            {
     
                acc.UserName = Request.Cookies["UserName"];
                acc.Password = Request.Cookies["Pass"];
                acc.RememberPass = bool.Parse(Request.Cookies["CheckRemember"]);
                return View("_Login",acc);
            }
            return View("_Login",acc);
        }
        
        [HttpPost("dang-nhap")]
        public async Task<IActionResult> Login(Accounts inputModel)
        {
            try
            {
                var success = true;

                //if (Validate.Check(inputModel.captcharesponse) == false)
                //{
                //    success = false;
                //    return Json(new { Result = false, Message = "Vui lòng nhấn xác nhận" });
                //}
                var account = await HttpHelper.PostData<Accounts>(inputModel, $"{_domain}/api/tai-khoan/dang-nhap", "false");
                if (account != null)
                {
                    if (inputModel.RememberPass.Value == true)
                    {
                        var option = new CookieOptions();
                        option.Expires = new DateTimeOffset(2038, 1, 1, 0, 0, 0, TimeSpan.FromHours(0));
                        Response.Cookies.Append("UserName", inputModel.UserName, option);
                        Response.Cookies.Append("Pass", inputModel.Password, option);
                        Response.Cookies.Append("CheckRemember", inputModel.RememberPass.Value.ToString(), option);
                    }
                    else
                    {
                        if (Request.Cookies["UserName"] != null)
                        {
                            foreach (var cookie in HttpContext.Request.Cookies)
                            {
                                Response.Cookies.Delete(cookie.Key);
                            }
                        }
                    }
                    UTILS.SessionExtensions.Set<Accounts>(_session, UTILS.SessionExtensions.SessionAccount, account);
                    UTILS.SessionExtensions.Set<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission, account.Role_Permissions);
                 
                    return Json(new { Result = true });
                }

                return Json(new { Result = false, Message = "Đăng nhập không thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }


        }
        [CustomAuthen("NoCheck")]
        [HttpGet("doi-mat-khau")]
        public async Task<IActionResult> ChangePass()
        {
            return View("_ChangePass");
        }
        [CustomAuthen("NoCheck")]
        [HttpPost("doi-mat-khau")]
        public async Task<IActionResult> ChangePass(Accounts inputModel)
        {

            try
            {
                Accounts acc = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
                inputModel.UserName = acc.UserName;
                await HttpHelper.PostData<Accounts>(inputModel, $"{_domain}/api/tai-khoan/doi-mat-khau");
                return Json(new { Result = true, Message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Tài khoản hoăc mật khẩu không chính xác" });
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
            ViewBag.Title = "Danh sách tài khoản";
            return View();
        }
        [CustomAuthen]
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string userName, string fullName, int status, int pageSize = 0, int pageNumber = 10)
        {
            var data = await HttpHelper.GetData<List<Accounts>>($"{_domain}/api/tai-khoan/get-list", $"userName={userName}&fullName={fullName}&status={status}");
            return PartialView("_List", data);
        }
        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("them-moi")]
        public async Task<IActionResult> Create()
        {

            return PartialView("_Create");
        }

        [CustomAuthen(nameof(RolesEnum.Update))]
        [HttpGet("sua")]
        public async Task<IActionResult> Update(long id)
        {
            var data = await HttpHelper.GetData<Accounts>($"{_domain}/api/tai-khoan/find-by-id", $"id={id}");
            return PartialView("_Update", data);
        }
        [CustomAuthen]
        [HttpPost("them-moi")]
        public async Task<IActionResult> Create(Accounts inputModel)
        {
            try
            {
                var data = await HttpHelper.PostData<Accounts>(inputModel, $"{_domain}/api/tai-khoan/create");
                return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }

        }
        [CustomAuthen]
        [HttpPost("sua")]
        public async Task<IActionResult> Update(Accounts inputModel)
        {
            try
            {
                await HttpHelper.PostData<Accounts>(inputModel, $"{_domain}/api/tai-khoan/update");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });


            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }

        }
        [CustomAuthen]
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/nhom-quyen/look-up");
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("lay-don-vi")]
        public async Task<IActionResult> LayDonVi()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/don-vi/look-up");
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("lay-chuc-vu")]
        public async Task<IActionResult> LayChucVu()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/chuc-vu/look-up");
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("get-list-thong-ke-bai-viet-theo-tai-khoan")]
        public async Task<IActionResult> GetContentByAccountCreate(string tuNgay = "", string denNgay = "")
        {
            try
            {
                var data = await HttpHelper.GetData<List<Accounts>>($"{_domain}/api/tai-khoan/thong-ke-bai-viet-theo-tai-khoan", $"tungay={tuNgay}&denngay={denNgay}");

                return PartialView("_ListContentByAccountCreated", data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpGet("thong-ke-bai-viet-theo-tai-khoan")]
        public async Task<IActionResult> ListContentByAccount()
        {
            var acc = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            ViewData["user"] = acc.FullName;
            ViewBag.Title = "Thống kê bài viết theo người tạo";
            return View();
        }
        [CustomAuthen]
        [HttpGet("get-roles-by-accountId")]
        public async Task<IActionResult> GetRolesByAccountId(long accountId)
        {
            try
            {
                var data = await HttpHelper.GetData<List<AccountRoles>>($"{_domain}/api/tai-khoan/get-roles-by-acountid", $"accountid={accountId}");
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen(nameof(RolesEnum.Delete))]
        [HttpPost("delete-or-restore")]
        public async Task<IActionResult> Delete(Accounts inputmodel)
        {
            try
            {
                if (inputmodel.Status == (byte)StatusEnum.Removed)
                {
                    await HttpHelper.PostData<Accounts>(inputmodel, $"{_domain}/api/tai-khoan/delete");
                    return Json(new { Result = true });
                }
                else
                {
                    await HttpHelper.PostData<Accounts>(inputmodel, $"{_domain}/api/tai-khoan/update-status");
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
        public async Task<IActionResult> UpdateStatus(Accounts inputModel)
        {
            try
            {
                await HttpHelper.PostData<Accounts>(inputModel, $"{_domain}/api/tai-khoan/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        //[CustomAuthen]
        //[HttpPost("tao-word")]
        //public async Task<IActionResult> CreateFileWord(string html)
        //{
        //    try
        //    {
        //        return File(HtmlToWord.HtmlToWordMethod(html), "application/force-download", "thongkebaiviettheochuyenmuc.doc");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("");
        //    }
        //}

        [HttpGet("quen-mat-khau")]
        public async Task<IActionResult> ForgetPass()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }
        [HttpPost("quen-mat-khau")]
        public async Task<IActionResult> ForgetPass(string email)
        {
            try
            {
                await HttpHelper.GetData<string>($"{_domain}/api/tai-khoan/quen-mat-khau", $"email={email}", "false");
                return Json(new { Result = true, Message = "Mật khẩu mới đã được gửi về email của bạn" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }

    }
}
