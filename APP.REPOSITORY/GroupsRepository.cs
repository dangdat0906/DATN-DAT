using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IGroupsRepository: IRepository<Groups>
    {

    }
    public class GroupsRepository: Repository<Groups>, IGroupsRepository
    {
        public GroupsRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
