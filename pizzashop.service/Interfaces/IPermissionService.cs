using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IPermissionService
{
    Task<List<PermissionViewModel>> GetPermissions(int Roleid);
    Task UpdatePermissions(List<PermissionViewModel> permissions);

    Task<Permission> GetPermissions(string role , string module);
}
