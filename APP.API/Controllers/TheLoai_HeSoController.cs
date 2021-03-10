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
    [Route("api/theloai-heso")]
    [ApiController]
    public class TheLoai_HeSoController : ControllerBase
    {
        private readonly ITheLoai_HeSoManager _TheLoai_HeSoManager;
        public TheLoai_HeSoController(ITheLoai_HeSoManager TheLoai_HeSoManager)
        {
            this._TheLoai_HeSoManager = TheLoai_HeSoManager;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] TheLoai_HeSo inputModel)
        {
            try
            {
                await _TheLoai_HeSoManager.Create(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] TheLoai_HeSo inputModel)
        {
            try
            {
                var data = await _TheLoai_HeSoManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _TheLoai_HeSoManager.Update(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-id")]
        public async Task<IActionResult> Find_By_Id(long id)
        {
            try
            {
                var data = await _TheLoai_HeSoManager.Find_By_Id(id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] TheLoai_HeSo inputModel)
        {
            try
            {
                var data = await _TheLoai_HeSoManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _TheLoai_HeSoManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] TheLoai_HeSo inputModel)
        {
            try
            {
                var data = await _TheLoai_HeSoManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _TheLoai_HeSoManager.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> Get_List(string month, long typeId, int status)
        {
            try
            {
                var data = await _TheLoai_HeSoManager.Get_List(month,typeId,status);
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
        [HttpGet("get-list-type")]
        public async Task<IActionResult> Get_List_Types()
        {
            try
            {
                var data = await _TheLoai_HeSoManager.Look_Up_TheLoai();
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
