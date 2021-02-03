using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IMenuRepository:IRepository<Menus>
    {

    }
    public class MenuRepository:Repository<Menus>,IMenuRepository
    {
        public MenuRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
