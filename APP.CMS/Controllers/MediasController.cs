using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using X.PagedList;
using APP.CMS.Models;
using APP.MODELS;
using APP.UTILS;

namespace APP.CMS.Controllers
{
    [Route("da-phuong-tien")]
    public class MediasController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        //private readonly IFileProvider _fileProvider;
        private readonly string _domain;
        public MediasController(IConfiguration config, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._domain = _config["APIDomain"].ToString();
            this._hostingEnvironment = hostingEnvironment;
            this._httpContextAccessor = httpContextAccessor;
        }
        [CustomAuthen]
        [HttpGet("get-list")]
        public async Task<IActionResult> Get_List(int status,int type,string folder, int pageNumber, int pageSize = 40)
        {
            try
            {
                var data = await HttpHelper.GetData<List<Medias>>($"{_domain}/api/media/get-list", $"status={status}&type={type}&folder={folder}");
                ViewBag.dataCount = Math.Ceiling((double)data.Count() / pageSize);
                var pagingData = data.ToPagedList(pageNumber, pageSize);
                return PartialView("_List", pagingData);
            }
            catch(Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }   
        }
        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("createImg")]
        public async Task<IActionResult> CreateImg()
        {
            return PartialView("_CreateImg");
        }
        [CustomAuthen(nameof(RolesEnum.Create))]
        [HttpGet("createVideo")]
        public async Task<IActionResult> CreateVideo()
        {
            return PartialView("_CreateVideo");
        }
        private string GetPathAndFilename(string filename,string folder,string typeFolder)
        {
            //string currentDate = DateTime.Now.Month.ToString("MMyyyy");
            folder = $"{this._hostingEnvironment.WebRootPath}/{_config["Medias"].ToString()}/{folder}/{Extensions.NormalVNese(typeFolder.ToLower())}";
            var filePath = $"{folder}/{filename}";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return filePath;
        }
        //[CustomAuthen]
        [HttpPost("create-or-update")]
        /**/
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create_Or_Update(IList<IFormFile> files,string obj)
        {
            try
            {
                Medias myObj = JsonConvert.DeserializeObject<Medias>(obj);
                Medias inputmodel = new Medias()
                {
                    Status = (int)StatusEnum.Active,
                    Url = "",
                    Type = myObj.Type,
                    Size = 0,
                    VideoType = myObj.VideoType,
                    Folder = myObj.Folder
                };
                string host = this._hostingEnvironment.WebRootPath;
                if(myObj.Type == (int)MediaTypeEnum.Image) //Image
                {
                    int imgWidth = 0;
                    int imgHeight = 0;
                    foreach (IFormFile source in files)
                    {
                        if (source.Length > 11534336)
                        {
                            throw new Exception("Dung lượng file quá 11 MB");
                        }
                        string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                        filename = $"{Guid.NewGuid().ToString()}{Path.GetExtension(filename)}";
                        using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename,myObj.Folder,Extensions.GetEnumDescription(MediaTypeEnum.Image))))
                            await source.CopyToAsync(output);
                        Bitmap imageFile = new Bitmap(GetPathAndFilename(filename, myObj.Folder, Extensions.GetEnumDescription(MediaTypeEnum.Image)));
                        imgWidth = imageFile.Width;
                        imgHeight = imageFile.Height;
                        inputmodel.Size = source.Length / 1024;
                        inputmodel.Url = $"{GetPathAndFilename(filename, myObj.Folder, Extensions.GetEnumDescription(MediaTypeEnum.Image))}".Replace(host, "");
                    }
                    inputmodel.Folder = inputmodel.Folder.Trim();
                    var data = await HttpHelper.PostData<Medias>(inputmodel, $"{_domain}/api/media/create");
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công", url = data.Url, width = imgWidth, height = imgHeight });
                }
                else //Video
                {
                    if(files.Count < 1)
                    {
                        inputmodel.Url = myObj.Url;
                    }
                    else
                    {
                        foreach (IFormFile source in files)
                        {
                            string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                            filename = $"{Guid.NewGuid().ToString()}{Path.GetExtension(filename)}";
                            using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename, myObj.Folder, Extensions.GetEnumDescription(MediaTypeEnum.Video))))
                                await source.CopyToAsync(output);
                            inputmodel.Size = source.Length / 1024;
                            inputmodel.Url = $"{GetPathAndFilename(filename, myObj.Folder, Extensions.GetEnumDescription(MediaTypeEnum.Video))}".Replace(host, "");
                        }
                    }                
                    var data = await HttpHelper.PostData<Medias>(inputmodel, $"{_domain}/api/media/create");
                    return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công", url = data.Url/*, width = imgWidth, height = imgHeight*/ });
                }

                //var data = await HttpHelper.PostData<Medias>(inputmodel, $"{_domain}/api/media/create");
                //return Json(new { Result = true, Message = "Thêm mới dữ liệu thành công", url = data.Url });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [HttpGet("category-getlistchild")]
        public async Task<IActionResult> Category_Lookup() //Get list of Category from api/category/look-up
        {
            try
            {
                var data = await HttpHelper.GetData<List<Categories>>($"{_domain}/api/chuyen-muc/get-list-child");
                return Json(new { Result = false, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
    
        [HttpGet("form-delete")]
        public async Task<IActionResult> Open_Form_Delete()
        {
            return PartialView("_Form_Delete");
        }
        [CustomAuthen(nameof(RolesEnum.Update))]
        [HttpGet("update")]
        public async Task<IActionResult> Update(long id)
        {
            var data = await HttpHelper.GetData<Medias>($"{_domain}/api/media/find-by-id", $"id={id}");
            return PartialView("_Update", data);
        }
        [CustomAuthen]
        [HttpGet("danh-sach")]
        public IActionResult Index()
        {
            var permission = UTILS.SessionExtensions.Get<List<Role_Permissions>>(_session, UTILS.SessionExtensions.SesscionPermission); // Lấy quyền từ sesion
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var currentPagePermission = permission.Where(c => c.MenuUrl.ToLower() == path.ToLower()).ToList();
            ViewData[nameof(RolesEnum.Create)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Create))) > 0 ? 1 : 0; //lấy quyền của user gắn vào viewbag
            ViewData[nameof(RolesEnum.Update)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Update))) > 0 ? 1 : 0;
            ViewData[nameof(RolesEnum.Delete)] = currentPagePermission.Count(c => c.ActionCode == (nameof(RolesEnum.Delete))) > 0 ? 1 : 0;
            ViewBag.Title = "Thư viện đa phương tiện";
            return View();
        }
        [CustomAuthen(nameof(RolesEnum.Delete))]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(List<long> listId)
        {
            try
            {
                foreach (var item in listId)
                {
                    var result = await HttpHelper.GetData<Medias>($"{_domain}/api/media/find-by-id",$"id={item}");
                    if(result.VideoType == false || result.VideoType == null)
                    System.IO.File.Delete($"{this._hostingEnvironment.WebRootPath}/{result.Url}");
                }
                await HttpHelper.PostData<List<long>>(listId, $"{_domain}/api/media/delete");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        [CustomAuthen]
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus(Medias inputModel)
        {
            try
            {
                await HttpHelper.PostData<Medias>(inputModel, $"{_domain}/api/media/update-status");
                return Json(new { Result = true, Message = "Cập nhật dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
    }
}