using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

[CustomAuthForApp("OrderWaitingList")]
public class OrderWaitingListController : Controller
{
    private readonly IWaitingTokenService _waitingTokenService;
     private readonly ITableSectionService _tableSectionService;

    public OrderWaitingListController(IWaitingTokenService waitingTokenService , ITableSectionService tableSectionService)
    {
        _waitingTokenService = waitingTokenService;
        _tableSectionService = tableSectionService;
    }
    public ActionResult WaitingList()
    {
        return View();
    }
    public async Task<ActionResult> WaitingData()
    {
        var model = await _waitingTokenService.GetWaitingData();
        return PartialView("_WaitingNavPartial", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingTokensBySection(int sectionId = 0)
    {
        var waitingTokens = await _waitingTokenService.GetWaitingTokensBySectionAsync(sectionId);
        return PartialView("_WaitingPartial", waitingTokens);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteWaitingToken(int tokenId)
    {
        if (tokenId <= 0)
        {
            return Json(new { success = false, message = "Invalid Token ID." });
        }

        bool deleted = await _waitingTokenService.RemoveWaitingTokenAsync(tokenId);
        if (deleted)
        {
            return Json(new { success = true, message = "Waiting token deleted successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Waiting token not found or could not be deleted." });
        }

    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingTokenForEdit(int tokenId)
    {
        var response = await _waitingTokenService.GetWaitingTokenForEditAsync(tokenId);
        if (response.success && response.model != null)
        {
            return PartialView("_EditWaiting", response.model);
        }

        return PartialView("_EditWaiting", new WaitingtokenViewModel { Sections = new() }); 
    }

    [HttpPost]
    public async Task<IActionResult> UpdateWaitingToken(WaitingtokenViewModel model)
    {
        var response = await _waitingTokenService.UpdateWaitingTokenDetailsAsync(model);

        return Json(new { success = response.success, message = response.message });
    }

    [HttpGet]
        public async Task<IActionResult> GetAssignTablePartial(int waitingTokenId, int? sectionId = null)
        {
            var viewModel = await _tableSectionService.GetAssignTableViewModelAsync(waitingTokenId, sectionId);
            return PartialView("_AssignTable", viewModel);
        }

    [HttpGet]
        public async Task<IActionResult> GetMultiTablesBySectionId([FromQuery] List<int> sectionId)
        {
            var tables = await _tableSectionService.GetMultiAssignTableViewModelAsync(0, sectionId);
            return Json(tables.TableList);
        }

    [HttpPost]
        public async Task<IActionResult> AssignTableFromWaiting(WaitingAssignViewModel model)
        {
             var (Message , id) = await _waitingTokenService.AssignTable(model);
             return Json(new { message = Message , orderId = id });
        }
}
