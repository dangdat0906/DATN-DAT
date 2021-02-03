
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using APP.MODELS;
using APP.REPOSITORY;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace APP.MANAGER
{
    public interface IAccountsManager
    {
        Task<Accounts> Create(Accounts inputModel);
        Task<Accounts> Update(Accounts inputModel);
        Task Delete(long id);
        Task<Accounts> Find_By_Id(long id);
        Task<Accounts> Find_By_Name(string inputName);
        Task<Accounts> Login(string userName, string password);
        Task<List<Accounts>> Look_up();
        Task<List<Accounts>> Get_List(string userName, string fullName, int status, int pageSize, int pageNumber);
        Task<Accounts> ChangePass(string userName, string passWord, string newPassword);
        Task<Accounts> FindByEmail(string email);
        Task<List<Role_Permissions>> GetRole_PermissionByAccountId(long accountId);
        Task<Accounts> CheckToken(string token);
        Task<Accounts> UpdateToken(Accounts inputModel);
        Task<List<LookViewModels>> Getstatistical();
        Task<List<Accounts>> GetListAccountCreateContents(string tuNgay = "", string denNgay = "");
        Task ForgetPass(string email);
    }
    public class AccountsManager : IAccountsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Accounts> _logger;
        public AccountsManager(IUnitOfWork unitOfWork, ILogger<Accounts> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Accounts> Create(Accounts inputModel)
        {
            try
            {
                inputModel.UserName = inputModel.UserName.ToLower();
                inputModel.Password = MD5.ToMD5(inputModel.Password);
                var result = await _unitOfWork.AccountsRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SendEmail(string mail_to, string mail_subject, string mail_body)
        {
            bool result = false;
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("hoplyfc19@gmail.com", "adm12345");
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("hoplyfc10@gmail.com");
                mailMessage.To.Add(mail_to);
                mailMessage.Body = mail_body;
                mailMessage.Subject = mail_subject;
                client.Send(mailMessage);
                result = true;
            }
            catch (Exception ex) { result = false; }
            return result;
        }
        public async Task<Accounts> Update(Accounts inputModel)
        {

            try
            {
                var data = await _unitOfWork.AccountsRepository.Get(x => x.Id == inputModel.Id);
                if (data.Password == inputModel.Password)
                {
                    inputModel.UserName = inputModel.UserName.ToLower();
                    await _unitOfWork.AccountsRepository.Update(inputModel);
                    await _unitOfWork.SaveChange();
                }
                else
                {
                    inputModel.UserName = inputModel.UserName.ToLower();
                    inputModel.Password = MD5.ToMD5(inputModel.Password);
                    await _unitOfWork.AccountsRepository.Update(inputModel);
                    await _unitOfWork.SaveChange();
                }

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Delete(long id)
        {
            var item = await _unitOfWork.AccountsRepository.Get(c => c.Id == id);
            item.Status = (byte)RolesEnum.Delete;
            await _unitOfWork.AccountsRepository.Update(item);
            await _unitOfWork.SaveChange();
        }

        public async Task<Accounts> Find_By_Id(long id)
        {
            return await _unitOfWork.AccountsRepository.Get(c => c.Id == id);
        }

        public async Task<Accounts> Find_By_Name(string inputName)
        {
            return await _unitOfWork.AccountsRepository.Get(c => c.UserName.Equals(inputName));
        }

        public async Task<List<Accounts>> Get_List(string userName = "", string fullName = "", int status = -1, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = (await _unitOfWork.AccountsRepository.FindBy(x => (x.Status == status || status == (int)StatusEnum.All)
                                                                    && ((string.IsNullOrEmpty(userName) || x.UserName.ToLower().Contains(userName)))
                                                                    && (x.Status != (byte)StatusEnum.Removed || status == (int)StatusEnum.Removed)
                                                                    && ((string.IsNullOrEmpty(fullName) || x.FullName.ToLower().Contains(fullName))))).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Accounts>> Look_up()
        {
            try
            {
                var data = (await _unitOfWork.AccountsRepository.FindBy(c => c.Status == (int)StatusEnum.Active)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Accounts> Login(string userName, string password)
        {
            password = MD5.ToMD5(password);
            var data = await _unitOfWork.AccountsRepository.Get(x => x.UserName.Equals(userName)
                                                                && x.Password.Equals(password)
                                                                && x.Status == (byte)StatusEnum.Active);
            if (data == null)
            {

                throw new Exception("Tài khoản hoặc mật khẩu không đúng");
            }
            else
            {
                if (data.IsFirstLogin == null)
                {
                    data.IsFirstLogin = true;
                }


                var listRolePermission = await GetRole_PermissionByAccountId(data.Id);
                data.Role_Permissions = listRolePermission;
                var listMenu = (await _unitOfWork.MenuRepository.FindBy(x => true)).ToList();
                var listMenuOfAccount = new List<Menus>();
                foreach (var item in listMenu)
                {
                    var menu = listRolePermission.Find(x => x.MenuId == item.Id);
                    if (menu != null)
                    {
                        listMenuOfAccount.Add(item);
                    }
                }
                data.ListMenu = listMenuOfAccount;
                return data;
            }


        }

        public async Task<Accounts> ChangePass(string userName, string password, string newPassword)
        {
            password = MD5.ToMD5(password);
            var data = await _unitOfWork.AccountsRepository.Get(x => x.UserName.Equals(userName) && x.Password.Equals(password));
            if (data == null)
            {
                throw new Exception("Mật khẩu không đúng");
            }
            else
            {
                data.IsFirstLogin = false;
                newPassword = MD5.ToMD5(newPassword);
                data.Password = newPassword;
                await _unitOfWork.AccountsRepository.Update(data);
                await _unitOfWork.SaveChange();
                return data;
            }
        }

        public async Task<Accounts> FindByEmail(string email)
        {
            try
            {
                var data = await _unitOfWork.AccountsRepository.Get(x => x.Email.Equals(email));
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Role_Permissions>> GetRole_PermissionByAccountId(long accountId)
        {
            try
            {
                var listRole = (await _unitOfWork.AccountRolesRepository.FindBy(x => x.AccountId == accountId)).ToList();
                var listRolePermissions = new List<Role_Permissions>();
                foreach (var item in listRole)
                {
                    var data = (await _unitOfWork.Role_PermissionsRepository.FindBy(x => x.RoleId == item.RoleId)).ToList();
                    if (data != null)
                    {
                        foreach (var role_Permission in data)
                        {
                            var linkMenu = await _unitOfWork.MenuRepository.Get(x => x.Id == role_Permission.MenuId);
                            role_Permission.MenuUrl = linkMenu.Url;
                        }
                        listRolePermissions = listRolePermissions.Union(data).ToList();
                    }
                }
                return listRolePermissions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Accounts> CheckToken(string token)
        {

            var now = DateTime.Now.Date;
            var data = await _unitOfWork.AccountsRepository.Get(x => x.Token == token && x.ExpiredToken.Value.Date >= now);
            return data;
        }

        public async Task<Accounts> UpdateToken(Accounts inputModel)
        {
            try
            {
                inputModel.UserName = inputModel.UserName.ToLower();
                await _unitOfWork.AccountsRepository.Update(inputModel);
                await _unitOfWork.SaveChange();
                var data = await _unitOfWork.AccountsRepository.Get(x => x.Id == inputModel.Id);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<LookViewModels>> Getstatistical()
        {
            try
            {
                var data = (await _unitOfWork.AccountsRepository.GetAll()).ToList();

                var listApproved = data.Where(x => x.Status == (byte)StatusEnum.Active).ToList();
                var lisUnactive = data.Where(x => x.Status == (byte)StatusEnum.Unactive).ToList();
                var listDelete = data.Where(x => x.Status == (byte)StatusEnum.Removed).ToList();
                List<LookViewModels> list = new List<LookViewModels>();
                LookViewModels approved = new LookViewModels()
                {
                    Value = listApproved.Count,
                    Title = "Đang hoạt động"
                };
                LookViewModels unactive = new LookViewModels()
                {
                    Value = lisUnactive.Count,
                    Title = "Đã khóa"
                };
                LookViewModels delete = new LookViewModels()
                {
                    Value = listDelete.Count,
                    Title = "Đã xóa"
                };
                LookViewModels all = new LookViewModels()
                {
                    Value = data.Count,
                    Title = "Tổng số tài khoản"
                };
                list.Add(approved);
                list.Add(unactive);
                list.Add(delete);
                list.Add(all);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public async Task<List<Accounts>> GetListAccountCreateContents(string tuNgay = "", string denNgay = "")
        //{
        //    try
        //    {
        //        DateTime formdate = DateTime.Now.Date;
        //        DateTime toDate = DateTime.Now.Date;
        //        if (!string.IsNullOrEmpty(tuNgay))
        //        {

        //            formdate = DateTime.ParseExact(conver(tuNgay), "yyyyMMdd", CultureInfo.InvariantCulture).Date;

        //            toDate = DateTime.ParseExact(conver(denNgay), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
        //        }
        //        var listAccoutn = (await _unitOfWork.AccountsRepository.GetAll()).ToList();
        //        if (listAccoutn == null)
        //        {
        //            throw new Exception("404");
        //        }
        //        foreach (var acc in listAccoutn)
        //        {
        //            var listContent = (await _unitOfWork.ContentsRepository.FindBy(x => x.CreatedBy == acc.Id
        //                   && (string.IsNullOrEmpty(tuNgay) || (x.PublishDate.Value.Date > formdate && x.PublishDate.Value.Date < toDate))
        //                   && x.Status != (byte)ContentStatusEnum.Delete)).ToList();
        //            var tenDonVi = "";
        //            var donvi = await _unitOfWork.UnitsRepository.Get(x => x.Id == acc.UnitsId);
        //            if(donvi != null)
        //            {
        //                tenDonVi = donvi.Name;
        //            }
        //            acc.tenDonVi = tenDonVi;
        //            acc.ListContent = listContent;
        //        }
        //        return listAccoutn;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public string conver(string date)
        {
            var s = date.Split("/");
            string d = s[2] + s[1] + s[0];
            return d;
        }

      

        public async Task ForgetPass(string email)
        {
            try
            {
                var account = (await _unitOfWork.AccountsRepository.Get(x => x.Email.ToLower().Equals(email.ToLower())));
                if(account == null)
                {
                    throw (new Exception("Email không tồn tại trong hệ thống"));
                }
                var pass = Extensions.RandomPassword(6);
                string body = "Hệ thống quản trị Trang TTĐT tổng hợp Ban ĐNTW xin thông báo: Mật khẩu của bạn đã được đặt lại là: " + pass;
                SendEmail(account.Email, "Thông báo đặt lại mật khẩu Trang TTĐT tổng hợp Ban ĐNTW ", body);
                pass = MD5.ToMD5(pass);
                account.Password = pass;
                await _unitOfWork.AccountsRepository.Update(account);
                await _unitOfWork.SaveChange();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<Accounts>> GetListAccountCreateContents(string tuNgay = "", string denNgay = "")
        {
            throw new NotImplementedException();
        }
    }
}
