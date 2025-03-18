using pizzashop.service;
using pizzashop.service.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace pizzashop.service.Attributes
{
    /// <summary>
    /// Extend with Attribute Class to make this class an Attribute.
    /// IAuthorizationFilter Interface used to implement the OnAuthorization lifeCycle method.
    /// </summary>
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Inject JwtService to use in Middleware.
            var jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;
            
            // Get the token from Cookie
            var token = CookieUtils.GetJWTToken(context.HttpContext.Request);

            // Validate Token
            var principal = jwtService?.ValidateToken(token ?? "");
            if (principal == null)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            context.HttpContext.User = principal;

            if (_roles.Length > 0)
            {
                // Get Role Claim from the principal
                var userRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (!_roles.Contains(userRole))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                }
            }
        }
    }
}
