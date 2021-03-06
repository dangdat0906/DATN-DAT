using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APP.MANAGER;
using APP.MODELS;
using APP.UTILS;

namespace APP.API.Controllers
{
    [Route("api/nguon-tin")]
    [ApiController]
    public class NewsSourcesController : ControllerBase
    {
        private readonly INewsSourcesManager _NewsSourcesManager;
        public NewsSourcesController(INewsSourcesManager NewsSources)
        {
            this._NewsSourcesManager = NewsSources;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] NewsSources inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Tên nguồn tin {MessageConst.NOT_EMPTY_INPUT}");
                }
                inputModel.CreatedDate = DateTime.Now;
                await _NewsSourcesManager.Create(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] NewsSources inputModel)
        {
            try
            {
                var data = await _NewsSourcesManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }

                inputModel.CreatedDate = data.CreatedDate;
                inputModel.UpdatedDate = DateTime.Now;
                await _NewsSourcesManager.Update(inputModel);
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
                var data = await _NewsSourcesManager.Look_up();
                return Ok(data.Select(c => new { Value = c.Id, Title = c.Name }));
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
                var data = await _NewsSourcesManager.Find_By_Id(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-name")]
        public async Task<IActionResult> Find_By_Name(string inputName)
        {
            try
            {

                var data = await _NewsSourcesManager.Find_By_Name(inputName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name, int status, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = await _NewsSourcesManager.Get_List(name, status, pageSize, pageNumber);
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
        public async Task<IActionResult> UpdateStatus([FromBody] NewsSources inputModel)
        {
            try
            {
                var data = await _NewsSourcesManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _NewsSourcesManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] NewsSources inputModel)
        {
            try
            {
                var data = await _NewsSourcesManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _NewsSourcesManager.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
