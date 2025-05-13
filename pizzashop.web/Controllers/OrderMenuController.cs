using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using pizzashop.service;
using pizzashop.service.Attributes;


namespace pizzashop.web.Controllers;

[CustomAuthForApp("OrderMenu")]
public class OrderMenuController : Controller
{
    private readonly IMenuService _menuService;

    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;
    private readonly IJwtService _JwtService;

    public OrderMenuController(IJwtService JwtService,IMenuService menuService, IOrderService orderService, ICustomerService customerService)
    {
        _menuService = menuService;
        _orderService = orderService;
        _customerService = customerService;
        _JwtService = JwtService;
    }
    
    public async Task<ActionResult> Menu()
    {
        var categories = await _menuService.GetAllCategories();
        return View(categories);
    }
    public async Task<ActionResult> OrderCard(int orderId)
    {
        var orderDetailsViewModel = await _orderService.GetOrderDetailsForViewAsync(orderId);
        if(orderDetailsViewModel == null){
            TempData["ErrorMessage"] = "Invalid Order Id or Order is Completed";
            return Ok(new { success = false});
        }
        return PartialView("_OrderDetailPartial", orderDetailsViewModel);
    }

    public IActionResult GenerateToken(int orderId)
    {
             var tokenString = _JwtService.GenerateOrderToken(orderId);
             return Ok(new { token = tokenString });
    }

    [HttpGet] 
public IActionResult GetOrderIdFromToken(string orderToken)
{
    if (string.IsNullOrEmpty(orderToken))
    {
        return BadRequest(new { success = false, message = "Order token is missing." });
    }

    var principal = _JwtService.ValidateToken(orderToken);

    if (principal == null)
    {
        // TempData["ErrorMessage"] = "Invalid or expired order token.";
        return RedirectToAction("Menu","Menu");
    }

    var orderIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "orderId");
    int.TryParse(orderIdClaim.Value, out int orderId);

    return Ok(new { success = true, orderId = orderId });
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

    public async Task<IActionResult> GetItemModifiers(int itemId)
    {
        var itemModifierMapping = await _menuService.GetItemModifierGroupsAsync(itemId);
        var item = await _menuService.GetEditItemDetails(itemId);
        if (itemModifierMapping.Count() == 0)
        {
            return Json(0);
        }

        return PartialView("_itemModifiersPartial", itemModifierMapping);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderCustomerDetails(int orderId)
    {
        var viewModel = await _customerService.GetOrderCustomerDetailsAsync(orderId);

        if (viewModel == null)
        {
            return NotFound($"Details not found for Order ID {orderId}.");
        }

        return PartialView("_CustomerDetailPartial", viewModel);
    }

    [HttpPost]
    public async Task<JsonResult> UpdateOrderCustomerDetails([FromForm] OrderMenuCustomerViewModel model)
    {
        var (Success, Message) = await _customerService.updateOrderMenuCustomer(model);
        return Json(new { success = Success, message = Message });
    }

    [HttpGet]
    public async Task<JsonResult> OrderComment(int orderId)
    {
        var order = await _orderService.GetOrderbyId(orderId);
        var comment = order.Orderwisecomment;
        return Json(new { success = true, comment });
    }

    [HttpPost]
    public async Task<JsonResult> OrderComment(int orderId, string orderWiseComment)
    {
        var order = await _orderService.GetOrderbyId(orderId);
        order.Orderwisecomment = orderWiseComment;
        await _orderService.updateOrder(order);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<JsonResult> ItemComment(int orderItemId)
    {
        var comment = await _orderService.GetItemCommentAsync(orderItemId);
        return Json(new { success = true, comment });
    }

    [HttpPost]
    public async Task<JsonResult> ItemComment(int orderItemId, string itemWiseComment)
    {
        var (Success, Message) = await _orderService.UpdateItemCommentAsync(orderItemId, itemWiseComment);
        return Json(new { success = Success, message = Message });
    }
    [HttpPost]
    public async Task<JsonResult> CompleteOrder(int orderId)
    {
        var (Success, Message) = await _orderService.CompleteOrder(orderId);
        return Json(new { success = Success, message = Message });
    }
    [HttpPost]
    public async Task<JsonResult> CancelOrder(int orderId)
    {
        var (Success, Message) = await _orderService.CancelOrder(orderId);
        if (Success)
        {
            TempData["SuccessMessage"] = Message;
        }
        else
        {
            TempData["ErrorMessage"] = Message;
        }
        return Json(new { success = Success, message = Message });
    }

    [HttpPost]
    public async Task<IActionResult> SaveRatings([FromBody] SaveRatingViewModel request)
    {
        if (request.OrderId <= 0)
        {
            TempData["ErrorMessage"] = "Invalid OrderId";
            return Json(new { success = false, message = "Invalid OrderId" });
        }

        var (Success, Message) = await _orderService.SaveCustomerReviewAsync(request);
        if (Success)
        {
            TempData["SuccessMessage"] = Message;
        }
        else
        {
            TempData["ErrorMessage"] = Message;
        }

        return Json(new { success = Success, message = Message });
    }

}
