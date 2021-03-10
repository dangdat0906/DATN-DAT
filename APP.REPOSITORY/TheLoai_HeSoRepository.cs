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
    public interface ITheLoai_HeSoRepository : IRepository<TheLoai_HeSo>
    {
        IQueryable<TheLoai_HeSo> GetFullListTheLoai_HeSo();
    }
    public class TheLoai_HeSoRepository: Repository<TheLoai_HeSo>, ITheLoai_HeSoRepository
    {
        private readonly APPDbContext _db;
        public TheLoai_HeSoRepository(APPDbContext context) : base(context)
        {
            this._db = context;
        }
        public IQueryable<TheLoai_HeSo> GetFullListTheLoai_HeSo()
        {
            try
            {
                var data =  (from t in _db.Types
                            join tl in _db.TheLoai_HeSo
                            on t.Id equals tl.TypeId into full
                            from fulltl in full.DefaultIfEmpty()
                            where t.Status == (byte)StatusEnum.Active
                            select new TheLoai_HeSo()
                            {
                                TypeName = t.Name,
                                TypeId = t.Id,
                                Coefficient = fulltl.Coefficient,
                                FromDate = fulltl.FromDate,
                                ToDate = fulltl.ToDate,
                                Status = fulltl.Status
                            });
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }

}
