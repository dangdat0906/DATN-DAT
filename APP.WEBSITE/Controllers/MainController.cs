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
    [Route("trang-chu")]
    public class MainController : GetMetaAttribute
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        public MainController(IConfiguration config):base(config)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
        }
        [HttpGet("get-languages")]
        public async Task<IActionResult> Get_Languages()
        {
           
            
            return PartialView("_Ngonngu");
        }
        [HttpGet("get-list-categories")]
        public async Task<IActionResult> Get_List_Menu()
        {
            var data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-category-menu-published","", "false");
            // danh sach menu da order by DisplayOrder
            ViewData["APIDomain"] = _domain;
            return PartialView("_Menu",data);
        }
        [HttpGet("get-list-right-banners")]
        public async Task<IActionResult> Get_List_Right_Banners()
        {          
            return PartialView("_RightSideBar");
        }
        [HttpGet("get-list-content")]
        public async Task<IActionResult> Get_TopHot_Content()
        {
            var data = await HttpHelper.GetData<List<Contents>>($"{_domain}/api/contents/get-list-published","","false");
            // danh sach data da orderby theo publishedDate
            ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
            
            return PartialView("_MainContent",data);
        }
        //[HttpGet("get-content-category-column")]
        //public async Task<IActionResult> Get_Content_Category_Column(long cateId)
        //{
        //    try
        //    {
        //        var data = await HttpHelper.GetData<List<Contents>>($"{_domain}/api/contents/get-list-by-category", $"cateID={cateId}", "false");
        //        if(data.Count()==0)
        //        {
        //            throw new Exception(MessageConst.DATA_NOT_FOUND);
        //        }
        //        data = data.OrderByDescending(x => x.PublishDate).ToList();
        //        data = data.Count() > 3 ? data.Take(3).ToList() : data; //lay 4 tin, 1 tin tren 3 tin duoi
        //        ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
        //        return PartialView("_ContentCategoryColumn", data);
        //    }
        //    catch(Exception ex)
        //    {
        //        return Json(new { Result = false, Message = ex.Message });
        //    }

        //}
        [HttpGet("get-content-category-column")]
        public async Task<IActionResult> Get_Content_Category_Column(long cateId)
        {
            try
            {
                var data = await HttpHelper.GetData<List<Contents>>($"{_domain}/api/contents/get-list-by-category", $"cateID={cateId}", "false");
                if (data.Count() == 0)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                data = data.OrderByDescending(x => x.PublishDate).ToList();
                data = data.Count() > 10 ? data.Take(10).ToList() : data; //lay 4 tin, 1 tin tren 3 tin duoi
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                return PartialView("_ContentCategoryColumn", data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }

        }
        [HttpGet("get-content-category-row")]
        public async Task<IActionResult> Get_Content_Category_Row(long cateId)
        {
            try
            {
                var data = await HttpHelper.GetData<List<Contents>>($"{_domain}/api/contents/get-list-and-childlist-by-category", $"cateID={cateId}", "false");
                if(data.Count()==0)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                data = data.OrderByDescending(x => x.PublishDate).ToList();
                data = data.Count() > 5 ? data.Take(5).ToList() : data;
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                return PartialView("_ContentCategoryRow", data);
            }
            catch(Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
            
        }
        [HttpGet("get-content-category-row50")]
        public async Task<IActionResult> Get_Content_Category_Row50 (long cateId)
        {
            try
            {
                var data = await HttpHelper.GetData<List<Contents>>($"{_domain}/api/contents/get-list-and-childlist-by-category", $"cateID={cateId}", "false");
                if (data.Count() == 0)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                data = data.Count() > 4 ? data.Take(4).ToList() : data;
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                return PartialView("_ContentCategoryRow50", data);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        //[HttpGet("get-banner-left")]
        //public async Task<IActionResult> Get_Banner_Left(int position)
        //{
        //    var data = await HttpHelper.GetData<List<Banners>>($"{_domain}/api/banner/get-list-left-published", "", "false");
        //    Banners result = new Banners();
        //    //danh sach banner panel = left order by displayorder
        //    ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
        //    if (position == 1)
        //    {
        //        result = data[0];
        //    }
        //    if(position == 2)
        //    {
        //        result = data[1];
        //    }
            
        //    return PartialView("_LeftBanner", result);
        //}
        [HttpGet("get-footer")]
        public async Task<IActionResult> Get_Footer()
        {
            return PartialView("_Footer");
        }
        [HttpGet("get-footerTop")]
        public async Task<IActionResult> Get_Footer_Top()
        {
            var data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-category-menu-published", "", "false");
            // danh sach menu da order by DisplayOrder
            return PartialView("_FooterTop",data);
        }
        [HttpGet("get-footerBot")]
        public async Task<IActionResult> Get_Footer_Bot()
        {
            var data = await HttpHelper.GetData<Contacts>($"{_domain}/api/contact/get-by-statuswebsite", "", "false");
            return PartialView("_FooterBot",data);
        }
        //[HttpGet("get-footerBotRight")]
        //public async Task<IActionResult> Get_Footer_BotRight()
        //{
        //    var data = await HttpHelper.GetData<List<LinkWebsites>>($"{_domain}/api/lien-ket/get-list-link", "", "false");
        //    return PartialView("_FooterBotRight", data);
        //}
        //[HttpGet("get-phimtailieu")]
        //public async Task<IActionResult> Get_PhimTaiLieu()
        //{
        //    var data = await HttpHelper.GetData<List<Medias>>($"{_domain}/api/media/get-video-by-cateId", $"cateId={24}", "false");
        //    var result = data[0];
        //    ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
        //    return PartialView("_PhimTaiLieu", result);
        //}
        //[HttpGet("get-slide-anh")]
        //public async Task<IActionResult> Get_Image_Slide()
        //{
        //    var data = await HttpHelper.GetData<List<DetailSilde>>($"{_domain}/api/silde/get-list-detail-slide-published","", "false");
        //    ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
        //    return PartialView("_ImgSlide", data);
        //}
        public IActionResult Index()
        {
            GetMeta();
            return View();
        }
        [HttpGet("get-main-category")]
        public async Task<IActionResult> MainCategory()
        {
            var data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-category-onhome-published", "", "false");
            return PartialView(data);
        }

    }
}
