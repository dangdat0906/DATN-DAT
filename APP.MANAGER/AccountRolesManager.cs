using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.MANAGER
{
    public interface IAccountRolesManager
    {
        Task<AccountRoles> Create(AccountRoles inputModel);
        Task CreateList(long accountId, List<long> listRoleId);
        Task Delete(long accountId);
        Task Update(AccountRoles inputModel);
        Task<AccountRoles> Find_By_Id(long id);
        Task<List<AccountRoles>> GetByAccountId(long accountId);
    }
    public class AccountRolesManager : IAccountRolesManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountRoles> _logger;
        public AccountRolesManager(IUnitOfWork unitOfWork, ILogger<AccountRoles> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AccountRoles> Create(AccountRoles inputModel)
        {
            try
            {
                var result = await _unitOfWork.AccountRolesRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(AccountRoles inputModel)
        {
            await _unitOfWork.CreateTransaction();
            try
            {
                await _unitOfWork.AccountRolesRepository.Update(inputModel);
                await _unitOfWork.Commit();
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();
                throw ex;
            }
        }
        public async Task<AccountRoles> Find_By_Id(long id)
        {
            return await _unitOfWork.AccountRolesRepository.Get(c => c.Id == id);
        }

        public async Task CreateList(long accountId, List<long> listRoleId)
        {
            try
            {
                foreach (var roleId in listRoleId)
                {
                    AccountRoles accountRoles = new AccountRoles();
                    accountRoles.RoleId = roleId;
                    accountRoles.AccountId = accountId;
                    await _unitOfWork.AccountRolesRepository.Add(accountRoles);
                    await _unitOfWork.SaveChange();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(long accountId)
        {
            try
            {
                var data = (await _unitOfWork.AccountRolesRepository.FindBy(x => x.AccountId == accountId)).ToList();
                if (data.Count > 0)
                {
                    foreach (var accountRole in data)
                    {
                        await _unitOfWork.AccountRolesRepository.Delete(accountRole);
                        await _unitOfWork.SaveChange();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<AccountRoles>> GetByAccountId(long accountId)
        {
            try
            {
                var data = (await _unitOfWork.AccountRolesRepository.FindBy(x => x.AccountId == accountId)).ToList();
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
