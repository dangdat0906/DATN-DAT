﻿using System;
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
    [Route("api/tac-gia")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorManager _authorManager;
        public AuthorsController(IAuthorManager authorManager)
        {
            this._authorManager = authorManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Authors inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Tên tác giả {MessageConst.NOT_EMPTY_INPUT}");
                }
                inputModel.CreatedDate = DateTime.Now;
                await _authorManager.Create(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Authors inputModel)
        {
            try
            {
                var data = await _authorManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                inputModel.CreatedDate = data.CreatedDate;
                inputModel.UpdatedDate = DateTime.Now;
                await _authorManager.Update(inputModel);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("look-up")]
        public async Task<IActionResult> Look_Up(long newsSourceId = 0)
        {
            try
            {
                var data = await _authorManager.Look_up(newsSourceId);
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
                var data = await _authorManager.Find_By_Id(id);
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

                var data = await _authorManager.Find_By_Name(inputName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name, int status,long newSourceId, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = await _authorManager.Get_List(name, status, newSourceId, pageSize, pageNumber);
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
        public async Task<IActionResult> UpdateStatus([FromBody] Authors inputModel)
        {
            try
            {
                var data = await _authorManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _authorManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] Authors inputModel)
        {
            try
            {
                var data = await _authorManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _authorManager.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
