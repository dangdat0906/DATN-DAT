using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.MANAGER
{
    public interface IContactManager
    {
        Task<Contacts> Create(Contacts inputModel);
        Task Update(Contacts inputModel);
        Task Delete(long id);
        Task<Contacts> Find_By_Id(long id);
        Task<Contacts> Find_By_Name(string inputName);
        Task<Contacts> Find_By_LangCode(string LangCode);
        Task<Contacts> Get(string langCode = "vie");
        Task updateView(string langCode = "VIE");


    }
    public class ContactManager : IContactManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Contacts> _logger;
        public ContactManager(IUnitOfWork unitOfWork, ILogger<Contacts> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Contacts> Create(Contacts inputModel)
        {
            try
            {
                var result = await _unitOfWork.ContactRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Update(Contacts inputModel)
        {
            try
            {

                if (inputModel.Id != 0)
                {
                    var item = await _unitOfWork.ContactRepository.Get(x => x.Id == inputModel.Id);
                    inputModel.LangCode = item.LangCode;
                    inputModel.Online = item.Online;
                    inputModel.OnlineOnMonth = item.OnlineOnMonth;
                    inputModel.OnlineTotal = item.OnlineTotal;
                    await _unitOfWork.ContactRepository.Update(inputModel);
                    await _unitOfWork.SaveChange();
                }
                else
                {
                    await _unitOfWork.ContactRepository.Add(inputModel);
                    await _unitOfWork.SaveChange();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Delete(long id)
        {
            var item = await _unitOfWork.ContactRepository.Get(c => c.Id == id);
            await _unitOfWork.ContactRepository.Delete(item);
            await _unitOfWork.Commit();
        }

        public async Task<Contacts> Find_By_Id(long id)
        {
            return await _unitOfWork.ContactRepository.Get(c => c.Id == id);
        }
        public async Task<Contacts> Find_By_LangCode(string LangCode)
        {
            return await _unitOfWork.ContactRepository.Get(c => c.LangCode == LangCode);
        }
        public async Task<Contacts> Find_By_Name(string inputName)
        {
            return await _unitOfWork.ContactRepository.Get(c => c.Name.ToLower() == inputName.Trim().ToLower());
        }

        public async Task<Contacts> Get(string langCode = "vie") //Lay thong tin 
        {
            return await _unitOfWork.ContactRepository.Get(c => c.StatusWebsite == true && c.LangCode.ToLower().Equals(langCode));
        }

        public async Task updateView(string langCode = "VIE")
        {
            try
            {
                var data = (await _unitOfWork.ContactRepository.FindBy(c=>c.LangCode == langCode)).ToList().FirstOrDefault();
                if (data == null)
                {
                    throw new Exception("404");
                }
                var day = (int)DateTime.Now.Day;
                var Month = (int)DateTime.Now.Month;

                if (data.Day == null)
                {
                    data.Day = day;
                }
                if (data.Month == null)
                {
                    data.Month = Month;
                }
                if (data.Day == day)
                {
                    data.Online = 0;
                    //data.Day = day+1;
                }
                if(data.Day == 31)
                {
                    data.Day = 1;
                }
                if(data.Month == 13)
                {
                    data.Month = 1;
                }
                if (data.Month == Month)
                {
                    data.OnlineOnMonth = 0;
                    //data.Month = Month+1;
                }
                data.Online += 1;
                data.OnlineOnMonth += 1;
                data.OnlineTotal += 1;
                await _unitOfWork.ContactRepository.Update(data);
                await _unitOfWork.SaveChange();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
