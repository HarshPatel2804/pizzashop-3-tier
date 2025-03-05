using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface IPermissionRepository
{
    Task<List<PermissionViewModel>> GetPermissionByRole(int Roleid);
    Task<Permission> GetPermissionByPermissionId(int Permissionid);

    Task UpdatePermission(Permission permission);
}
