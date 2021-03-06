using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IContentsRepository : IRepository<Contents>
    {

    }
    public class ContentsRepository: Repository<Contents>,IContentsRepository
    {
        public ContentsRepository(DbContext dbContext): base(dbContext)
        {

        }
    }
}
