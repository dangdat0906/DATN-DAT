using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
   
    public interface INewsSourcesRepository : IRepository<NewsSources>
    {

    }
    public class NewsSourcesRepository : Repository<NewsSources>, INewsSourcesRepository
    {
        public NewsSourcesRepository(DbContext context) : base(context)
        {

        }
    }
}
