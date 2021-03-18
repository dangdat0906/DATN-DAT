using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using APP.MODELS;
using APP.REPOSITORY;
using System.Linq;
using APP.UTILS;
using System.Globalization;

namespace APP.MANAGER
{
    public interface IThongKeNhuanButManager
    {
        Task<List<Authors>> Get_List_Author();
        Task<List<TheLoai_HeSo>> Get_List_Type(string month);
        Task<QuanLyDonGiaNhuanBut> Get_Don_Gia(string month);
        Task<List<Contents>> Get_Content_By_Month(string month);
        Task<List<Contents>> Get_Content_By_Author_Month(long authorId,string month);
    }
    public class ThongKeNhuanButManager : IThongKeNhuanButManager
    {
        private readonly IUnitOfWork _unitOfWork;
        public ThongKeNhuanButManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public string conver(string date)
        {
            var s = date.Split("/");
            string d = s[1] + s[0] + "01";
            return d;
        }
        public async Task<QuanLyDonGiaNhuanBut> Get_Don_Gia(string month)
        {
            try
            {
                DateTime fromDate = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(month))
                {
                    fromDate = DateTime.ParseExact(conver(month), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                }
                var data = await _unitOfWork.QuanLyDonGiaNhuanButRepository.Get(c => fromDate.Month >= c.FromMonth.Month 
                && fromDate.Month <= c.ToMonth.Month && fromDate.Year >= c.FromMonth.Year && fromDate.Year <= c.ToMonth.Year);  
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TheLoai_HeSo>> Get_List_Type(string month)
        {
            try
            {
                DateTime fromDate = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(month))
                {
                    fromDate = DateTime.ParseExact(conver(month), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                }
                // list full type left join TheLoai_HeSo where type.Status = active
                var listFullTheLoai_HeSo = _unitOfWork.TheLoai_HeSoRepository.GetFullListTheLoai_HeSo();
                // list TheLoai_HeSo theo thang
                var data = listFullTheLoai_HeSo.Where(c => ((fromDate.Month >= c.FromDate.Month
                                                       && fromDate.Month <= c.ToDate.Month
                                                       && fromDate.Year >= c.FromDate.Year
                                                       && fromDate.Year <= c.ToDate.Year)
                                                       || c.FromDate == null && c.ToDate == null)
                                                       && (c.Status == (byte)StatusEnum.Active) || c.Status == null ).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Authors>> Get_List_Author()
        {
            try
            {
                //List full tac gia kem ten co quan where status = active
                return _unitOfWork.AuthorsRepository.GetFullListAuthor().ToList();              
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Contents>> Get_Content_By_Month(string month)
        {
            try
            {
                DateTime monthDateTime = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(month))
                {
                    monthDateTime = DateTime.ParseExact(conver(month), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                }
                //Danh sach bai viet da phe duyet theo thang
                var listContentByMonth = (await _unitOfWork.ContentsRepository.FindBy(c => c.PublishDate.Value.Month == monthDateTime.Month
                                                                                     && c.PublishDate.Value.Year == monthDateTime.Year
                                                                                     && c.Status == (byte)ContentStatusEnum.Approved)).ToList();
                return listContentByMonth;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Contents>> Get_Content_By_Author_Month(long authorId, string month /*int typeId*/)
        {
            try
            {
                DateTime monthDateTime = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(month))
                {
                    monthDateTime = DateTime.ParseExact(conver(month), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                }
                //Danh sach bai viet da phe duyet theo tac gia
                var listContentByAuthor = (await _unitOfWork.ContentsRepository.FindBy(c => c.PublishDate.Value.Month == monthDateTime.Month
                                                                                     && c.PublishDate.Value.Year == monthDateTime.Year
                                                                                     //&& c.ContentType == typeId
                                                                                     && c.Status == (byte)ContentStatusEnum.Approved && c.AuthorId == authorId)).ToList();
                return listContentByAuthor;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
