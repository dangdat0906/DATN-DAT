using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IRolesRepository : IRepository<Roles>
    {

    }
    public class RolesRepository:Repository<Roles>,IRolesRepository
    {
        public RolesRepository(DbContext db): base(db)
        {

        }
    }
}
