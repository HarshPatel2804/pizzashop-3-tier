using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

public class OrderTableController : Controller
{
    private readonly ITableSectionService _tableSectionService;

     private readonly IWaitingTokenService _waitingTokenService;

    public OrderTableController(ITableSectionService tableSectionService , IWaitingTokenService waitingTokenService)
    {
       _tableSectionService = tableSectionService;
       _waitingTokenService = waitingTokenService;

    }

    public async Task<ActionResult> Table(){
        return View();
    }
    public async Task<ActionResult> Sections(){
        var sections = await _tableSectionService.GetAllSectionsWithTablesAsync();
        return PartialView("_OrderTablePartial",sections);
    }

    [HttpGet]
    public async Task<IActionResult> AddWaitingToken(int section){
        var model = await _tableSectionService.GetSectionList();
        model.Sectionid = section;
        return PartialView("_WaitingTokenPartial" , model);
    }
    [HttpPost]
    public async Task<IActionResult> AddWaitingToken(WaitingtokenViewModel model){
        await _waitingTokenService.SaveWaitingToken(model);
        return Json(new{success = true,message="Waiting Token Added Successfully"});
    }
}
