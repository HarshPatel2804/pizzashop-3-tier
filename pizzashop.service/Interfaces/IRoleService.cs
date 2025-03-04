using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.service.Interfaces;

public interface IRoleService
{
    Task<Role> GetRoleById(int Roleid);

    Task<List<SelectListItem>> GetAllRoles();
}
