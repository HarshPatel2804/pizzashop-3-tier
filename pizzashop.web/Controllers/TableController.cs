using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Interfaces;
using pizzashop.service;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.ViewModels;

namespace pizzashop.web.Controllers;

public class TableController : Controller
{
    public IActionResult Table(){
        return View();
    }
}