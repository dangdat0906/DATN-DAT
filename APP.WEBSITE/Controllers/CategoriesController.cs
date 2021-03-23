using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APP.MODELS;
using APP.UTILS;
using APP.WEBSITE.Models;

namespace Portal.Website.Controllers
{
    //[Route("danh-muc")]
    public class CategoriesController : GetMetaAttribute
    {
        private readonly IConfiguration _config;
        private readonly string _domain;
        private readonly string _websiteDomain;
        private int pageSize = 12;
        private int pageSizeRow = 15;
        public CategoriesController(IConfiguration config):base(config)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            this._websiteDomain = _config["WebsiteDomain"].ToString();
        }
       // [HttpGet("chi-tiet")]
        public async Task<IActionResult> Index(string categoryUrl, int page = 1)
        {

            try
            {
                var lang = _config["Website"].ToString();
                if(lang != "VN")
                {
                    return RedirectToAction("Error404", "Error");
                }
                var data = await HttpHelper.GetData<CategoriesViewModel>($"{_domain}/api/contents/get-top-news-by-category", $"categoryUrl={categoryUrl}&contentNumber={5}", "false");

                if (data.ListContents.Count() == 1)
                {
                    //return RedirectToAction("Index", "ContentDetail",new { url = listContent[0].Url.Replace(".html","") });
                    //return Redirect($"ContentDetail/Index?url={listContent[0].Url}");
                    return Redirect($"{_websiteDomain}/{data.ListContents[0].Url}");
                }
                 
                ViewData["Category"] = data;
                ViewData["Page"] = page;
                ViewData["categoryUrl"] = categoryUrl;
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                GetMeta();
                return View(data);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error404", "Error");
            }
        }
       // [HttpGet("get-list-content")]
        public async Task<IActionResult> GetListContent(string categoryUrl, int page = 1)
        { 
                    
            var model = await HttpHelper.GetData<PaginationSet<Contents>>($"{_domain}/api/contents/get-content-paging-by-categoryid", $"categoryUrl={categoryUrl}&pagesize={pageSize}&pagenumber={page}", "false");
            ViewData["categoryUrl"] = categoryUrl;
            ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
            return PartialView("_List",model);
        }
        public async Task<IActionResult> GetListRowContent(string categoryUrl, int page = 1)
        {
             
            var model = await HttpHelper.GetData<PaginationSet<Contents>>($"{_domain}/api/contents/get-content-paging-by-categoryid", $"categoryUrl={categoryUrl}&pagesize={pageSizeRow}&pagenumber={page}", "false");
            ViewData["categoryUrl"] = categoryUrl;
            ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
            return PartialView("_ListRow", model);
        }
        public async Task<IActionResult> GetContentByTag(long id, int page = 1)
        {
            try
            {
                 pageSize = 12;
                var data = await HttpHelper.GetData<PaginationSet<Contents>>($"{_domain}/api/contents/get-content-paging-by-tagid",
                    $"tagid={id}&pagesize={pageSize}&pagenumber={page}", "false");
                ViewData["TagId"] = id;
                GetMeta();
                ViewData["CMSDomain"] = _config["CMSDomain"].ToString();
                return View(data);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error404", "Error");
            }
        }
        public async Task<IActionResult> GetTagById(long id)
        {
            var data = await HttpHelper.GetData<Groups>($"{_domain}/api/contents/find-group-by-id",
                $"id={id}", "false");
            return Json(data);
        }
        //public async Task<IActionResult> GetRss(long categoryId)
        //{
        //    try
        //    {
        //        var feed = new SyndicationFeed("Title", "Description", new Uri("http://localhost"), "RSSUrl",(DateTimeOffset) DateTime.Now);
        //        feed.Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Mitchel Sellers");
        //        var items = new List<SyndicationItem>();
        //        var model = await HttpHelper.GetData<List<Contents>>($"{_domain}/api/contents/get-list-and-childlist-by-category", $"cateid={categoryId}", "false");
        //        var result = model.Where(c => c.LangCode == "VIE");
        //        foreach (var item in result)
        //        {
        //            if(item.Summary != null)
        //            {
        //                var postUrl = this._websiteDomain + "/" + item.Url;
        //                var title = item.Title;
        //                var description = item.Summary.ToString().Trim();
        //                items.Add(new SyndicationItem(title, description, new Uri(postUrl), item.Url, (DateTimeOffset)item.PublishDate));
        //            }    
        //        }

        //        feed.Items = items;
        //        var settings = new XmlWriterSettings
        //        {
        //            Encoding = Encoding.UTF8,
        //            NewLineHandling = NewLineHandling.Entitize,
        //            OmitXmlDeclaration = true,
        //            NewLineOnAttributes = true,
        //            Indent = true
        //        };
        //        using (var stream = new MemoryStream())
        //        {
        //            using (var xmlWriter = XmlWriter.Create(stream, settings))
        //            {
        //                var rssFormatter = new Rss20FeedFormatter(feed, true);
        //                rssFormatter.WriteTo(xmlWriter);
        //                xmlWriter.Flush();
        //            }
        //            return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Error404", "Error");
        //    }
        //}
        //public async Task<IActionResult> Rss(long categoryId)
        //{
        //    GetMeta();
        //    var langCode = _config["LangCodeVN"].ToString();
        //    var data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-parent", "", "false");
        //    var model = data.Where(c => c.LangCode == langCode).ToList();
        //    return View(model);
        //}
        //public async Task<IActionResult> SiteMap()
        //{
        //    //List<Categories> data = new List<Categories>();
        //    //var catOnHome = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-category-onhome-published", "", "false");
        //    //var catMenu = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-category-menu-published", "", "false");
        //    //if (catOnHome != null)
        //    //{
        //    //    data.AddRange(catOnHome);
        //    //}
        //    //GetMeta();
        //    //if (catMenu != null)
        //    //{
        //    //    data.AddRange(catMenu);
        //    //}
        //    //List<Categories> result = data.GroupBy(i=>i.Id).Select(i => i.FirstOrDefault()).OrderBy(c=>c.DisplayOrder).ToList();

        //    List<Categories> data = new List<Categories>();
        //    List<Categories> result = new List<Categories>();
        //    GetMeta();
        //    data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-child", "", "false");
        //    foreach (var item in data)
        //    {
        //        if (item.Status == (int)StatusEnum.Active && item.ParentId == 0)
        //        {
        //            result.Add(item);
        //        }
        //    }
        //    result.Add(new Categories()
        //    {
        //        Code = "hethongvanban",
        //        Name = "Hệ thống văn bản",
        //        DisplayOrder = 55
        //    });
        //    result.Add(new Categories()
        //    {
        //        Code = "rss",
        //        Name = "Rss",
        //        DisplayOrder = 55
        //    });
        //    result.Add(new Categories()
        //    {
        //        Code = "cau-hoi-doc-gia",
        //        Name = "Câu hỏi độc giả",
        //        DisplayOrder = 55
        //    });
        //    result.Add(new Categories()
        //    {
        //        Code = "khaosat",
        //        Name = "Khảo sát",
        //        DisplayOrder = 55
        //    });
        //    result.Add(new Categories()
        //    {
        //        Code = "thuvienanh",
        //        Name = "Thư viện ảnh",
        //        DisplayOrder = 55
        //    });
        //    result.Add(new Categories()
        //    {
        //        Code = "thuvienvideo",
        //        Name = "Thư viện video",
        //        DisplayOrder = 55
        //    });
        //    result = result.OrderBy(c => c.DisplayOrder).ToList();
        //    return View(result);
        //}

    }
}
