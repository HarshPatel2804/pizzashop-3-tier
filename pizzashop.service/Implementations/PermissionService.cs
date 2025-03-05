using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class PermissionService : IPermissionService
{
     private readonly IRoleService _RoleService;

     private readonly IPermissionRepository _permissionRepository;

     public PermissionService(IRoleService RoleService , IPermissionRepository permissionRepository)
    {
       _RoleService = RoleService;
       _permissionRepository = permissionRepository;
    }
    public async Task<List<PermissionViewModel>> GetPermissions(int Roleid)
    {
        var model = await _permissionRepository.GetPermissionByRole(Roleid);

        return model;
    }

    public async Task UpdatePermissions(List<PermissionViewModel> permissions)
    {
        foreach (var permission in permissions)
        {
            var existingPermission = await _permissionRepository.GetPermissionByPermissionId(permission.PermissionId);

            if (existingPermission != null)
            {
                existingPermission.Canview = permission.CanView;
                existingPermission.Canaddedit = permission.CanEdit;
                existingPermission.Candelete = permission.CanDelete;
            }

            await _permissionRepository.UpdatePermission(existingPermission);
        }
    }

}
