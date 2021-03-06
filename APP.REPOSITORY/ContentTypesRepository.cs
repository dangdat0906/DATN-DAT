using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IContentTypesRepository:IRepository<ContentTypes>
    {

    }
    public class ContentTypesRepository: Repository<ContentTypes>, IContentTypesRepository
    {
        public ContentTypesRepository(DbContext dbContext):base(dbContext)
        {

        }
    }
}
