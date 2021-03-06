﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.MODELS;
using APP.MODELS.ViewModels;
using APP.UTILS;

namespace APP.CMS.Controllers
{
    [Route("thong-ke-nhuan-but")]
    public class ThongKeNhuanButController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public ThongKeNhuanButController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
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
            return View();
        }
        [CustomAuthen]
        [HttpGet("get-list")]
        public async Task<IActionResult> Get_List(string month)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var data = await HttpHelper.GetData<List<ThongKeNhuanButViewModel>>($"{_domain}/api/thong-ke-nhuan-but/get-list", $"month={month}");
            var listType = await HttpHelper.GetData<List<TheLoai_HeSo>>($"{_domain}/api/thong-ke-nhuan-but/get-list-type",$"month={month}");
            ViewData["ListType"] = listType;
            decimal tongTien = 0;
            decimal tongNhuanBut = 0;
            foreach (var item in data)
            {
                tongTien += item.Tongtien;
                tongNhuanBut += item.NhuanBut;
            }
            ViewData["TongTien"] = tongTien;
            ViewData["TongNhuanBut"] = tongNhuanBut;
            return PartialView("_List", data);
        }
        [CustomAuthen]
        [HttpGet("chi-tiet")]
        public async Task<IActionResult> Detail (long authorId,string month)
        {
            string controllerName = this.ControllerContext.ActionDescriptor.ControllerTypeInfo.CustomAttributes.FirstOrDefault().ConstructorArguments[0].Value.ToString();
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission);
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower().Contains(controllerName.ToLower())).ToList();
            ViewData[nameof(RolesEnum.Approval)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Approval))) > 0 ? 1 : 0;
            var data = await HttpHelper.GetData<List<ThongKeNhuanButByAuthor>>($"{_domain}/api/thong-ke-nhuan-but/detail", $"authorId={authorId}&month={month}");
            decimal tongTien = 0;
            decimal tongNhuanBut = 0;
            foreach (var item in data)
            {
                tongTien += item.Tongtien;
                tongNhuanBut += item.NhuanBut;
            }
            
            var author = await HttpHelper.GetData<Authors>($"{_domain}/api/tac-gia/find-by-id", $"id={authorId}");
            ViewData["author"] = author;
            ViewData["TongTien"] = tongTien;
            ViewData["TongNhuanBut"] = tongNhuanBut;
            return View(data);
        }
        [CustomAuthen]
        [HttpPost("export-word")]
        public async Task<IActionResult> CreateFileWord(ViewModel input)
        {
            try
            {
                return File(HtmlToWord.HtmlToWordMethod11(input.Html), "application/force-download", input.FileName);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }
    }
}
