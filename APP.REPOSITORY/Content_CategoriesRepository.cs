﻿using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Repository
{
    public interface IContent_CategoriesRepository : IRepository<Content_Categories>
    {

    }
    public class Content_CategoriesRepository : Repository<Content_Categories>, IContent_CategoriesRepository
    {
        public Content_CategoriesRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
