using Microsoft.AspNetCore.Mvc;
using pizzashop.service;
using pizzashop.repository.ViewModels;
using pizzashop.service.Utils;
using pizzashop.service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using pizzashop.service.Attributes;

namespace pizzashop.web.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _config;

    private readonly IAuthService _AuthService;

    private readonly IEmailService _EmailService;

    private readonly IJwtService _JwtService;

    public HomeController(IConfiguration config, IAuthService AuthService, IEmailService EmailService, IJwtService JwtService)
    {
        _config = config;
        _AuthService = AuthService;
        _EmailService = EmailService;
        _JwtService = JwtService;
    }
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
        var user = SessionUtils.GetUser(HttpContext);
        var token = CookieUtils.GetJWTToken(Request);
        if (user != null && token != null)
            return RedirectToAction("Dashboard", "Dashboard");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel loginViewModel)
    {

        if (ModelState.IsValid)
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
                TempData["ErrorMessage"] = "Invalid Email or Password";
                return View();
            }
        }
        else
        {
            return View();
        }
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _AuthService.ForgotPassword(model.email);
        // Console.WriteLine("EMAIl" + email);
        Console.WriteLine(user);
        if (user)
        {
            var token = _JwtService.GenerateJwtResetToken(model.email);
            var url = Url.ActionLink("ResetPassword", "Home", new { Token = token }, Request.Scheme);
            await _EmailService.SendResetPasswordEmailAsync(model.email, url);
            TempData["SuccessMessage"] = "Email sent successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Email is incorrect";
        }
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        Console.WriteLine(token + "token");
        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        bool isValidLink = await _AuthService.ResetPasswordAsync(model);
        if (!isValidLink)
        {
            TempData["ErrorMessage"] = "Reset link can be used for one time only";
            return View(model);
        }
        return RedirectToAction("Index", "Home");


    }

    [HttpPost]
    public IActionResult logout()
    {
        CookieUtils.ClearCookies(HttpContext);
        SessionUtils.ClearSession(HttpContext);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Error(int statusCode)
    {
        return View();
    }
}
