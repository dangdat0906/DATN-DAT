using Microsoft.EntityFrameworkCore;
using Portal.Data;
using APP.MODELS;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.REPOSITORY
{
    public interface IQuanLyDonGiaNhuanButRepository : IRepository<QuanLyDonGiaNhuanBut>
    {

    }
    public class QuanLyDonGiaNhuanButRepository : Repository<QuanLyDonGiaNhuanBut>, IQuanLyDonGiaNhuanButRepository
    {
        public QuanLyDonGiaNhuanButRepository(DbContext context) : base(context)
        {

        }
    }
}
