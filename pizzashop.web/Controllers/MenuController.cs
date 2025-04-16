using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

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
        if (string.IsNullOrEmpty(model.Categoryname))
        {
            return Json(new { success = false, message = "Category name is required" });
        }
        var existingCategory = await _menuService.GetCategoryByName(model);
        if (existingCategory != null)
        {
            return Json(new { success = false, message = "Category with this name already exists" });
        }
        Console.WriteLine(model.Categoryname + "name");
        await _menuService.AddCategory(model);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> ModifierGroup()
    {
        var model = await _menuService.GetAllmodifiergroups();
        return PartialView("_ModifierGrouppartial", model);
    }

    public async Task<IActionResult> Modifiers(int ModifierGroupId, int page = 1, int pageSize = 5, string search = "")
    {
        var (model, totalUsers, totalPages) = await _menuService.GetModifiersByGroup(ModifierGroupId, page, pageSize, search);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;
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
    public async Task<IActionResult> AddNewModifier()
    {
        var model = await _menuService.GetModifierGroups();
        return PartialView("_AddNewModifier", model);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewItem(AddEditItemViewModel addEditItemViewModel, IFormFile ProfileImage)
    {
        var jsonString = Request.Form["SelectedModifierGroups"].ToString();
        Console.WriteLine($"Raw JSON received: {jsonString}");
        if (!string.IsNullOrEmpty(jsonString))
        {
            int arrayStartIndex = jsonString.IndexOf('[');
            if (arrayStartIndex == -1)
            {
                return Json(new { success = false, message = "Invalid JSON format" });
            }

            string cleanedJson = jsonString.Substring(arrayStartIndex);

            var selectedModifierGroups = JsonConvert.DeserializeObject<List<ItemModifierGroupMapping>>(cleanedJson);
            addEditItemViewModel.SelectedModifierGroups = selectedModifierGroups;
        }
        if (addEditItemViewModel.SelectedModifierGroups != null)
        {
            Console.WriteLine(addEditItemViewModel.SelectedModifierGroups);
            // Process the selected modifier groups
            foreach (var group in addEditItemViewModel.SelectedModifierGroups)
            {
                Console.WriteLine(group.Modifiergroupid + "id");
                Console.WriteLine($"Modifier Group ID: {group.Modifiergroupid}, Min Selection: {group.Minselectionrequired}, Max Selection: {group.Maxselectionallowed}");
            }
        }
        var existingItem = await _menuService.GetItemByName(addEditItemViewModel);
        if (existingItem != null)
        {
            return Json(new { success = false, message = "Item with this name already exists" });
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
        var jsonString = Request.Form["SelectedModifierGroups"].ToString();
        Console.WriteLine($"Raw JSON received: {jsonString}");
        if (!string.IsNullOrEmpty(jsonString))
        {
            int arrayStartIndex = jsonString.IndexOf('[');
            if (arrayStartIndex == -1)
            {
                return Json(new { success = false, message = "Invalid JSON format" });
            }

            // Extract the JSON array substring
            string cleanedJson = jsonString.Substring(arrayStartIndex);

            // Deserialize the cleaned JSON
            var selectedModifierGroups = JsonConvert.DeserializeObject<List<ItemModifierGroupMapping>>(cleanedJson);
            addEditItemViewModel.SelectedModifierGroups = selectedModifierGroups;
        }
        var existingItem = await _menuService.GetItemByName(addEditItemViewModel);
        if (existingItem != null)
        {
            return Json(new { success = false, message = "Item with this name already exists" });
        }
        await _menuService.EditItemAsync(addEditItemViewModel, ProfileImage);
        return Json(new { success = true, message = "added successfully" });
    }

    public async Task<CategoryViewModel> EditCategoryById(int categoryId)
    {
        var model = await _menuService.GetCategoryById(categoryId);

        return model;
    }

    [HttpPost]
    public async Task<IActionResult> EditCategoryById([FromBody] CategoryViewModel model)
    {
        if (string.IsNullOrEmpty(model.Categoryname))
        {
            return Json(new { success = false, message = "Category name is required" });
        }
        var existingCategory = await _menuService.GetCategoryByName(model);
        if (existingCategory != null)
        {
            return Json(new { success = false, message = "Category with this name already exists" });
        }
        await _menuService.EditCategory(model);
        return Json(new { success = true });
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
    public async Task DeleteModifier(int modifierId, int modifierGroupId)
    {
        Console.WriteLine(modifierId + "id");
        await _menuService.DeleteModifier(modifierId, modifierGroupId);
    }

    [HttpGet]
    public async Task<IActionResult> GetModifiers(int id)
    {

        var modifiers = await _menuService.GetModifiersBymodifierGroup(id);
        return Json(new { success = true, modifiers });
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

    [HttpGet("Menu/GetItemModifierGroups/{itemId}")]
    public async Task<IActionResult> GetItemModifierGroups(int itemId)
    {
        var modifierGroups = await _menuService.GetItemModifierGroupsAsync(itemId);
        return Ok(modifierGroups);
    }

    [HttpGet]
    public async Task<IActionResult> Modifierlist(int page = 1, int pageSize = 5, string search = "")
    {

        var (modifiers, totalModifiers, totalPages) = await _menuService.GetModifierList(page, pageSize, search);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalModifiers;
        ViewBag.TotalPages = totalPages;

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
            ? PartialView("_AddExistingModifier", modifiers)
            : View(modifiers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateModifierGroup([FromBody] ModifierGroupViewModel model)
    {
        if (string.IsNullOrEmpty(model.Modifiergroupname))
        {
            return Json(new { success = false, message = "Modifier group name is required" });
        }
        var existingGroup = await _menuService.GetModifierGroupByName(
                    model.Modifiergroupname, 0);

        if (existingGroup != null)
        {
            return Json(new { success = false, message = "A modifier group with this name already exists" });
        }
        int modifierGroupId = await _menuService.AddModifierGroup(model);
        return Json(new { success = true, ID = modifierGroupId });
    }
    [HttpPost]
    public async Task<IActionResult> UpdateModifierGroup([FromBody] ModifierGroupViewModel model)
    {
        if (string.IsNullOrEmpty(model.Modifiergroupname))
        {
            return Json(new { success = false, message = "Modifier group name is required" });
        }
        var existingGroup = await _menuService.GetModifierGroupByName(
                    model.Modifiergroupname, model.Modifiergroupid);

        if (existingGroup != null)
        {
            return Json(new { success = false, message = "A modifier group with this name already exists" });
        }
        int modifierGroupId = await _menuService.EditModifierGroup(model);
        return Json(new { success = true, ID = modifierGroupId });
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierGroupDetails(int modifierGroupId)
    {
        var model = await _menuService.GetSelectedModifiers(modifierGroupId);
        return Json(new { success = true, Data = model });
    }

    [HttpPost]
    public async Task<IActionResult> SaveModifier(ModifierViewModel model)
    {
        var jsonString = Request.Form["SelectedModifierGroups"];
        Console.WriteLine($"Raw value received: {jsonString}");

        if (!string.IsNullOrEmpty(jsonString))
        {
            var values = jsonString.ToString().Split(',');
            model.SelectedModifierGroups = values.Select(int.Parse).ToList();
        }
        else
        {
            return Json(new { success = false, message = "Please select at least one modifier group" });
        }
        var existingModifier = await _menuService.GetModifierByName(model);
        if (existingModifier != null)
        {
            return Json(new { success = false, message = "Modifier with this name already exists" });
        }
        int modifierId = await _menuService.SaveModifier(model);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierDetails(int modifierId)
    {
        var model = await _menuService.GetModifierDetails(modifierId);
        return PartialView("_EditModifier", model);
    }

    [HttpPost]
    public async Task<IActionResult> EditModifier(ModifierViewModel model)
    {
        var jsonString = Request.Form["SelectedModifierGroups"];
        Console.WriteLine($"Raw value received: {jsonString}");

        if (!string.IsNullOrEmpty(jsonString))
        {
            var values = jsonString.ToString().Split(',');
            model.SelectedModifierGroups = values.Select(int.Parse).ToList();
        }
        else
        {
            return Json(new { success = false, message = "Please select at least one modifier group" });
        }
        var existingModifier = await _menuService.GetModifierByName(model);
        if (existingModifier != null)
        {
            return Json(new { success = false, message = "Modifier with this name already exists" });
        }
        await _menuService.EditModifier(model);
        return Json(new { success = true, modifierId = model.Modifierid });
    }

    [HttpPost]
    public async Task<JsonResult> MassDeleteItems([FromBody] List<int> selectedIds)
    {
        await _menuService.DeleteMultipleItems(selectedIds);
        return Json(new { success = true, message = "Items deleted successfully" });
    }

    [HttpPost]
    public async Task<JsonResult> MassDeleteModifiers(List<int> selectedIds, int modifierGroupid)
    {
        await _menuService.DeleteMultipleModifiers(selectedIds, modifierGroupid);
        return Json(new { success = true, message = "Modifiers deleted successfully" });
    }

    [HttpPost]
    public async Task UpdateCategoryOrder(List<int> sortOrder){
        await _menuService.UpdateCategorySortOrder(sortOrder);
    }

    [HttpPost]
    public async Task UpdateModifierGroupOrder(List<int> sortOrder){
        await _menuService.UpdateModifierGroupSortOrder(sortOrder);
    }

    public async Task<int> FirstCategoryId(){
        return await _menuService.FirstCategoryId();
    }

    [HttpPost]
    public async Task<JsonResult> DeleteModifierGroup(int modifierGroupid){
        await _menuService.DeleteModifiergroup(modifierGroupid);
         return Json(new { success = true, message = "Modifier Group deleted successfully" });
    }

}
