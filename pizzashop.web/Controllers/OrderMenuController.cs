using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

public class OrderMenuController : Controller
{
    private readonly IMenuService _menuService;

    public OrderMenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }
    public async Task<ActionResult> Menu()
    {
        var categories = await _menuService.GetAllCategories();
        return View(categories);
    }
    public async Task<ActionResult> OrderCard(int orderId)
    {
        return PartialView("_OrderDetailPartial");
    }

    public async Task<IActionResult> GetMenuItems(string categoryId, string searchText)
    {
        var menuItems = await _menuService.GetMenuItemsAsync(categoryId, searchText);
        return PartialView("_ItemCardPartial", menuItems);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavorite(int itemId, bool isFavorite)
    {
        var result = await _menuService.ToggleFavoriteAsync(itemId, isFavorite);
        return Json(new { success = result });
    }

    public async Task<IActionResult> GetItemModifiers(int itemId){
        var itemModifierMapping = await _menuService.GetItemModifierGroupsAsync(itemId);
         var item = await _menuService.GetEditItemDetails(itemId);
         if(itemModifierMapping.Count() == 0){
         return Json(0);
         }

        return PartialView("_itemModifiersPartial", itemModifierMapping );
    }
}
