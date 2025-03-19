using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.service.Interfaces;

namespace pizzashop.repository.Implementations;

public class RoleRepository : IRoleRepository
{
     private readonly PizzaShopContext _context;

    public RoleRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<Role> GetUserRoleAsync(int Roleid)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Roleid == Roleid);
    }

    public async Task<List<SelectListItem>> GetAllRoleAsync()
    {
        return await _context.Roles
        .Select(r => new SelectListItem
        {
            Value = r.Roleid.ToString(),
            Text = r.Rolename,
        })
        .ToListAsync();
    }

}
