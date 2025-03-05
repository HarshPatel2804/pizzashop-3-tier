using Microsoft.AspNetCore.Mvc;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

public class MenuController : Controller
{
     private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }
    public async Task<IActionResult> Menu()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Category(){
        var model = await _menuService.GetAllCategories();
        return PartialView("_CategoryPartial",model);
    }

    [HttpPost]
    public async Task<IActionResult> Category([FromBody] CategoryViewModel model){
        Console.WriteLine(model.Categoryname + "name");
        await _menuService.AddCategory(model);
        return RedirectToAction("Menu","Menu");
    }
}
