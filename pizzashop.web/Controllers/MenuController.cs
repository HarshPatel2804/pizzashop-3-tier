using Microsoft.AspNetCore.Mvc;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

[CustomAuthorize]
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

    [HttpGet]
    public async Task<IActionResult> ModifierGroup(){
        var model = await _menuService.GetAllmodifiergroups();
        return PartialView("_ModifierGrouppartial",model);
    }

    public async Task<IActionResult> Modifiers(int ModifierGroupId){
        var model = await _menuService.GetModifiersByGroup(ModifierGroupId);
        return PartialView("_ModifierPartial",model);
    }

    public async Task<IActionResult> Items(int categoryId){
        var model = await _menuService.GetItemsByCategory(categoryId);
        return PartialView("_ItemPartial",model);
    }

    public async Task<IActionResult> AddNewItem(){
        var model = await _menuService.GetItemDetails();
        return PartialView("_AddItem" , model);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewItem(AddEditItemViewModel addEditItemViewModel){
        await _menuService.AddItemAsync(addEditItemViewModel);
        return Json(new{success = true,message="added successfully"});
    }

    public async Task<CategoryViewModel> EditCategoryById(int categoryId){
        var model = await _menuService.GetCategoryById(categoryId);

        return model;
    }

    [HttpPost]
    public async Task<CategoryViewModel> EditCategoryById([FromBody] CategoryViewModel model){
        await _menuService.EditCategory(model);
        return model;
    }

    [HttpPost]
     public async Task DeleteCategory(int categoryId){
        Console.WriteLine(categoryId + "id");
        await _menuService.DeleteCategory(categoryId);
    }

    [HttpPost]
     public async Task DeleteItem(int itemId){
        await _menuService.DeleteItem(itemId);
    }
}
