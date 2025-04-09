using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Interfaces;
using pizzashop.service;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.ViewModels;
using pizzashop.service.Implementations;

namespace pizzashop.web.Controllers;

public class TableController : Controller
{

    private readonly ITableSectionService _tableSectionService;

    public TableController(ITableSectionService tableSectionService)
    {
       _tableSectionService = tableSectionService;
    }
    public IActionResult Table(){
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Section(){
        var model = await _tableSectionService.GetAllSections();
        return PartialView("_SectionPartial",model);
    }

    public async Task<IActionResult> Tables(int sectionId , int page = 1, int pageSize = 5, string search = ""){
         var (model, totalUsers, totalPages) = await _tableSectionService.GetTablesBySection(sectionId , page, pageSize, search);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;
        return PartialView("_TablePartial",model);
    }

    public async Task<IActionResult> AddSection([FromBody] SectionViewModel model){
        await _tableSectionService.AddSection(model);
        return Json(new { success = true });
    }

    public async Task<int> FirstSectionId(){
        return await _tableSectionService.FirstSectionId();
    }

    public async Task<SectionViewModel> EditSectionById(int sectionId){
        var model = await _tableSectionService.GetSectionById(sectionId);
        return model;
    }

    [HttpPost]
    public async Task<SectionViewModel> EditSectionById([FromBody] SectionViewModel model){
        await _tableSectionService.EditSection(model);
        return model;
    }

    [HttpPost]
     public async Task DeleteSection(int sectionId){
        await _tableSectionService.DeleteSection(sectionId);
    }

    [HttpGet]
    public async Task<IActionResult> AddNewTable(){
        var model = await _tableSectionService.GetSections();
        return PartialView("_AddTablePartial" , model);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewTable(TableViewModel model){
        Console.WriteLine("Hii" + model.Sectionid);
        var existingTable = await _tableSectionService.GetTableByName(model);
        if (existingTable != null)
        {
            return Json(new { success = false, message = "Table with this name already exists" });
        }
        await _tableSectionService.AddTable(model);
        return Json(new { success = true });
    }

    [HttpPost]
     public async Task DeleteTable(int tableId){
        Console.WriteLine(tableId + "table");
        await _tableSectionService.DeleteTable(tableId);
    }

    public async Task<IActionResult> editTable(int tableId){

        var model = await _tableSectionService.GetTableById(tableId);
        return PartialView("_EditTablePartial" , model);
    }

    [HttpPost]
    public async Task<IActionResult> editTable(TableViewModel tableViewModel){
        var existingTable = await _tableSectionService.GetTableByName(tableViewModel);
        if (existingTable != null)
        {
            return Json(new { success = false, message = "Table with this name already exists" });
        }
        await _tableSectionService.EditTable(tableViewModel);
        return Json(new{success = true,message="Table Edited Successfully"});
    }

    [HttpPost]
    public async Task UpdateSectionOrder(List<int> sortOrder){
        await _tableSectionService.UpdateSectionSortOrder(sortOrder);
    }

    [HttpPost]
    public async Task<JsonResult> MassDeleteTables([FromBody] List<int> selectedIds)
    {
       await _tableSectionService.DeleteMultipleTables(selectedIds);
        return Json(new { success = true, message = "Items deleted successfully" });
    }
}