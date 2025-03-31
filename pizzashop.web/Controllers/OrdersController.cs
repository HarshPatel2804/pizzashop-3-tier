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
    private readonly IOrderService _orderService;
    private readonly PizzaShopContext _context;

    public OrdersController(IOrderService orderService , PizzaShopContext context)
    {
        _orderService = orderService;
        _context = context;
    }
    public IActionResult Order()
    {
        return View();
    }

    public async Task<IActionResult> OrderList(int page = 1, int pageSize = 5, string search = "", string sortColumn = "", string sortOrder = "", orderstatus? status = null, DateTime? fromDate = null , DateTime? toDate = null)
    {

        var (orders, totalUsers, totalPages) = await _orderService.GetPaginatedOrdersAsync(page, pageSize, search, sortColumn, sortOrder,status,fromDate,toDate);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;

        return  PartialView("_OrderTablePartial", orders);
    }
}