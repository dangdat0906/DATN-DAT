using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IContent_GroupsRepository : IRepository<Content_Groups>
    {

    }
    public class Content_GroupsRepository: Repository<Content_Groups>, IContent_GroupsRepository
    {
        public Content_GroupsRepository(DbContext dbContext) : base(dbContext)
        {         
        }
    }
}
