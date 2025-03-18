using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Interfaces;
using pizzashop.service;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;

namespace pizzashop.web.Controllers;

[CustomAuthorize]
public class RoleController : Controller
{
     private readonly IRoleService _RoleService;
     private readonly IPermissionService _PermissionService;

    public RoleController(IRoleService RoleService , IPermissionService PermissionService)
    {
       _RoleService = RoleService;
       _PermissionService = PermissionService;
    }

     public async Task<IActionResult> Role()
    {
        var Roles = await _RoleService.GetAllRoles();
        return View(Roles);
    }

    public async Task<IActionResult> Permission(int id)
    {
        var permissions = await _PermissionService.GetPermissions(id);
        
        return View(permissions);
    }

    [HttpPost]
    public async Task<IActionResult> Permission([FromBody] List<PermissionViewModel> permissions)
    {
        if (permissions == null || permissions.Count == 0)
        {
            return Json(new { success = false });
        }
        await _PermissionService.UpdatePermissions(permissions);
        return Json(new { success = true });
    }
}
