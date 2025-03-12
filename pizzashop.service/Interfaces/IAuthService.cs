using Microsoft.AspNetCore.Http;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service
{
    public interface IAuthService
    {
        Task<bool> AuthenticateUser(LoginViewModel loginViewModel, HttpContext  httpContext);

        Task<bool> ForgotPassword(string email);

        Task<bool> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}
