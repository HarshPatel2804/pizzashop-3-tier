using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;
using pizzashop.service.Attributes;
using pizzashop.service.Constants;

namespace pizzashop.web.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;
    private readonly PizzaShopContext _context;

    public CustomerController(ICustomerService customerService, PizzaShopContext context)
    {
        _customerService = customerService;
        _context = context;
    }
    public IActionResult Customer()
    {
        return View();
    }

    public async Task<IActionResult> CustomerList(int page = 1, int pageSize = 5, string search = "", string sortColumn = "", string sortOrder = "", DateTime? fromDate = null, DateTime? toDate = null)
    {

        var (customers, totalUsers, totalPages) = await _customerService.GetPaginatedCustomersAsync(page, pageSize, search, sortColumn, sortOrder, fromDate, toDate);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;

        return PartialView("_CustomerList", customers);
    }

    public async Task<IActionResult> CustomerHistory(int customerId)
    {
        var model = await _customerService.GetCustomerHistory(customerId);
        return PartialView("_CustomerHistory", model);
    }

    [HttpGet]
    public async Task<IActionResult> ForExportExcel(string searchString = "", DateTime? fromDate = null, DateTime? toDate = null)
    {
        var (fileName, fileContent) = await _customerService.GenerateCustomerExcel(searchString, fromDate, toDate);
        return File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerByEmail(string email)
    {
        var customer = await _customerService.GetCustomerByEmail(email);
        if (customer == null)
        {
            return Json(new { success = false });
        }

        return Json(new
        {
            success = true,
            data = new
            {
                customername = customer.Customername,
                phoneno = customer.Phoneno
            }
        });
    }
}