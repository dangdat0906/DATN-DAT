using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using APP.UTILS;
using System.Linq;

namespace APP.MANAGER
{
    public interface ITheLoai_HeSoManager
    {
        Task Create(TheLoai_HeSo inputModel);
        Task Update(TheLoai_HeSo inputModel);
        Task<TheLoai_HeSo> Find_By_Id(long id);
        Task<TheLoai_HeSo> Find_By_Type_Id(long id,string month);
        Task Delete(long id);
        Task<List<TheLoai_HeSo>> Get_List(string month, long typeId, int status);
        Task<List<Types>> Look_Up_TheLoai();
    }
    public class TheLoai_HeSoManager: ITheLoai_HeSoManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TheLoai_HeSo> _logger;
        public TheLoai_HeSoManager(IUnitOfWork unitOfWork, ILogger<TheLoai_HeSo> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Create(TheLoai_HeSo inputModel)
        {
            try
            {
                var result = await _unitOfWork.TheLoai_HeSoRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            };
        }

        public async Task Delete(long id)
        {
            try
            {
                var item = await _unitOfWork.TheLoai_HeSoRepository.Get(c => c.Id == id);
                item.Status = (byte)StatusEnum.Removed;
                await _unitOfWork.TheLoai_HeSoRepository.Update(item);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TheLoai_HeSo> Find_By_Id(long id)
        {
            try
            {
                var data = await _unitOfWork.TheLoai_HeSoRepository.Get(c => c.Id == id);
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<TheLoai_HeSo> Find_By_Type_Id(long id,string month)
        {
            try
            {
                DateTime fromDate = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(month))
                {
                    fromDate = DateTime.ParseExact(conver(month), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                }
                var data = await _unitOfWork.TheLoai_HeSoRepository.Get(c => c.TypeId == id && (fromDate.Month >= c.FromDate.Month
                && fromDate.Month <= c.ToDate.Month && fromDate.Year >= c.FromDate.Year && fromDate.Year <= c.ToDate.Year));
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TheLoai_HeSo>> Get_List(string month,long typeId,int status)
        {
            try
            {
                DateTime fromDate = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(month))
                {
                    fromDate = DateTime.ParseExact(conver(month), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                }
                var data = (await _unitOfWork.TheLoai_HeSoRepository.FindBy(c =>((string.IsNullOrEmpty(month)) || (fromDate.Month >= c.FromDate.Month
                && fromDate.Month <= c.ToDate.Month && fromDate.Year >= c.FromDate.Year && fromDate.Year <= c.ToDate.Year))
                && (typeId == 0 || c.TypeId == typeId) && (status == (int)StatusEnum.All || c.Status == status ))).ToList();
                foreach(var item in data)
                {
                    item.TypeName = (await _unitOfWork.TypesRepository.Get(c => c.Id == item.TypeId)).Name;
                }
                data = data.OrderByDescending(c => c.ToDate).ToList();
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public string conver(string date)
        {
            var s = date.Split("/");
            string d = s[1] + s[0] + "01";
            return d;
        }
        public async Task Update(TheLoai_HeSo inputModel)
        {
            try
            {
                await _unitOfWork.TheLoai_HeSoRepository.Update(inputModel);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Types>> Look_Up_TheLoai()
        {
            try
            {
                var data = (await _unitOfWork.TypesRepository.FindBy(c=>c.Status == (byte)StatusEnum.Active)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
