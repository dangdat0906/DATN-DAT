using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using APP.UTILS;
using System.Linq;

namespace APP.MANAGER
{
    public interface IQuanLyDonGiaNhuanButManager
    {
        Task Create(QuanLyDonGiaNhuanBut inputModel);
        Task Update(QuanLyDonGiaNhuanBut inputModel);
        Task Delete(long id);
        Task<QuanLyDonGiaNhuanBut> Find_By_Id(long id);
        Task<List<QuanLyDonGiaNhuanBut>> Get_List();
    }
    public class QuanLyDonGiaNhuanButManager : IQuanLyDonGiaNhuanButManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuanLyDonGiaNhuanBut> _logger;
        public QuanLyDonGiaNhuanButManager(IUnitOfWork unitOfWork, ILogger<QuanLyDonGiaNhuanBut> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Create(QuanLyDonGiaNhuanBut inputModel)
        {
            try
            {
                var result = await _unitOfWork.QuanLyDonGiaNhuanButRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Delete(long id)
        {
            try
            {
                var item = await _unitOfWork.QuanLyDonGiaNhuanButRepository.Get(c => c.Id == id);
                item.Status = (byte)StatusEnum.Removed;
                await _unitOfWork.QuanLyDonGiaNhuanButRepository.Update(item);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<QuanLyDonGiaNhuanBut>> Get_List()
        { 
            try
            {
                var data = (await _unitOfWork.QuanLyDonGiaNhuanButRepository.GetAll()).ToList();
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task Update(QuanLyDonGiaNhuanBut inputModel)
        {
            try
            {
                await _unitOfWork.QuanLyDonGiaNhuanButRepository.Update(inputModel);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {    
                throw ex;
            }
        }
        public async Task<QuanLyDonGiaNhuanBut> Find_By_Id(long id)
        {
            try
            {
                var data = await _unitOfWork.QuanLyDonGiaNhuanButRepository.Get(c => c.Id == id);
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            } 
        }
    }
}
