using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IAccountsRepository : IRepository<Accounts>
    {

    }
    public class AccountsRepository : Repository<Accounts>, IAccountsRepository
    {
        public AccountsRepository(DbContext context) : base(context)
        {

        }
    }
   
}
