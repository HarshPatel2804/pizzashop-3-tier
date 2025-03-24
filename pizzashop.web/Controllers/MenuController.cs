using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

[CustomAuthorize]
public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly PizzaShopContext _context;

    public MenuController(IMenuService menuService, PizzaShopContext context)
    {
        _menuService = menuService;
        _context = context;
    }
    public async Task<IActionResult> Menu()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Category()
    {
        var model = await _menuService.GetAllCategories();
        return PartialView("_CategoryPartial", model);
    }

    [HttpPost]
    public async Task<IActionResult> Category([FromBody] CategoryViewModel model)
    {
        Console.WriteLine(model.Categoryname + "name");
        await _menuService.AddCategory(model);
        return RedirectToAction("Menu", "Menu");
    }

    [HttpGet]
    public async Task<IActionResult> ModifierGroup()
    {
        var model = await _menuService.GetAllmodifiergroups();
        return PartialView("_ModifierGrouppartial", model);
    }

    public async Task<IActionResult> Modifiers(int ModifierGroupId)
    {
        var model = await _menuService.GetModifiersByGroup(ModifierGroupId);
        return PartialView("_ModifierPartial", model);
    }

    public async Task<IActionResult> Items(int categoryId, int page = 1, int pageSize = 5, string search = "")
    {
        var (model, totalUsers, totalPages) = await _menuService.GetItemsByCategory(categoryId, page, pageSize, search);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;
        return PartialView("_ItemPartial", model);
    }

    public async Task<IActionResult> AddNewItem()
    {
        var model = await _menuService.GetItemDetails();
        return PartialView("_AddItem", model);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewItem(AddEditItemViewModel addEditItemViewModel, IFormFile ProfileImage)
    {
        if (addEditItemViewModel.SelectedModifierGroups != null)
        {Console.WriteLine(addEditItemViewModel.SelectedModifierGroups);
            // Process the selected modifier groups
            foreach (var group in addEditItemViewModel.SelectedModifierGroups)
            {
                Console.WriteLine(group.Modifiergroupid + "id");
                Console.WriteLine($"Modifier Group ID: {group.Modifiergroupid}, Min Selection: {group.Minselectionrequired}, Max Selection: {group.Maxselectionallowed}");
            }
        }
        await _menuService.AddItemAsync(addEditItemViewModel, ProfileImage);
        return Json(new { success = true, message = "added successfully" });
    }

    public async Task<IActionResult> editItem(int itemId)
    {
        var model = await _menuService.GetEditItemDetails(itemId);
        return PartialView("_EditItem", model);
    }

    [HttpPost]
    public async Task<IActionResult> editItem(AddEditItemViewModel addEditItemViewModel, IFormFile ProfileImage)
    {
        await _menuService.EditItemAsync(addEditItemViewModel, ProfileImage);
        return Json(new { success = true, message = "added successfully" });
    }

    public async Task<CategoryViewModel> EditCategoryById(int categoryId)
    {
        var model = await _menuService.GetCategoryById(categoryId);

        return model;
    }

    [HttpPost]
    public async Task<CategoryViewModel> EditCategoryById([FromBody] CategoryViewModel model)
    {
        await _menuService.EditCategory(model);
        return model;
    }

    [HttpPost]
    public async Task DeleteCategory(int categoryId)
    {
        Console.WriteLine(categoryId + "id");
        await _menuService.DeleteCategory(categoryId);
    }

    [HttpPost]
    public async Task DeleteItem(int itemId)
    {
        await _menuService.DeleteItem(itemId);
    }

    [HttpPost]
    public async Task DeleteModifier(int modifierId)
    {
        Console.WriteLine(modifierId + "id");
        await _menuService.DeleteModifier(modifierId);
    }

    [HttpGet]
    public async Task<IActionResult> GetModifiers(int id)
    {
        try
        {
            // Check if modifier group exists
            var modifierGroup = await _context.Modifiergroups
                .FirstOrDefaultAsync(mg => mg.Modifiergroupid == id);

            if (modifierGroup == null)
            {
                return Json(new { success = false, message = "Modifier group not found" });
            }

            // Get all modifiers for the group
            var modifiers = await _context.Modifiers
                .Where(m => m.Modifiergroupid == id && (!m.Isdeleted.HasValue || !m.Isdeleted.Value))
                .Select(m => new
                {
                    modifierid = m.Modifierid,
                    modifiername = m.Modifiername,
                    rate = m.Rate,
                    quantity = m.Quantity,
                    unitName = m.Unit.Unitname
                })
                .ToListAsync();

            return Json(new { success = true, modifiers });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public async Task<IActionResult> AddItemModal()
    {
        var viewModel = new AddEditItemViewModel
        {
            // Populate Categories dropdown
            Category = await _context.Categories
                .Where(c => !c.Isdeleted.HasValue || !c.Isdeleted.Value)
                .Select(c => new SelectListItem
                {
                    Value = c.Categoryid.ToString(),
                    Text = c.Categoryname
                })
                .ToListAsync(),

            // Populate Units dropdown
            Units = await _context.Units
                .Select(u => new SelectListItem
                {
                    Value = u.Unitid.ToString(),
                    Text = u.Unitname
                })
                .ToListAsync(),

            // Populate ModifierGroups for dropdown
            ModifierGroups = await _context.Modifiergroups
                .Select(u => new SelectListItem
                {
                    Value = u.Modifiergroupid.ToString(),
                    Text = u.Modifiergroupname
                })
                .ToListAsync(),
        };

        return PartialView("_AddItemPartial", viewModel);
    }
}
