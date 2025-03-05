using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly PizzaShopContext _context;

    public PermissionRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<List<PermissionViewModel>> GetPermissionByRole(int Roleid)
    {
         var permissions = await _context.Permissions
            .Include(u => u.Module)
            .Include(u => u.Role)
            .Where(u => u.Roleid == Roleid)
            .OrderByDescending(u => u.Module.Moduleid)
            .Select(u => new PermissionViewModel
            {
                PermissionId = u.Permissionid,
                Rolename = u.Role.Rolename,
                ModuleName = u.Module.Modulename,
                CanView = (bool)u.Canview,
                CanEdit = (bool)u.Canaddedit,
                CanDelete = (bool)u.Candelete
            }
            ).ToListAsync();

            return permissions;
    }

    public async Task<Permission> GetPermissionByPermissionId(int Permissionid)
    {
        return await  _context.Permissions.FirstOrDefaultAsync(p => p.Permissionid == Permissionid);
    }

    public async Task UpdatePermission(Permission permission)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync();
    }
}
