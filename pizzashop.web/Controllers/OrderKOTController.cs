using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Attributes;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using Microsoft.AspNetCore.SignalR;
using pizzashop.Hubs;

namespace pizzashop.web.Controllers;
public class OrderKOTController : Controller
{
    private readonly IKOTService _KotService;

     private readonly IHubContext<KOTHub> _kotHubContext;

    public OrderKOTController(IKOTService kOTService, IHubContext<KOTHub> kotHubContext)
    {
        _KotService = kOTService;
        _kotHubContext = kotHubContext;
    }
     [CustomAuthForApp("OrderKOT")]
    public async Task<IActionResult> KOT()
    {
        KOTViewModel kOTViewModel = await _KotService.GetKOTViewModel();
        return View(kOTViewModel);
    }

     [CustomAuthForApp("OrderKOT")]
    [HttpGet]
    public async Task<IActionResult> GetKOTOrdersPartial(string categoryId, string status, int page = 1, int itemsPerPage = 8)
    {
        try
        {
            var (orders, totalOrders) = await _KotService.GetKOTOrders(categoryId, status, page, itemsPerPage);

            ViewBag.CurrentPage = page;
        ViewBag.PageSize = itemsPerPage;
        ViewBag.TotalPages = totalOrders;
        ViewBag.hasMore = page * itemsPerPage < totalOrders;

            return PartialView("_KOTOrdersPartial", orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

     [CustomAuthForApp("OrderKOT")]
    [HttpGet]
    public async Task<IActionResult> GetKOTOrders(string categoryId, string status, int page = 1, int itemsPerPage = 8)
    {
        try
        {
            var orders = await _KotService.GetKOTOrders(categoryId, status, page, itemsPerPage);
            return Json(orders);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, ex.Message);
        }
    }

     [CustomAuthForApp("OrderKOT")]
    [HttpPost]
    public async Task<IActionResult> UpdatePreparedItems([FromBody] UpdateKOTviewModel model)
    {
        if (model.Items == null || !model.Items.Any())
            return BadRequest("No items provided.");

        var orderId = await _KotService.UpdatePreparedQuantities(model.Items , model.Status);
        await _kotHubContext.Clients.All.SendAsync("updateOrderItems" , orderId);
        return Ok(new { message = "Items updated successfully." });
    }


}