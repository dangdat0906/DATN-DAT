using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.MANAGER;
using APP.MODELS;
using APP.UTILS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace APP.API.Controllers
{
    [Route("api/nhom-quyen")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesManger _rolesManger;
        private readonly IRole_PermissionsManager _role_PermissionsManager;
        public RolesController(IRolesManger rolesManger, IRole_PermissionsManager role_PermissionsManager)
        {
            this._rolesManger = rolesManger;
            this._role_PermissionsManager = role_PermissionsManager;
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name = "", int status = -1, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = await _rolesManger.GetList(name, status, pageSize, pageNumber);
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
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Roles inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Tên nhóm quyền {MessageConst.NOT_EMPTY_INPUT}");
                }
                var exist = await _rolesManger.FindByName(inputModel.Name.ToLower().Trim());
                if (exist != null)
                {
                    throw new Exception($"Tên nhóm quyền {MessageConst.EXIST}");
                }
                inputModel.Name = Extensions.StringStandar(inputModel.Name, 1);
                //inputModel.CreatedDate = DateTime.Now;
                Roles role = await _rolesManger.Create(inputModel);
                if (inputModel.Role_Permissions != null)
                {
                    await CreateRolePermission(role);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        public async Task CreateRolePermission(Roles inputModel)
        {
            await _role_PermissionsManager.Delete(inputModel.Id);
            await _role_PermissionsManager.CreateList(inputModel);
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Roles inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Tên nhóm quyền {MessageConst.NOT_EMPTY_INPUT}");
                }
                var data = await _rolesManger.FindById(inputModel.Id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                inputModel.Name = Extensions.StringStandar(inputModel.Name, 1);
                var exist = await _rolesManger.FindByName(inputModel.Name.Trim());
                if (exist != null && (!inputModel.Name.Equals(exist.Name)))
                {
                    throw new Exception($"Tên nhóm quyền {MessageConst.EXIST}");
                }
                //inputModel.CreatedDate = data.CreatedDate;
                //inputModel.UpdatedDate = DateTime.Now;
                await _rolesManger.Update(inputModel);
                if (inputModel.Role_Permissions != null)
                {

                    await CreateRolePermission(inputModel);
                }
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
                var data = await _rolesManger.LookUp();
                return Ok(data.Select(x => new { Value = x.Id, Title = x.Name }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-id")]
        public async Task<IActionResult> FindById(long id)
        {
            try
            {
                var data = await _rolesManger.FindById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-permisstion-by-roleid")]
        public async Task<IActionResult> GetPermission(long roleId)
        {
            try
            {

                var data = await _role_PermissionsManager.GetByRoleId(roleId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] Roles inputModel)
        {
            try
            {
                var data = await _rolesManger.FindById(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _rolesManger.Update(data);
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
                var data = await _rolesManger.FindById(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _rolesManger.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
