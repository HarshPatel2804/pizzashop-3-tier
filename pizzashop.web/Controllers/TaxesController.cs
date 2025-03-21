using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Interfaces;
using pizzashop.service;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;

namespace pizzashop.web.Controllers;

[CustomAuthorize]
public class TaxesController : Controller
{
     private readonly ITaxService _taxService;

    public TaxesController(ITaxService taxService)
    {
       _taxService = taxService;
    }
    public IActionResult Tax()
    {
        return View("Tax");
    }

     public async Task<IActionResult> TaxTable(int page = 1, int pageSize = 5, string search = "")
    {
        var (model, totalTaxes, totalPages) = await _taxService.GetTaxes(page, pageSize, search);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalTaxes;
        ViewBag.TotalPages = totalPages;
        return PartialView("_TaxPartial",model);
    }


}
