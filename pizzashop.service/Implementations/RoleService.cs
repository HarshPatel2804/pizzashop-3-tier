using pizzashop.repository.Models;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _RoleRepository;

    public RoleService(IRoleRepository RoleRepository)
    {
        _RoleRepository = RoleRepository;
    }
    public async Task<Role> GetRoleById(int Roleid)
    {
        return await _RoleRepository.GetUserRoleAsync(Roleid);
    }

}
