using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.UTILS;

namespace APP.MANAGER
{
    public interface IMenuManager
    {
        Task<Menus> Create(Menus inputModel);
        Task Update(Menus inputModel);
        Task<Menus> Find_By_Id(long id);
        Task<List<MenuViewModels>> Get_list(string name, long parentId, int status, int pageSize, int pageNumber);

        Task<Menus> Find_By_Name(string ten);
        Task<List<Menus>> Look_up();
        Task Delete(long id);
        Task<List<Menus>> GetListChild();
        Task<List<Menus>> GetChild(long parentId);
        Task UpdateStatus(Menus inputModel);



    }
    public class MenuManager : IMenuManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Menus> _logger;
        public MenuManager(IUnitOfWork unitOfWork, ILogger<Menus> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Menus> Create(Menus inputModel)
        {
            try
            {
                var result = await _unitOfWork.MenuRepository.Add(inputModel);
                await _unitOfWork.SaveChange();
                return result;
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
                var data = await _unitOfWork.MenuRepository.Get(x => x.Id == id);
                data.Status = (byte)StatusEnum.Removed;
                await _unitOfWork.MenuRepository.Update(data);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Menus> Find_By_Id(long id)
        {
            return await _unitOfWork.MenuRepository.Get(c => c.Id == id);
        }

        public async Task<Menus> Find_By_Name(string name)
        {
            return await _unitOfWork.MenuRepository.Get(c => c.Name.Equals(name));
        }

        public async Task<List<Menus>> GetChild(long parentId)
        {

            var ListMenu = (await _unitOfWork.MenuRepository.FindBy(x => x.ParentId == parentId)).ToList();
            if (ListMenu.Count > 0)
            {
                foreach (var item in ListMenu)
                {
                    item.ListChild = await GetChild(item.Id);
                }
                return ListMenu;
            }
            return null;
        }

        public async Task<List<Menus>> GetListChild()
        {
            try
            {
                var data = (await _unitOfWork.MenuRepository.FindBy(x => x.ParentId == 0)).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ListChild = await GetChild(item.Id);
                    }
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<MenuViewModels>> Get_list(string name = "", long parentId = 0, int status = -1, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var data = (await _unitOfWork.MenuRepository.FindBy(x =>
                (x.Status == status || status == (int)StatusEnum.All) &&
                (x.Status != (byte) StatusEnum.Removed || status == (int)StatusEnum.Removed) &&
                (string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name)))).ToList();
                List<MenuViewModels> result = new List<MenuViewModels>();

                //result = data.Select(c => new MenuViewModels()
                //{
                //    Id = c.Id,
                //    ParentName = c.Parentld == null || c.Parentld == 0 ? "" : data.Find(e => e.Id == c.Parentld).Name
                //}).ToList();

                foreach (var item in data)
                {
                    string parentName = "";

                    var parentItem = data.Find(c => c.Id == item.ParentId);
                    var listChild = data.Where(x => x.Id == item.Id).ToList();
                    if (parentItem != null)
                    {
                        parentName = parentItem.Name;

                    }
                    result.Add(new MenuViewModels()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Note = item.Note,
                        ParentId = item.ParentId,
                        Url = item.Url,
                        Status = item.Status,
                        ParentName = parentName,
                        DisplayOrder = item.DisplayOrder,
                        ListChild = listChild

                    });
                }
                var returnResult = result.Where(x => x.ParentId == parentId || parentId == 0).ToList();
                return returnResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Menus>> Look_up()
        {
            return (await _unitOfWork.MenuRepository.GetAll()).ToList();
        }

        public async Task Update(Menus inputModel)
        {
            try
            {
                await _unitOfWork.MenuRepository.Update(inputModel);
                await _unitOfWork.SaveChange();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateStatus(Menus inputModel)
        {
            try
            {
                await _unitOfWork.MenuRepository.Update(inputModel);
                await _unitOfWork.SaveChange();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
