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
    [Route("api/tai-khoan")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsManager _accountsManager;
        private readonly IAccountRolesManager _accountRolesManager;

        public AccountsController(IAccountsManager Accounts, IAccountRolesManager accountRolesManager)
        {
            this._accountsManager = Accounts;
            this._accountRolesManager = accountRolesManager;


        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Accounts inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.UserName))
                {
                    throw new Exception($"Tên tài khoản  {MessageConst.NOT_EMPTY_INPUT}");
                }

                if (Validation.HasSpecialChar(inputModel.UserName))
                {
                    throw new Exception($"Tên tài khoản  {MessageConst.SPECIAL_CHAR}");
                }
                var exist = await _accountsManager.Find_By_Name(inputModel.UserName);
                if (exist != null)
                {
                    throw new Exception($"Tên tài khoản {MessageConst.EXIST}");
                }

                if (!Validation.IsValidEmail(inputModel.Email))
                {
                    throw new Exception($"Email không hợp lệ");
                }
                var checkEmai = await _accountsManager.FindByEmail(inputModel.Email);
                if (checkEmai != null)
                {
                    throw new Exception($"Email đã được dùng cho tài khoản khác");
                }
                inputModel.FullName = Extensions.StringStandar(inputModel.FullName, 1);
                inputModel.CreatedDate = DateTime.Now;
                var data = await _accountsManager.Create(inputModel);
                await CreateAccountRole(data.Id, inputModel.ListRoleId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        public async Task CreateAccountRole(long accountId, List<long> listRoleId)
        {
            await _accountRolesManager.Delete(accountId);
            await _accountRolesManager.CreateList(accountId, listRoleId);
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Accounts inputModel)
        {
            try
            {
                var data = await _accountsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                if (string.IsNullOrEmpty(inputModel.UserName))
                {
                    throw new Exception($"Tên tài khoản  {MessageConst.NOT_EMPTY_INPUT}");
                }

                if (Validation.HasSpecialChar(inputModel.UserName))
                {
                    throw new Exception($"Tên tải khoản {MessageConst.SPECIAL_CHAR}");
                }
                var exist = await _accountsManager.Find_By_Name(inputModel.UserName);
                if (exist != null && (!inputModel.UserName.Equals(exist.UserName)))
                {
                    throw new Exception($"Tên tài khoản {MessageConst.EXIST}");
                }

                if (!Validation.IsValidEmail(inputModel.Email))
                {
                    throw new Exception($"Email không hợp lệ");
                }
                var checkEmail = await _accountsManager.FindByEmail(inputModel.Email);
                if (checkEmail != null && !inputModel.Email.Equals(checkEmail.Email))
                {
                    throw new Exception($"Email đã được dùng cho tài khoản khác");
                }
                
                inputModel.UpdatedDate = DateTime.Now;
                inputModel.FullName = Extensions.StringStandar(inputModel.FullName, 1);
                inputModel.CreatedDate = data.CreatedDate;
                await _accountsManager.Update(inputModel);
                await CreateAccountRole(inputModel.Id, inputModel.ListRoleId);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] Accounts inputModel)
        {
            try
            {
                var data = await _accountsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await _accountsManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] Accounts inputModel)
        {
            try
            {
                var data = await _accountsManager.Find_By_Id(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _accountsManager.Delete(inputModel.Id);
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
                var data = await _accountsManager.Look_up();
                return Ok(data.Select(c => new { Value = c.Id, Title = c.UserName }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-id")]
        public async Task<IActionResult> Find_By_ID(int id)
        {
            try
            {
                var data = await _accountsManager.Find_By_Id(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-name")]
        public async Task<IActionResult> Find_By_Name(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception($"Tên tài khoản  {MessageConst.NOT_EMPTY_INPUT}");
                }

                if (Validation.HasSpecialChar(userName))
                {
                    throw new Exception($"Tên tài khoản {MessageConst.SPECIAL_CHAR}");
                }
                var exist = await _accountsManager.Find_By_Name(userName);
                if (exist != null)
                {
                    return Ok(exist);
                }
                else
                {
                    throw new Exception($"Tên tài khoản không tồn tại");

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string userName, string fullName, int status = -1, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = await _accountsManager.Get_List(userName, fullName, status, pageSize, pageNumber);
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
        [HttpPost("dang-nhap")]
        public async Task<IActionResult> Login([FromBody] Accounts inputModel)
        {
            try
            {
                
                var data = await _accountsManager.Login(inputModel.UserName, inputModel.Password);
                if (data == null)
                    throw new Exception("Tài khoản hoặc mật khẩu không đúng");
                data.Token = CreateToken();
                data.ExpiredToken = DateTime.Now.AddHours(12);
                await _accountsManager.UpdateToken(data);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private string CreateToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Console.WriteLine(token);
            return token;
        }
        [HttpPost("doi-mat-khau")]
        public async Task<IActionResult> ChangePass([FromBody] Accounts inputModel)
        {
            try
            {

                var data = await _accountsManager.ChangePass(inputModel.UserName, inputModel.Password, inputModel.NewPass);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-roles-by-acountid")]
        public async Task<IActionResult> GetRolesByAccountId(long accountId)
        {
            try
            {
                var data = await _accountRolesManager.GetByAccountId(accountId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-statistical")]
        public async Task<IActionResult> Get_statistical()
        {
            try
            {
                var data = await _accountsManager.Getstatistical();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("thong-ke-bai-viet-theo-tai-khoan")]
        public async Task<IActionResult> GetListContentByAccountCreated(string tuNgay = "", string denNgay = "")
        {
            try
            {
                var data = await _accountsManager.GetListAccountCreateContents(tuNgay,denNgay);
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
        [HttpGet("quen-mat-khau")]
        public async Task<IActionResult> ForgetPass(string email)
        {
            try
            {
                 await _accountsManager.ForgetPass(email);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
