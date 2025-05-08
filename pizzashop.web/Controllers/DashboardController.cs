using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;
using pizzashop.service.Attributes;
using pizzashop.service.Constants;

namespace pizzashop.web.Controllers;

[CustomAuthorize]
public class DashboardController : Controller
{
    private readonly IProfileService _ProfileService;
    private readonly PizzaShopContext _context;

    private readonly IDashboardService _dashboardService;

    public DashboardController(IProfileService ProfileService , PizzaShopContext context , IDashboardService dashboardService)
    {
        _ProfileService = ProfileService;
        _context = context;
        _dashboardService = dashboardService;
    }
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
        public async Task<IActionResult> Data(DateTime startDate, DateTime endDate)
        {
            var data = await _dashboardService.GetDashboardDataAsync(startDate, endDate);
            return PartialView("_DashboardPartial",data);
        }

    public async Task<IActionResult> Profile(string from)
    {
        var userData = SessionUtils.GetUser(HttpContext);
        if (userData == null)
            return RedirectToAction("index", "Home");
        var Id = userData.Id;
        var model = await _ProfileService.GetProfileData((int)Id);
        Console.WriteLine(model.Profileimg + "image");
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model , IFormFile ProfileImage,[FromForm(Name = "from")] string? from)
    { 
            ModelState.Remove(nameof(model.Countries));
            ModelState.Remove(nameof(model.States));
            ModelState.Remove(nameof(model.Cities));
            ModelState.Remove(nameof(model.Roles));
            ModelState.Remove(nameof(model.Profileimg));
            ModelState.Remove(nameof(ProfileImage));
         if(!ModelState.IsValid) return View(model);
        await _ProfileService.UpdateProfileData(model , ProfileImage);
        
        return RedirectToAction("Profile","Dashboard",new { from = from });
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
         if(!ModelState.IsValid) return View(model);
        var userData = SessionUtils.GetUser(HttpContext);
        model.Email = userData.Email;
        Console.WriteLine(model.OldPassword + "Password");
        var (Success , Message) = await _ProfileService.UpdatePassword(model);
        if(!Success){
            TempData["ErrorMessage"] = Message;
            return RedirectToAction("ChangePassword","Dashboard");
        }
        TempData["SuccessMessage"] = Message;
        return RedirectToAction("Dashboard","Dashboard");
    }

    
    public JsonResult GetStates(int countryId)
    {
        var states = _context.States.Where(s => s.Countryid == countryId);
        return Json(states);
    }

    public JsonResult GetCities(int stateId)
    {
        var cities = _context.Cities.Where(s => s.Stateid == stateId);
        return Json(cities);
    }
}