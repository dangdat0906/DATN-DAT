using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.UTILS;
using APP.WEBSITE.Models;

namespace APP.WEBSITE.Controllers
{
    public class HomeController : GetMetaAttribute
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly string _domain;
        public HomeController( ILogger<HomeController> logger, IConfiguration config):base(config)
        {
            this._logger = logger;
            this._config = config;
            this._domain = _config["APIDomain"].ToString();      
            UpdateView();
        }

        public async Task<IActionResult> DetailContent()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            GetMeta();
            return View();
        }
        public void UpdateView()
        {
             HttpHelper.GetData<Accounts>($"{_domain}/api/contact/update-view","", "false");
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
        
    }
}
