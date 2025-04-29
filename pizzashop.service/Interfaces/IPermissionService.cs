using Microsoft.AspNetCore.Http;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IPermissionService
{
    Task<(List<PermissionViewModel>, bool success)> GetPermissions(int Roleid , HttpContext httpContext);
    Task UpdatePermissions(List<PermissionViewModel> permissions);

    Task<Permission> GetPermissions(string role , string module);

    Task<Permission> GetCurrentUserPermissionsForModule(string moduleName);
}
