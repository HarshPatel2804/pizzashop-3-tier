using Microsoft.AspNetCore.Http;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;
namespace pizzashop.service.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IRoleService _RoleService;

    private readonly IPermissionRepository _permissionRepository;

    private readonly IHttpContextAccessor _httpContextAccessor;
    int RoleId = 0;

    public PermissionService(IRoleService RoleService, IPermissionRepository permissionRepository, IHttpContextAccessor httpContextAccessor)
    {
        _RoleService = RoleService;
        _permissionRepository = permissionRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<(List<PermissionViewModel>, bool success)> GetPermissions(int Roleid , HttpContext httpContext)
    {
         CookiesViewModel user = SessionUtils.GetUser(httpContext);
        
        if (user != null)
        {
            RoleId = user.roleId;
        }
        var model = new List<PermissionViewModel>();
        if(RoleId < Roleid){
            model = await _permissionRepository.GetPermissionByRole(Roleid);
        }
        else{
            return (model , false);
        }

        return (model , true);
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

    public async Task<Permission> GetPermissions(string role, string module)
    {
        return await _permissionRepository.GetPermissionByRoleAndModule(role, module);
    }

    public async Task<Permission> GetCurrentUserPermissionsForModule(string moduleName)
    {
        CookiesViewModel user = SessionUtils.GetUser(_httpContextAccessor.HttpContext);

        if (user == null)
        {
            return new Permission();
        }

        Role userRole = await _RoleService.GetRoleById(user.roleId);

        if (userRole == null)
        {
            return new Permission();
        }

        return await GetPermissions(userRole.Rolename, moduleName);
    }

}
