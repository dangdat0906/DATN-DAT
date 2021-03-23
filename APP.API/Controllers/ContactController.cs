using Microsoft.AspNetCore.Mvc;
using APP.MANAGER;
using APP.MODELS;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.API.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactManager _contact;
        public ContactController(IContactManager contact)
        {
            this._contact = contact;
        }

        [HttpPost("update")]
        public async Task<IActionResult> Insert([FromBody] Contacts inputModels)
        {
            try
            {
              
                inputModels.UpdatedDate = DateTime.Now;
                await _contact.Update(inputModels);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("update-view")]
        public async Task<IActionResult> UpdateView(string langCode="VIE")
        {
            try
            {              
                await _contact.updateView(langCode);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("delete")]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            try
            {
                var data = await _contact.Find_By_Id(id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                //data.is_delete = true;
                //data.DeletedDate = DateTime.Now;
                await _contact.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("find-by-id")]
        public async Task<IActionResult> FindById(int id)
        {
            try
            {
                var data = await _contact.Find_By_Id(id);
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
        [HttpGet("find-by-langcode")]
        public async Task<IActionResult> FindByLangCode(string langCode)
        {

            var data = await _contact.Find_By_LangCode(langCode); 
            return Ok(data);
        }
        [HttpGet("get-by-statuswebsite")]
        public async Task<IActionResult> GetByStatusWebSite(string langCode = "vie")
        {

            try
            {
                var data = await _contact.Get(langCode);
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup()
        {
            try
            {
                var data = await _contact.Get();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
