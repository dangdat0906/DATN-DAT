using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.MODELS;
using APP.UTILS;
using APP.WEBSITE.Models;

namespace APP.WEBSITE.Controllers
{
    public class SearchController : GetMetaAttribute
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly string _websiteDomain;
        private int pageSize = 6;
        private int pageSizeRow = 15;
        public SearchController(IConfiguration config) : base(config)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            this._websiteDomain = _config["WebsiteDomain"].ToString();
        }
        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            try
            {
                //var data = await HttpHelper.GetData<CategoriesViewModel>($"{_domain}/api/contents/get-searching-content", $"txtSearch={txtSearch}&contentNumber={5}", "false");
                //ViewData["Category"] = data;
                ViewData["Page"] = page;
                //ViewData["categoryUrl"] = categoryUrl;
                string langCode = _config["LangCodeVN"].ToString();
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                var model = await HttpHelper.GetData<PaginationSet<Contents>>($"{_domain}/api/contents/get-searching-content", $"txtSearch={search}&pagesize={pageSize}&pagenumber={page}&langCode={langCode}", "false");
                ViewData["listContent"] = model;
                ViewData["txtSearch"] = search;
                GetMeta();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error404", "Error");
            }
        }
    }
}
