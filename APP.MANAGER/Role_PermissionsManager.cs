using Microsoft.Extensions.Logging;
using APP.MODELS;
using APP.REPOSITORY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace APP.MANAGER
{
    public interface IRole_PermissionsManager
    {
        Task Delete(long rolePermissionId);
        Task CreateList(Roles inputModel);
        Task<List<Role_Permissions>> GetByRoleId(long roleId);
    }
    public class Role_PermissionsManager:IRole_PermissionsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Role_Permissions> _logger;
        public Role_PermissionsManager(IUnitOfWork unitOfWork, ILogger<Role_Permissions> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Delete(long roleId)
        {
            try
            {
                var data = (await _unitOfWork.Role_PermissionsRepository.FindBy(c => c.RoleId == roleId)).ToList();
                if (data.Count > 0)
                {
                    //await _unitOfWork.CreateTransaction();
                    foreach (var item in data)
                    {
                        await _unitOfWork.Role_PermissionsRepository.Delete(item);
                        await _unitOfWork.SaveChange();
                    }
                    //await _unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CreateList(Roles inputModel)
        {
            try
            {

                foreach (var item in inputModel.Role_Permissions)
                {
                    item.RoleId = inputModel.Id;
                    await _unitOfWork.Role_PermissionsRepository.Add(item);
                    await _unitOfWork.SaveChange();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

        public async Task<List<Role_Permissions>> GetByRoleId(long roleId)
        {
            var data = (await _unitOfWork.Role_PermissionsRepository.FindBy(x => x.RoleId == roleId)).ToList();
            foreach(var item in data)
            {
                var linkMenu = await _unitOfWork.MenuRepository.Get(x => x.Id == item.MenuId);
                item.MenuUrl = linkMenu.Url;
            }
            return data;
        }
    }
}
