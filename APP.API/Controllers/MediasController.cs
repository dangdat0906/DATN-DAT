using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using APP.MANAGER;
using APP.MODELS;
using APP.UTILS;

namespace APP.API.Controllers
{
    [Route("api/media")]
    [ApiController]
    public class MediasController : Controller
    {
        private readonly IMediasManager _mediasManager;
        private readonly ICategoryManager _categoryManager;
        public MediasController(IMediasManager mediasManager, ICategoryManager categoryManager)
        {
            this._mediasManager = mediasManager;
            this._categoryManager = categoryManager;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Medias inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Url))
                {
                    throw new Exception($"Tập tin {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputModel.Url.Length > 500)
                {
                    throw new Exception($"Tập tin {MessageConst.LENGTH_ERROR}");
                }
                //var exist = await _mediasManager.Find_By_Url(inputModel.Url);
                //if (exist != null)
                //{
                //    throw new Exception($"Tập tin {MessageConst.EXIST}");
                //}
                inputModel.CreatedDate = DateTime.Now;
                var data = await _mediasManager.Create(inputModel);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Medias inputModel)
        {
            try
            {
                var data = await _mediasManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                if (string.IsNullOrEmpty(inputModel.Url))
                {
                    throw new Exception($"Tập tin {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputModel.Url.Length > 500)
                {
                    throw new Exception($"Tập tin {MessageConst.LENGTH_ERROR}");
                }
                data.Status = inputModel.Status == -1 ? data.Status : inputModel.Status;
                data.UpdatedDate = DateTime.Now;
                await _mediasManager.Update(data);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-detail-album-media")]
        public async Task<IActionResult> UpdateDetailAlbum([FromBody] Medias inputModel)
        {
            try
            {
                inputModel.UpdatedDate = DateTime.Now;
                await _mediasManager.Update(inputModel);
                return Ok();
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<long> listId)
        {
            try
            {
                foreach (var item in listId)
                {
                    var data = await _mediasManager.Find_By_Id(item);
                    if (data == null)
                    {
                        throw new Exception(MessageConst.DATA_NOT_FOUND);
                    }
                    await _mediasManager.Delete(data);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("look-up")]
        public async Task<IActionResult> Look_Up()
        {
            try
            {
                var data = await _mediasManager.Look_up();
                return Ok(data.Select(c => new { Value = c.Id, Title = c.Url }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-id")]
        public async Task<IActionResult> Find_By_ID(long id)
        {
            try
            {
                var data = await _mediasManager.Find_By_Id(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-url")]
        public async Task<IActionResult> Find_By_Url(string url)
        {
            try
            {
                var data = await _mediasManager.Find_By_Url(url);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet("find-by-name")]
        public async Task<IActionResult> Find_By_Name(string inputName)
        {
            try
            {
                if (string.IsNullOrEmpty(inputName))
                {
                    throw new Exception($"Ảnh {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputName.Length > 50)
                {
                    throw new Exception($"Ảnh {MessageConst.LENGTH_ERROR}");
                }
                var data = await _mediasManager.Find_By_Url(inputName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(int status,int type, string folder, int pageSize = 40, int pageNumber = 1)
        {
            try
            {
                var data = await _mediasManager.Get_List(status,type, folder, pageSize, pageNumber);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] Medias inputModel)
        {
            try
            {
                var data = await _mediasManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _mediasManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //[HttpPost("delete")]
        //public async Task<IActionResult> Delete([FromBody] Medias inputModel)
        //{
        //    try
        //    {
        //        var data = await _mediasManager.Find_By_Id(inputModel.Id);
        //        if (data == null)
        //        {
        //            throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
        //        }
        //        await _mediasManager.Delete(inputModel.Id);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        [HttpGet("get-video-by-cateId")]
        public async Task<IActionResult> Get_Video_By_CateId(long cateId)
        {
            try
            {
                var category = await _categoryManager.FindById(cateId);
                string folder = Extensions.NormalVNese(category.Name);
                var data = await _mediasManager.Get_List((int)StatusEnum.Active, (int)MediaTypeEnum.Video, folder, 40, 1);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}