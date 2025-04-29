using Microsoft.AspNetCore.Mvc;
using pizzashop.service.Interfaces;
using pizzashop.service;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.ViewModels;
using pizzashop.service.Implementations;
using pizzashop.service.Attributes;

namespace pizzashop.web.Controllers;

[CustomAuthorize]
public class TableController : Controller
{

    private readonly ITableSectionService _tableSectionService;

    public TableController(ITableSectionService tableSectionService)
    {
       _tableSectionService = tableSectionService;
    }
    [CustomAuthorize("TableAndSection", "CanView")]
    public IActionResult Table(){
        return View();
    }
    [CustomAuthorize("TableAndSection", "CanView")]
    [HttpGet]
    public async Task<IActionResult> Section(){
        var model = await _tableSectionService.GetAllSections();
        return PartialView("_SectionPartial",model);
    }
    [CustomAuthorize("TableAndSection", "CanView")]
    public async Task<IActionResult> Tables(int sectionId , int page = 1, int pageSize = 5, string search = ""){
         var (model, totalUsers, totalPages) = await _tableSectionService.GetTablesBySection(sectionId , page, pageSize, search);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;
        if(pageSize == 0) return Json(new { data = model });
        return PartialView("_TablePartial",model);
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
    public async Task<IActionResult> AddSection([FromBody] SectionViewModel model){
        var existingSection = await _tableSectionService.GetSectionByName(model);
        if (existingSection != null)
        {
            return Json(new { success = false, message = "Section with this name already exists" });
        }
        await _tableSectionService.AddSection(model);
        return Json(new { success = true });
    }

    public async Task<int> FirstSectionId(){
        return await _tableSectionService.FirstSectionId();
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
    public async Task<SectionViewModel> EditSectionById(int sectionId){
        var model = await _tableSectionService.GetSectionById(sectionId);
        return model;
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> EditSectionById([FromBody] SectionViewModel model){
        var existingSection = await _tableSectionService.GetSectionByName(model);
        if (existingSection != null)
        {
            return Json(new { success = false, message = "Section with this name already exists" });
        }
        await _tableSectionService.EditSection(model);
        return Json(new { success = true, message = "Section Updated Successfully" });
    }
    [CustomAuthorize("TableAndSection", "CanDelete")]
    [HttpPost]
     public async Task<JsonResult> DeleteSection(int sectionId){
        var (Success, Message) = await _tableSectionService.DeleteSection(sectionId);
        return Json(new{success = Success,message = Message});
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
    [HttpGet]
    public async Task<IActionResult> AddNewTable(int sectionId){
        var model = await _tableSectionService.GetTableviewModel(sectionId);
        return PartialView("_AddTablePartial" , model);
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
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
    [CustomAuthorize("TableAndSection", "CanDelete")]
    [HttpPost]
     public async Task<JsonResult> DeleteTable(int tableId){
        var (Message,Success) = await _tableSectionService.DeleteTable(tableId);
        return Json(new{success = Success,message=Message});
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
    public async Task<IActionResult> editTable(int tableId){

        var model = await _tableSectionService.GetTableById(tableId);
        return PartialView("_EditTablePartial" , model);
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
    [HttpPost]
    public async Task<IActionResult> editTable(TableViewModel tableViewModel){
        var existingTable = await _tableSectionService.GetTableByName(tableViewModel);
        if (existingTable != null)
        {
            return Json(new { success = false, message = "Table with this name already exists", code = 0 });
        }
        var (Message, Success) = await _tableSectionService.EditTable(tableViewModel);
        return Json(new{success = Success,message = Message,code = 1});
    }
    [CustomAuthorize("TableAndSection", "CanAddEdit")]
    [HttpPost]
    public async Task UpdateSectionOrder(List<int> sortOrder){
        await _tableSectionService.UpdateSectionSortOrder(sortOrder);
    }
    [CustomAuthorize("TableAndSection", "CanDelete")]
    [HttpPost]
    public async Task<JsonResult> MassDeleteTables([FromBody] List<int> selectedIds)
    {
       if (selectedIds == null || !selectedIds.Any())
    {
        return Json(new { success = false, message = "No tables selected for deletion" });
    }
    
    var (success, message) = await _tableSectionService.DeleteMultipleTables(selectedIds);
    
    return Json(new { success, message });
    }

    
}