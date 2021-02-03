using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IRole_PermissionsRepository: IRepository<Role_Permissions>
    {

    }
    public class Role_PermissionsRepository : Repository<Role_Permissions>,IRole_PermissionsRepository
    {
        public Role_PermissionsRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
