using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;

namespace pizzashop.web.Controllers;

public class DashboardController : Controller
{
    private readonly IProfileService _ProfileService;

    public DashboardController(IProfileService ProfileService)
    {
        _ProfileService = ProfileService;
    }
    public IActionResult Dashboard()
    {
        return View();
    }

    public async Task<IActionResult> Profile()
    {
        var userData = SessionUtils.GetUser(HttpContext);
        var id = userData.Id;
        var model = await _ProfileService.GetProfileData((int)id);
        
        return View(model);
    }
}