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
using SelectPdf;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using pizzashop.Hubs;

namespace pizzashop.web.Controllers;

[CustomAuthorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly PizzaShopContext _context;
    private readonly IHubContext<OrderHub> _orderHubContext;

    public OrdersController(IOrderService orderService, PizzaShopContext context, IHubContext<OrderHub> orderHubContext)
    {
        _orderService = orderService;
        _context = context;
        _orderHubContext = orderHubContext;
    }
    [CustomAuthorize("Order", "CanView")]
    public IActionResult Order()
    {
        return View();
    }



    [CustomAuthorize("Order", "CanView")]
    public async Task<IActionResult> OrderList(int page = 1, int pageSize = 5, string search = "", string sortColumn = "", string sortOrder = "", orderstatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {

        var (orders, totalUsers, totalPages) = await _orderService.GetPaginatedOrdersAsync(page, pageSize, search, sortColumn, sortOrder, status, fromDate, toDate);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.TotalPages = totalPages;

        return PartialView("_OrderTablePartial", orders);
    }

    [CustomAuthorize("Order", "CanView")]
    [HttpGet]
    public async Task<IActionResult> ForExportExcel(string searchString = "", orderstatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var (fileName, fileContent) = await _orderService.GenerateOrderExcel(searchString, status, fromDate, toDate);
        return File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }

    public async Task<IActionResult> FororderView(int orderid)
    {
        OrderDetailsView orderDetailsView = await _orderService.GetOrderDetailsViewService(orderid);

        return PartialView("_Orderview", orderDetailsView);

    }

    [CustomAuthorize("Order", "CanView")]
    public async Task<IActionResult> ForPdfDownload(int orderid)
    {

        OrderDetailsView orderDetailsView = await _orderService.GetOrderDetailsViewService(orderid);

        var viewHtml = await RenderViewToString("OrderPdfView", orderDetailsView);

        // TO Convert HTML to PDF
        HtmlToPdf converter = new HtmlToPdf();


        // PDF style
        converter.Options.PdfPageSize = PdfPageSize.A3;
        converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
        converter.Options.MarginTop = 20;
        converter.Options.MarginBottom = 20;
        converter.Options.MarginLeft = 20;
        converter.Options.MarginRight = 20;

        // Create PDF document
        PdfDocument doc = converter.ConvertHtmlString(viewHtml, $"{Request.Scheme}://{Request.Host}");

        // Save PDF to a memory stream
        MemoryStream ms = new MemoryStream();
        doc.Save(ms);
        doc.Close();

        // Return the PDF file
        return File(ms.ToArray(), "application/pdf", "Report.pdf");

    }

    private async Task<string> RenderViewToString(string viewName, OrderDetailsView model)
    {
        using (var sw = new StringWriter())
        {
            var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"View {viewName} not found");
            }

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext);
            return sw.ToString();

        }
    }

    public async Task<IActionResult> SaveOrder([FromBody] OrderSaveViewModel model)
    {
        bool success = await _orderService.SaveOrderAsync(model);
        //In same group send message after saving order
        if (success)
        {
            //Order
            string groupName = $"Order_{model.OrderId}";
            await _orderHubContext.Clients.Group(groupName).SendAsync(
                "OrderSaved",
                model.OrderId
            );

            //KOT
            await _orderHubContext.Clients.All.SendAsync("OrderSavedKOT",model.OrderId);
        }
        return Json(new { success = success, message = "Order Saved Successfully" });
    }


    public async Task<IActionResult> GetOrderTaxIds(int orderId)
    {
        var taxIds = await _orderService.GetTaxIdsByOrderIdAsync(orderId);
        return Json(new { taxIds });

    }
}
