using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System.Linq;
using APP.UTILS;

namespace APP.REPOSITORY
{
    public interface IAuthorsRepository : IRepository<Authors>
    {
        IQueryable<Authors> GetFullListAuthor(); // status = active
        IQueryable<Authors> GetFullList(); //GetAll
        Authors Find_By_Id_Querry(long id);
    }
    public class AuthorsRepository : Repository<Authors>, IAuthorsRepository
    {
        private readonly APPDbContext _db;
        public AuthorsRepository(APPDbContext context) : base(context)
        {
            this._db = context;
        }
        public Authors Find_By_Id_Querry(long id)
        {
            try
            {
                var data = (from a in _db.Authors
                           join s in _db.NewsSources on a.NewsSourcesId equals s.Id
                           where a.Status == (byte)StatusEnum.Active && a.Id == id
                           select new Authors()
                           {
                               Id = a.Id,
                               Name = a.Name,
                               NewsSourcesId = a.NewsSourcesId,
                               NewsSourcesName = s.Name,
                               Status = a.Status
                           }).Cast<Authors>().FirstOrDefault();
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<Authors> GetFullListAuthor()
        {
            //List full tac gia kem ten co quan where status = active
            try
            {
                var data = from a in _db.Authors
                           join s in _db.NewsSources on a.NewsSourcesId equals s.Id
                           where a.Status == (byte)StatusEnum.Active
                           select new Authors()
                           {
                               Id = a.Id,
                               Name = a.Name,
                               NewsSourcesId = a.NewsSourcesId,
                               NewsSourcesName = s.Name,
                               Status = a.Status
                           };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<Authors> GetFullList()
        {
            //List full tac gia kem ten co quan
            try
            {
                var data = from a in _db.Authors
                           join s in _db.NewsSources on a.NewsSourcesId equals s.Id
                           select new Authors()
                           {
                               Id = a.Id,
                               Name = a.Name,
                               NewsSourcesId = a.NewsSourcesId,
                               NewsSourcesName = s.Name,
                               Status = a.Status
                           };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
