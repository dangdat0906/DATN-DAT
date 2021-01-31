using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.MODELS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.UTILS;

namespace APP.CMS.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        public UsersController(IConfiguration config)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
        }
        [HttpGet("danh-sach")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name, string code, int status)
        {
            var data = await HttpHelper.GetData<List<Users>>($"{_domain}/api/user/get-list/", $"name={name}&code={code}&status={status}");
            return PartialView("GetList", data);
        }
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            return PartialView("Create");
        }
        
    }
}
