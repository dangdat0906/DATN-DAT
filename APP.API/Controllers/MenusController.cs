using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APP.MANAGER;
using APP.MODELS;
using APP.UTILS;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace APP.API.Controllers
{
    [Route("api/menu")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly IMenuManager _menuManager;
        public MenusController(IMenuManager IMenus)
        {
            this._menuManager = IMenus;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Menus inputModel)
        {
            try
            {
                var exist = await _menuManager.Find_By_Name(inputModel.Name);
                if (exist != null)
                {
                    throw new Exception($"Tên Menu {MessageConst.EXIST}");
                }
               
                inputModel.CreatedDate = DateTime.Now;
                       
                await _menuManager.Create(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
       
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Menus inputModel)
        {
            try
            {
                var data = await _menuManager.Find_By_Id(inputModel.Id);
                if(data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
               
                var exist = await _menuManager.Find_By_Name(inputModel.Name);
               
                inputModel.CreatedDate = data.CreatedDate;
                inputModel.UpdatedDate = DateTime.Now;
                await _menuManager.Update(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] Menus inputModel)
        {
            try
            {
                var data = await _menuManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _menuManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] Menus inputModel)
        {
            try
            {
                var data = await _menuManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }              
                await _menuManager.Delete(inputModel.Id);
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
                var data = await _menuManager.Find_By_Id(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name = "",long parentId = 0, int status = -1, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = await _menuManager.Get_list(name, parentId,status, pageSize, pageNumber);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("look-up")]
        public async Task<IActionResult> LookUp()
        {
            try
            {
                var data = await _menuManager.Look_up();
                return Ok(data.Select(x => new { Value = x.Id, Title = x.Name }));
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-child")]
        public async Task<IActionResult> GetListChild()
        {
            try
            {
                var data = await _menuManager.GetListChild();
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
