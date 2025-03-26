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

    public DashboardController(IProfileService ProfileService , PizzaShopContext context)
    {
        _ProfileService = ProfileService;
        _context = context;
    }
    public IActionResult Dashboard()
    {
        return View();
    }

    public async Task<IActionResult> Profile()
    {
        var userData = SessionUtils.GetUser(HttpContext);
        if (userData == null)
            return RedirectToAction("index", "Home");
        var id = userData.Id;
        var model = await _ProfileService.GetProfileData((int)id);
        Console.WriteLine(model.Profileimg + "image");
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model , IFormFile ProfileImage)
    { 
            ModelState.Remove(nameof(model.Countries));
            ModelState.Remove(nameof(model.States));
            ModelState.Remove(nameof(model.Cities));
            ModelState.Remove(nameof(model.Roles));
            ModelState.Remove(nameof(model.Profileimg));
            ModelState.Remove(nameof(ProfileImage));
         if(!ModelState.IsValid) return View(model);
        await _ProfileService.UpdateProfileData(model , ProfileImage);
        
        return RedirectToAction("Profile","Dashboard");
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
        await _ProfileService.UpdatePassword(model);
        return RedirectToAction("Dashboard","Dashboard");;
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