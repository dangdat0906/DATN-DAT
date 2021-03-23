using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.MODELS;
using APP.UTILS;
using APP.WEBSITE.Models;

namespace APP.WEBSITE.Controllers
{
    //[Route("bai-viet")]
    public class ContentDetailController : GetMetaAttribute
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        public ContentDetailController(IConfiguration config):base(config)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
        }
      //  [HttpGet("chi-tiet")]
        public async Task<IActionResult> Index(string url)
        {
            try
            {
                GetMeta();
                url = url + ".html";
                var result = await HttpHelper.GetData<Contents>($"{_domain}/api/contents/find-by-url", $"url={url}", "false");
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                return View(result);
            }
            catch(Exception ex)
            {
                if(ex.Message == ((int)StatusCodes.Status404NotFound).ToString())
                {
                    return RedirectToAction("Error404","Error");
                }
                return null;
            }
        }
        public async Task<IActionResult> GetListSame(string urlCategory,long contentId)
        {
            try
            {
                int pageSize = 15;
                PaginationSet<Contents> model = await HttpHelper.GetData<PaginationSet<Contents>>($"{_domain}/api/contents/get-content-paging-by-categoryid", $"categoryUrl={urlCategory}&pagesize={pageSize}", "false");
                var data = model.items.ToList();
                List<Contents> result = data.Where(x => x.Id != contentId).OrderByDescending(x => x.PublishDate).Take(10).ToList();
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                return PartialView(result);
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public async Task<IActionResult> GetGroupbyContent( long contentId)
        {

           var model = await HttpHelper.GetData<List<Groups>>($"{_domain}/api/contents/get-group-by-contentid",$"contentId={contentId}", "false");
            ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
            return Json(model);
        }
        [HttpPost]
        public async Task<IActionResult> SendMail(SendMailModel inputModel)
        {
            try
            {
                var body = @" " + inputModel.namesender + @" ( " + inputModel.Form + @" )
Đã gửi cho bạn bài báo với tiêu đề : " + inputModel.Title + @"
Link liên kết: " + inputModel.Link + @" 
Lời nhắn: " + inputModel.body;
                SendEmail.SendEmailTo(inputModel.To, inputModel.Title, body);
                return Json(new { Result = true, Message = "Gửi thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Gửi thất bại" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> SendMail(string title,string link)
        {
            try
            {
                ViewData["titleDetail"] = title;
                ViewData["link"] = link;
                return PartialView();
            }
            catch (Exception ex)
            {
                return Json(new { Message =ex.Message });
            }
        }

    }
}
