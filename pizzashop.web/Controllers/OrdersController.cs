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
public class OrdersController : Controller
{
    private readonly IProfileService _ProfileService;
    private readonly PizzaShopContext _context;

    public OrdersController(IProfileService ProfileService , PizzaShopContext context)
    {
        _ProfileService = ProfileService;
        _context = context;
    }
    public IActionResult Order()
    {
        return View();
    }
}