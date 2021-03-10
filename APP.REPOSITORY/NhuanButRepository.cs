using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface INhuanButRepository : IRepository<NhuanBut>
    {

    }
    public class NhuanButRepository : Repository<NhuanBut>, INhuanButRepository
    {
        public NhuanButRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
