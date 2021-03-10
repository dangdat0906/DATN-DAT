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
    [Route("api/quan-ly-nhuan-but")]
    [ApiController]
    public class QuanLyDonGiaNhuanButController : ControllerBase
    {
        private readonly IQuanLyDonGiaNhuanButManager _quanLyDonGiaNhuanButManager;
        public QuanLyDonGiaNhuanButController(IQuanLyDonGiaNhuanButManager quanLyDonGiaNhuanButManager)
        {
            this._quanLyDonGiaNhuanButManager = quanLyDonGiaNhuanButManager;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] QuanLyDonGiaNhuanBut inputModel)
        {
            try
            {
                await _quanLyDonGiaNhuanButManager.Create(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] QuanLyDonGiaNhuanBut inputModel)
        {
            try
            {
                var data = await _quanLyDonGiaNhuanButManager.Find_By_Id(inputModel.Id);
                if(data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _quanLyDonGiaNhuanButManager.Update(inputModel);
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
                var data = await _quanLyDonGiaNhuanButManager.Find_By_Id(id);
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
        public async Task<IActionResult> UpdateStatus([FromBody] QuanLyDonGiaNhuanBut inputModel)
        {
            try
            {
                var data = await _quanLyDonGiaNhuanButManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _quanLyDonGiaNhuanButManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] QuanLyDonGiaNhuanBut inputModel)
        {
            try
            {
                var data = await _quanLyDonGiaNhuanButManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _quanLyDonGiaNhuanButManager.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> Get_List()
        {
            try
            {
                var data = await _quanLyDonGiaNhuanButManager.Get_List();
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data.OrderBy(c => c.ToMonth.Month).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
