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
    [CustomAuthorize("TaxAndFee", "CanView")]
    public IActionResult Tax()
    {
        return View("Tax");
    }

    [CustomAuthorize("TaxAndFee", "CanView")]
     public async Task<IActionResult> TaxTable(int page = 1, int pageSize = 5, string search = "")
    {
        var (model, totalTaxes, totalPages) = await _taxService.GetTaxes(page, pageSize, search);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalTaxes;
        ViewBag.TotalPages = totalPages;
        return PartialView("_TaxPartial",model);
    }

    [CustomAuthorize("TaxAndFee", "CanAddEdit")]
   [HttpGet]
    public async Task<IActionResult> GetTaxModal(int? id = null)
    {
        TaxViewModel viewModel;
        
        if (id.HasValue && id.Value > 0)
        {
            viewModel = await _taxService.GetTaxById(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }
        }
        else
        {
            viewModel = await _taxService.PrepareNewTaxViewModel();
        }
        
        return PartialView("_AddTax", viewModel);
    }

    [CustomAuthorize("TaxAndFee", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> SaveTax(TaxViewModel viewModel)
    {
        Console.WriteLine("Save");
        bool success;

        if(viewModel.TaxTypeId == 1 && Int32.Parse(viewModel.Taxvalue) > 100){
             return Json(new { success = false, message = "Tax percentage can not be more than 100" , code = 1 });
        }
        
        var existingTable = await _taxService.GetTaxByName(viewModel);
        if (existingTable != null)
        {
            return Json(new { success = false, message = "Tax with this name already exists"});
        }
        if (viewModel.Taxid > 0)
        {
            success = await _taxService.UpdateTax(viewModel);
        }
        else
        {
            success = await _taxService.CreateTax(viewModel);
        }

        return Json(new { 
            success, 
            message = success 
                ? viewModel.Taxid > 0 ? "Tax updated successfully." : "Tax created successfully." 
                : "Failed to save tax."
        });
    }

    [CustomAuthorize("TaxAndFee", "CanDelete")]
    [HttpPost]
    public async Task<IActionResult> DeleteTax(int id)
    {
        var success = await _taxService.DeleteTax(id);
        
        return Json(new { 
            success, 
            message = success ? "Tax deleted successfully." : "Failed to delete tax." 
        });
    }


}
