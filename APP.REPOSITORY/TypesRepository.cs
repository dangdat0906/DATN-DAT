using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface ITypesRepository : IRepository<Types>
    {

    }
    public class TypesRepository : Repository<Types>, ITypesRepository
    {
        public TypesRepository(DbContext context) : base(context)
        {

        }
    }
}
