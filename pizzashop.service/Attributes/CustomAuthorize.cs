using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using pizzashop.service;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;


namespace pizzashop.service.Attributes
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _moduleName;
        private readonly string _permissionType;

        public CustomAuthorizeAttribute(string? moduleName = null, string? permissionType = null)
        {
            _moduleName = moduleName ?? string.Empty;
            _permissionType = permissionType ?? string.Empty;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            OnAuthorizationAsync(context).GetAwaiter().GetResult();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;
            var permissionService = context.HttpContext.RequestServices.GetService(typeof(IPermissionService)) as IPermissionService;

            var token = CookieUtils.GetJWTToken(context.HttpContext.Request);

            var principal = jwtService?.ValidateToken(token ?? "");
            if (principal == null)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            context.HttpContext.User = principal;

            if (string.IsNullOrEmpty(_moduleName) || string.IsNullOrEmpty(_permissionType))
            {
                return;
            }

            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == null)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 403 });
                return;
            }

            var permission = await permissionService?.GetPermissions(role, _moduleName);

            if (permission == null)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 403 });
                return;
            }

            bool hasPermission = _permissionType switch
            {
                "CanView" => permission.Canview ?? false,
                "CanAddEdit" => permission.Canaddedit ?? false,
                "CanDelete" => permission.Candelete ?? false,
                _ => false
            };

            if (!hasPermission && (_permissionType == "CanAddEdit" || _permissionType == "CanDelete"))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 403 , permissionType = _permissionType});
            }

            else if (!hasPermission)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 403 });
            }
        }
    }
}
