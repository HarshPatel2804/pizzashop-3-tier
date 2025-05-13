using pizzashop.service.Utils;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.service.Interfaces;
using Microsoft.AspNetCore.Http;
using pizzashop.repository.ViewModels;
using System.Data;
using pizzashop.repository.Interfaces;
using System.Security.Claims;

namespace pizzashop.service.Implementations;

public class AuthService : IAuthService
{
    private readonly IUsersLoginService _usersloginService;
    private readonly IJwtService _jwtService;
    private readonly IRoleService _roleService;

    private readonly IUsersloginRepository _usersloginRepository;

    private readonly IUserService _userService;

    public AuthService(IUsersLoginService usersloginService, IJwtService jwtService, IRoleService roleService, IUsersloginRepository usersloginRepository , IUserService userService)
    {
        _usersloginService = usersloginService;
        _jwtService = jwtService;
        _roleService = roleService;
        _usersloginRepository = usersloginRepository;
        _userService = userService;
    }
    public async Task<bool> AuthenticateUser(LoginViewModel loginViewModel, HttpContext httpContext)
    {
        var usersLogin = await _usersloginService.GetUserByEmail(loginViewModel.Email);
        if (usersLogin == null || !PasswordUtills.VerifyPassword(loginViewModel.Password, usersLogin.Passwordhash) || usersLogin.status==repository.Models.statustype.Inactive)
            return false;
        var role = await _roleService.GetRoleById(usersLogin.Roleid);
        var user = await _userService.GetUserById((int)usersLogin.Userid);
        if(user.Isdeleted == true) return false;
        Console.WriteLine(user.Profileimg);

        // Generate JWT Token
        var token = _jwtService.GenerateJwtToken(usersLogin.Email, role.Rolename);

        // Save token in Cookie
        CookieUtils.SaveJWTToken(httpContext.Response, token);

        // Save User Data in Cookie for Remember Me
        if (loginViewModel.Remember)
        {
            CookieUtils.SaveUserData(httpContext.Response, usersLogin, user);
        }

        SessionUtils.SetUser(httpContext, usersLogin, user);

        return true;
    }

    public async Task<bool> ForgotPassword(string email)
    {
        var usersLogin = await _usersloginService.GetUserByEmail(email);
        if (usersLogin == null) return false;
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        if (model.Password == model.ConfirmPassword)
        {
            var principal = _jwtService.ValidateToken(model.Token);

            var emailClaim = principal?.FindFirst(ClaimTypes.Email);

            var usersLogin = await _usersloginService.GetUserByEmail(emailClaim.Value);
            if (usersLogin == null) return false;

            if(usersLogin.ResetToken != model.Token) return false;
            if(usersLogin.IsfirstLogin == true){
                usersLogin.IsfirstLogin = false;
            } 

            usersLogin.Passwordhash = model.Password;
            await _usersloginRepository.UpdateUserLoginDetails(usersLogin);
            await _usersloginService.SetResetTokenAsync(emailClaim.Value , null);
            return true;
        }
        return false;
    }
}
