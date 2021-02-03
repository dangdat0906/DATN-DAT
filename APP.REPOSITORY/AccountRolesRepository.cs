using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IAccountRolesRepository: IRepository<AccountRoles>
    {

    }
    public class AccountRolesRepository: Repository<AccountRoles>,IAccountRolesRepository
    {
        public AccountRolesRepository(DbContext context) : base(context)
        {

        }
    }
}
