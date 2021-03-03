using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APP.CMS.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using APP.UTILS;
using APP.MODELS;
using Microsoft.VisualBasic.CompilerServices;

namespace APP.CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public HomeController(IConfiguration config, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [CustomAuthen("NoCheck")]
        public IActionResult Index()
        {
            var account = UTILS.SessionExtensions.Get<Accounts>(_session, UTILS.SessionExtensions.SessionAccount);
            if (account != null)
            {
                return View();
            }
            return RedirectToAction("dang-nhap", "tai-khoan");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> GetTotalContent()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/contents/get-statistical");
                ViewData["Name"] = "Bài viết";
                return PartialView("TableView", data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetTotalCategory()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/chuyen-muc/get-statistical");
                ViewData["Name"] = "Danh mục";
                return PartialView("TableView", data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetTotalAccount()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/tai-khoan/get-statistical");
                ViewData["Name"] = "Tài khoản";
                return PartialView("TableView", data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetTotalDocument()
        {
            try
            {
                var data = await HttpHelper.GetData<List<LookViewModels>>($"{_domain}/api/document/get-statistical");
                ViewData["Name"] = "Văn bản";
                return PartialView("TableView", data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
