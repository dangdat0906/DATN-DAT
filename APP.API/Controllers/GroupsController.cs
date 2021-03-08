using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APP.UTILS;
using APP.MODELS;
using APP.MANAGER;

namespace APP.API.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupsController : Controller
    {
        private readonly IGroupsManager _groupsManager;
        public GroupsController(IGroupsManager groups)
        {
            this._groupsManager = groups;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Groups inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Name's Group {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputModel.Name.Length > 50)
                {
                    throw new Exception($"Name's Group {MessageConst.LENGTH_ERROR}");
                }
                if (Validation.HasSpecialChar(inputModel.Name))
                {
                    throw new Exception($"Name's Group {MessageConst.SPECIAL_CHAR}");
                }
                var exist = await _groupsManager.Find_By_Name(inputModel.Name);
                if (exist != null)
                {
                    throw new Exception($"Name's Group {MessageConst.EXIST}");
                }
                inputModel.CreatedDate = DateTime.Now;
                await _groupsManager.Create(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Groups inputModel)
        {
            try
            {
                var data = await _groupsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Name's Group {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputModel.Name.Length > 50)
                {
                    throw new Exception($"Name's Group {MessageConst.LENGTH_ERROR}");
                }
                if (Validation.HasSpecialChar(inputModel.Name))
                {
                    throw new Exception($"Name's Group {MessageConst.SPECIAL_CHAR}");
                }
                if (!string.IsNullOrEmpty(inputModel.Note))
                {
                    if (inputModel.Note.Length > 50)
                    {
                        throw new Exception($"Note's Group {MessageConst.LENGTH_ERROR}");
                    }
                    if (Validation.HasSpecialChar(inputModel.Note))
                    {
                        throw new Exception($"Note's Group {MessageConst.SPECIAL_CHAR}");
                    }
                }
                data.Name = inputModel.Name ?? data.Name;
                data.Note = inputModel.Note == null ? Extensions.StringStandar(inputModel.Note, 1) : Extensions.StringStandar(inputModel.Note, 1);
                data.Status = inputModel.Status == -1 ? data.Status : inputModel.Status;
                data.UpdatedDate = DateTime.Now;
                await _groupsManager.Update(data);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //[HttpGet("look-up")]
        //public async Task<IActionResult> Look_Up(string langCode = "VIE")
        //{
        //    try
        //    {
        //        var data = await _groupsManager.Look_up();
        //        var result = data.Where(c => c.LangCode == langCode).ToList().Select(c => new { Value = c.Id, Title = c.Name });
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        [HttpGet("find-by-id")]
        public async Task<IActionResult> Find_By_ID(long id)
        {
            try
            {
                var data = await _groupsManager.Find_By_Id(id);
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
                if (string.IsNullOrEmpty(inputName))
                {
                    throw new Exception($"Name's Group {MessageConst.NOT_EMPTY_INPUT}");
                }
                if (inputName.Length > 50)
                {
                    throw new Exception($"Name's Group {MessageConst.LENGTH_ERROR}");
                }
                if (Validation.HasSpecialChar(inputName))
                {
                    throw new Exception($"Name's Group {MessageConst.SPECIAL_CHAR}");
                }
                var data = await _groupsManager.Find_By_Name(inputName);
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
                var data = await _groupsManager.Get_List(name, status, pageSize, pageNumber);
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
        public async Task<IActionResult> UpdateStatus([FromBody] Groups inputModel)
        {
            try
            {
                var data = await _groupsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _groupsManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] Groups inputModel)
        {
            try
            {
                var data = await _groupsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _groupsManager.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("look-up")]
        public async Task<IActionResult> LookUp()
        {
            try
            {
                var data = await _groupsManager.Look_up();
                return Ok(data.Select(x => new { Value = x.Id, Title = x.Name }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}