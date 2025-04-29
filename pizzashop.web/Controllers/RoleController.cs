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

    [CustomAuthorize("RoleAndPermission", "CanView")]
     public async Task<IActionResult> Role()
    {
        var Roles = await _RoleService.GetAllRoles();
        return View(Roles);
    }
    [CustomAuthorize("RoleAndPermission", "CanView")]
    public async Task<IActionResult> Permission(int id)
    {
        var (permissions , Success) = await _PermissionService.GetPermissions(id , HttpContext);

        if(!Success){
            TempData["ErrorMessage"] = "Invalid Request!";
            return RedirectToAction("Role","Role");
        }
        
        return View(permissions);
    }
    [CustomAuthorize("RoleAndPermission", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> Permission([FromBody] List<PermissionViewModel> permissions)
    {

        if (permissions == null || permissions.Count == 0)
        {
            // TempData["ErrorMessage"] = "No Unsaved Changes!";
            return Json(new { success = false });
        }
        await _PermissionService.UpdatePermissions(permissions);
        TempData["SuccessMessage"] = "Changes are Saved Successfully!";
        return Json(new { success = true });
    }
}
