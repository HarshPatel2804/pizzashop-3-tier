using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.service.Interfaces;

public interface IRoleRepository
{
    Task<Role> GetUserRoleAsync(int Roleid);

    Task<List<SelectListItem>> GetAllRoleAsync(int Roleid);
}
