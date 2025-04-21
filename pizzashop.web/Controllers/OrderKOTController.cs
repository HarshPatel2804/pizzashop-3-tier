using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Attributes;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;
public class OrderKOTController : Controller
{

    private readonly IKOTService _KotService;

    public OrderKOTController(IKOTService kOTService)
    {
        _KotService = kOTService;
    }
    public async Task<IActionResult> KOT()
    {
        KOTViewModel kOTViewModel = await _KotService.GetKOTViewModel();
        return View(kOTViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetKOTOrdersPartial(string categoryId, string status, int page = 1, int itemsPerPage = 8)
    {
        try
        {
            var orders = await _KotService.GetKOTOrders(categoryId, status, page, itemsPerPage);

            ViewData["HasMore"] = orders.Count == itemsPerPage;

            return PartialView("_KOTOrdersPartial", orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

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

    [HttpPost]
    public IActionResult UpdatePreparedItems([FromBody] UpdateKOTviewModel model)
    {
        if (model.Items == null || !model.Items.Any())
            return BadRequest("No items provided.");

        _KotService.UpdatePreparedQuantities(model.Items , model.Status);
        return Ok(new { message = "Items updated successfully." });
    }


}