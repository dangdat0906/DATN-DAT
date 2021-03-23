using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.CMS.Models;
using APP.MODELS;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.CMS.Controllers
{
    [Route("cau-hinh")]
    public class ContactController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public ContactController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            _httpContextAccessor = httpContextAccessor;
        }
        
        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup()
        {
            var data = await HttpHelper.GetData<List<LookupModels>>($"{_domain}/api/contact/look-up");
            return Json(new { Result = false, data = data });
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create_Or_Update(Contacts inputModel)
        {
            try
            {                                               
                    await HttpHelper.PostData<Contacts>(inputModel, $"{_domain}/api/contact/update");
                    return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
                
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [HttpGet("danh-sach")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var langcode = UTILS.SessionExtensions.Get<string>(_session, UTILS.SessionExtensions.SesscionLanguages);
                var data = await HttpHelper.GetData<Contacts>($"{_domain}/api/contact/find-by-langcode/", $"langcode={langcode}");
                if (data == null)
                {
                    return View();
                }
                return View(data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
    }
}

