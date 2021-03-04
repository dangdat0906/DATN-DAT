using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface ICategoryRepository: IRepository<Categories>
    {

    }
    public class CategoryRepository: Repository<Categories>,ICategoryRepository
    {
        public CategoryRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
