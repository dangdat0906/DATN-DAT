using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IContactRepository : IRepository<Contacts>
    {

    }
    public class ContactRepository : Repository<Contacts>, IContactRepository
    {
        public ContactRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
