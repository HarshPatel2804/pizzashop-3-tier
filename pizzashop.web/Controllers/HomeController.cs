using Microsoft.AspNetCore.Mvc;
using pizzashop.service;
using pizzashop.repository.ViewModels;
using pizzashop.service.Utils;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _config;

    private readonly IAuthService _AuthService;

    private readonly IEmailService _EmailService;

    public HomeController( IConfiguration config,IAuthService AuthService , IEmailService EmailService)
    {
        _config = config;
        _AuthService = AuthService;
        _EmailService = EmailService;
    }
    [HttpGet]
    public IActionResult Index()
    {
        var user = SessionUtils.GetUser(HttpContext);
        if (user != null)
            return RedirectToAction("Dashboard", "Dashboard");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel loginViewModel)
    {
        if (string.IsNullOrEmpty(loginViewModel.Email) || string.IsNullOrEmpty(loginViewModel.Password))
        {
            ViewBag.ErrorMessage = "Email and password are required";
            return View();
        }
        var user = await _AuthService.AuthenticateUser(loginViewModel, HttpContext);
        
        TempData["LoginEmail"] = loginViewModel.Email;
        if (user == true)
        {
            return RedirectToAction("Dashboard", "Dashboard");
        }
        else
        {
              TempData["ErrorMessage"] = "Invalid email or password";
            return View();
        }
    }



    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = _AuthService.ForgotPassword(email);
        // Console.WriteLine("EMAIl" + email);
        if (user != null)
        {
            var url = Url.ActionLink("ResetPassword", "Home");
            await _EmailService.SendResetPasswordEmailAsync(email , url);
        }
        else
        {
            ViewBag.Error = "Email is incorrect";
        }
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    // [HttpPost]
    // public IActionResult ResetPassword(string password, string conf_password)
    // {
    //     var email = TempData["email"].ToString();
    //     if (password != conf_password)
    //     {
    //         ViewBag.Error = "Both passwords are different!";
    //         return View();
    //     }
    //     var user = _context.Userslogins.FirstOrDefault(u => u.Email == email);
    //     if (user != null)
    //     {
    //         user.Passwordhash = password;
    //         _context.SaveChanges();
    //         ViewBag.Success = "Password changed successfully!";
    //     }
    //     else
    //     {
    //         ViewBag.Error = "Email does not exist!";
    //     }
    //     return View();
    // }

}
