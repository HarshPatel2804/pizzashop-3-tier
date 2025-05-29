using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pizzashop.repository.ViewModels;
using pizzashop.service.Attributes;
using pizzashop.service.Interfaces;

namespace pizzashop.web.Controllers;

[CustomAuthForApp("OrderTable")]
public class OrderTableController : Controller
{
    private readonly ITableSectionService _tableSectionService;

    private readonly IWaitingTokenService _waitingTokenService;

    private readonly IOrderService _orderService;

    private readonly ICustomerService _customerService;

    public OrderTableController(ITableSectionService tableSectionService, IWaitingTokenService waitingTokenService, IOrderService orderService, ICustomerService customerService)
    {
        _tableSectionService = tableSectionService;
        _waitingTokenService = waitingTokenService;
        _orderService = orderService;
        _customerService = customerService;
    }

    public async Task<ActionResult> Table()
    {
        return View();
    }
    public async Task<ActionResult> Sections()
    {
        var sections = await _tableSectionService.GetAllSectionsWithTablesAsync();
        return PartialView("_OrderTablePartial", sections);
    }

    [HttpGet]
    public async Task<IActionResult> AddWaitingToken(int section)
    {
        var model = await _tableSectionService.GetSectionList();
        model.Sectionid = section;
        return PartialView("_WaitingTokenPartial", model);
    }
    [HttpPost]
    public async Task<IActionResult> AddWaitingToken(WaitingtokenViewModel model)
    {
        // var customer = await _customerService.GetCustomerByEmail(model.Email);
        // if (customer != null)
        // {
        //     var hasActiveOrder = await _orderService.HasCustomerActiveOrder(customer.Customerid);
        //     if (hasActiveOrder)
        //     {
        //         return Json(new { success = false, message = "Customer already has an active order." });
        //     }

        //     var isInWaitingList = await _waitingTokenService.IsCustomerInWaitingList(customer.Customerid);
        //     if (isInWaitingList)
        //     {
        //         return Json(new { success = false, message = "Customer is already in the waiting list." });
        //     }
        // }

        var (Success , Message) = await _waitingTokenService.SaveWaitingToken(model);
        return Json(new { success = Success, message = Message });
    }

    public async Task<IActionResult> GetWaitingDetails([FromQuery] List<int> section)
    {
        var tokens = await _waitingTokenService.GetAllWaitingTokens(section);

        var model = new AssignTablePageViewModel
        {
            WaitingCustomers = (List<WaitingtokenViewModel>)tokens,
            AssignTableForm = new AssignTableViewModel()
        };
        model.AssignTableForm.Sections = await _tableSectionService.GetSections();
        return PartialView("_AssignTablePartial", model);
    }

    public async Task<IActionResult> AssignTable([FromBody] AssignTableViewModel model){

        // var customer = await _customerService.GetCustomerByEmail(model.Email);
        // if (customer != null)
        // {
        //     var hasActiveOrder = await _orderService.HasCustomerActiveOrder(customer.Customerid);
        //     if (hasActiveOrder)
        //     {
        //         return Json(new { success = false, message = "Customer already has an active order." });
        //     }

        //     var isInWaitingList = await _waitingTokenService.IsCustomerInWaitingList(customer.Customerid);
        //     if (model.Waitingtokenid == null && isInWaitingList)
        //     {
        //         return Json(new { success = false, message = "Customer is already in the waiting list." });
        //     }
        // }

        // var (result , orderId) = await _tableSectionService.AssignTable(model);
        // if(result != "true"){
        //     return Json(new { success = false, message = result });
        // }

        // return Json(new { success = true, message = "Table Assigned Successfully", orderid = orderId});

        var (success, message , orderId) = await _tableSectionService.AssignTable(model);
        if(success)
        {
            return Json(new { success = true, message = "Table Assigned Successfully", orderid = orderId });
        }
        else
        {
            return Json(new { success = false, message = message });
        }
    }

    
}
