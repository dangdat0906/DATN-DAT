using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface ITitleImagesRepository : IRepository<TitleImages>
    {

    }
    public class TitleImagesRepository : Repository<TitleImages>, ITitleImagesRepository
    {
        public TitleImagesRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
