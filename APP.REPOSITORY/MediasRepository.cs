using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IMediasRepository : IRepository<Medias>
    {

    }
    public class MediasRepository: Repository<Medias>, IMediasRepository
    {
        public MediasRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
