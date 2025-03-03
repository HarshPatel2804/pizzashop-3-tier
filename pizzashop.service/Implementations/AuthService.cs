using pizzashop.service.Utils;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;
using Microsoft.AspNetCore.Http;
using pizzashop.repository.ViewModels;
using System.Data;

namespace pizzashop.service.Implementations;

public class AuthService : IAuthService
{
    private readonly IUsersLoginService _usersloginService;
    private readonly IJwtService _jwtService;
    private readonly IRoleService _roleService;


    public AuthService(IUsersLoginService usersloginService, IJwtService jwtService, IRoleService roleService)
    {
        _usersloginService = usersloginService;
        _jwtService = jwtService;
        _roleService = roleService;
    }
    public async Task<bool> AuthenticateUser(LoginViewModel loginViewModel, HttpContext httpContext)
    {
        var usersLogin = await _usersloginService.GetUserByEmail(loginViewModel.Email);
        if (usersLogin == null || !PasswordUtills.VerifyPassword(loginViewModel.Password, usersLogin.Passwordhash))
            return false;
        var role = await _roleService.GetRoleById(usersLogin.Roleid);

        // Generate JWT Token
        var token = _jwtService.GenerateJwtToken(usersLogin.Email, role.Rolename);

        // Save token in Cookie
        CookieUtils.SaveJWTToken(httpContext.Response, token);

        // Save User Data in Cookie for Remember Me
        if (loginViewModel.Remember)
        {
            CookieUtils.SaveUserData(httpContext.Response, usersLogin);
        }

        SessionUtils.SetUser(httpContext, usersLogin);

        return true;
    }

    public async Task<bool> ForgotPassword(string email){
         var usersLogin = await _usersloginService.GetUserByEmail(email);
         if(usersLogin == null) return false;
         return true;
    }
}
