
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using pizzashop.service;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;


namespace pizzashop.service.Attributes
{
public class CustomAuthForAppAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _moduleName;

        public CustomAuthForAppAttribute(string moduleName)
        {
            _moduleName = moduleName ?? string.Empty;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            OnAuthorizationAsync(context).GetAwaiter().GetResult();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;

            if (jwtService == null)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 500, message = "JWTService not configured." });
                return;
            }

            var token = CookieUtils.GetJWTToken(context.HttpContext.Request);

            var principal = jwtService.ValidateToken(token ?? string.Empty);
            if (principal == null)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null); 
                return;
            }


            context.HttpContext.User = principal;

            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roleClaim == null || string.IsNullOrEmpty(roleClaim.Value))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 403, message = "Role not found in token." });
                return;
            }

            string userRole = roleClaim.Value;
            List<string> modules = new List<string>
            {
                "OrderKOT" , "OrderMenu" , "OrderTable" , "OrderWaitingList"
            };

            if (userRole == "Admin" && modules.Contains(_moduleName))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 403, message = "Access restricted for this role." });
                return;
            }

            if (userRole == "Chef")
            {
                if (!string.Equals(_moduleName, "OrderKOT"))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", new { statusCode = 403, message = $"Access to module '{_moduleName}' restricted for this role." });
                    return;
                }
            }

            await Task.CompletedTask;
        }
    }
}