using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderService _orderService;

    public CustomerService(ICustomerRepository customerRepository, IOrderService orderService)
    {
        _customerRepository = customerRepository;
        _orderService = orderService;
    }
    public async Task<(List<Customer> customers, int totalCustomers, int totalPages)> GetPaginatedCustomersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder, DateTime? fromDate, DateTime? toDate)
    {
        var (customers, totalCustomers) = await _customerRepository.GetPaginatedCustomersAsync(page, pageSize, search, sortColumn, sortOrder, fromDate, toDate);

        int totalPages = (int)System.Math.Ceiling((double)totalCustomers / pageSize);

        return (customers, totalCustomers, totalPages);
    }

    public async Task<Customer> GetCustomerByEmail(string Email)
    {
        return await _customerRepository.GetCustomerByEmail(Email);
    }

    public async Task<int> AddCustomer(Customer model)
    {
        return await _customerRepository.AddCustomer(model);
    }
    public async Task<int> UpdateCustomer(Customer model)
    {
        return await _customerRepository.UpdateCustomer(model);
    }

    public async Task<(bool Success, string Message)> updateOrderMenuCustomer(OrderMenuCustomerViewModel model)
    {
        var tables = await _orderService.GetOrdertables(model.OrderId);

        var totalCapacity = 0;
        foreach (var table in tables)
        {
            totalCapacity += (int)table.Capacity;
        }
        if (totalCapacity < model.Noofpeople)
        {
            return (false, $"Selected tables don't have enough capacity. Required: {model.Noofpeople}, Available: {totalCapacity}");
        }

        bool IsSameEmail = await _customerRepository.IsSameEmail(model.Email , model.CustomerId);
        bool IsSamePhone = await _customerRepository.IsSamePhone(model.Phoneno , model.CustomerId);

        if(IsSameEmail){
            return (false, "User with this email already exist!");
        }
        if(IsSamePhone){
            return (false, "User with this phone number already exist!");
        }
        Customer customer = await _customerRepository.GetCustomerById(model.CustomerId);
        customer.Email = model.Email;
        customer.Customername = model.Customername;
        customer.Phoneno = model.Phoneno;

        await UpdateCustomer(customer);

        var order = await _orderService.GetOrderbyId(model.OrderId);
        order.Noofperson = (short?)model.Noofpeople;
        await _orderService.updateOrder(order);

        return (true, "Customer Details Updates successfully!");
    }

    public async Task<CustomerViewModel> GetCustomerHistory(int customerId)
    {
        var customer = await _customerRepository.GetCustomerById(customerId);

        var orders = await _customerRepository.GetOrdersByCustomer(customerId);

        decimal maxOrder = orders.OrderByDescending(o => o.Totalamount).FirstOrDefault().Totalamount;
        decimal averageBill = orders.Average(o => o.Totalamount);
        DateTime? comingSince = orders.Count() > 0 ? orders.Min(o => o.Orderdate) : customer.Createdat;

        var model = new CustomerViewModel
        {
            Customerid = customer.Customerid,
            Email = customer.Email,
            Phoneno = customer.Phoneno,
            Visitcount = customer.Visitcount,
            Customername = customer.Customername,
            OrderHistory = orders,
            ComingSince = comingSince,
            AverageBill = averageBill,
            MaxOrder = maxOrder
        };

        return model;
    }

    public async Task<(List<Customer>, int totalOrders)> GetOrdersForExport(string searchString, DateTime? fromDate, DateTime? toDate)
    {
        return await _customerRepository.GetCustomersForExport(searchString, fromDate, toDate);
    }

    public async Task<(string fileName, byte[] fileContent)> GenerateCustomerExcel(string searchString, DateTime? fromDate, DateTime? toDate)
    {
        var (orders, totalOrders) = await GetOrdersForExport(searchString, fromDate, toDate);

        string templatePath = Path.Combine("wwwroot", "Templates", "CustomerTemplate.xlsx");

        string fileName = $"PizzaShopCustomers_{DateTime.Now:yyyyMMdd}.xlsx";

        using (var templateFile = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(templateFile);
            ISheet sheet = workbook.GetSheetAt(0);

            FillHeaderData(sheet, searchString, totalOrders, fromDate, toDate);

            FillOrderData(workbook, sheet, orders);

            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                return (fileName, exportData.ToArray());
            }
        }
    }

    private void FillHeaderData(ISheet sheet, string searchString, int recordCount, DateTime? fromDate, DateTime? toDate)
    {
        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 2, 5));
        ICell statusCell = sheet.GetRow(1).GetCell(2);
        statusCell.SetCellValue("Harsh");

        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 9, 12));
        ICell searchCell = sheet.GetRow(1).GetCell(9);
        searchCell.SetCellValue(searchString);

        string dateText = "All Time";
        if (fromDate.HasValue && toDate.HasValue)
        {
            dateText = $"{fromDate.Value.ToString("MM/dd/yyyy")} to {toDate.Value.ToString("MM/dd/yyyy")}";
        }
        else if (fromDate.HasValue)
        {
            dateText = $"From {fromDate.Value.ToString("MM/dd/yyyy")}";
        }
        else if (toDate.HasValue)
        {
            dateText = $"Until {toDate.Value.ToString("MM/dd/yyyy")}";
        }
        Console.WriteLine(dateText + " dateText");
        sheet.AddMergedRegion(new CellRangeAddress(4, 5, 2, 5));
        ICell dateCell = sheet.GetRow(4).GetCell(2);
        dateCell.SetCellValue(dateText);

        sheet.AddMergedRegion(new CellRangeAddress(4, 5, 9, 12));
        ICell recordsCell = sheet.GetRow(4).GetCell(9);
        recordsCell.SetCellValue(recordCount);
    }

    private void FillOrderData(IWorkbook workbook, ISheet sheet, IEnumerable<Customer> customers)
    {
        ICellStyle dataStyle = CreateDataCellStyle(workbook);
        ICellStyle currencyStyle = CreateCurrencyStyle(workbook, dataStyle);
        ICellStyle dateStyle = CreateDateStyle(workbook, dataStyle);

        int rowIndex = 9;
        foreach (var customer in customers)
        {
            IRow dataRow = sheet.CreateRow(rowIndex);

            CreateCell(dataRow, 0, customer.Customerid.ToString(), dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 1, 3));
            CreateCell(dataRow, 1, customer.Customername, dataStyle);
            CreateCell(dataRow, 2, "", dataStyle);
            CreateCell(dataRow, 3, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 7));
            CreateCell(dataRow, 4, customer.Email, dataStyle);
            CreateCell(dataRow, 5, "", dataStyle);
            CreateCell(dataRow, 6, "", dataStyle);
            CreateCell(dataRow, 7, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 8, 10));
            ICell dateCell = dataRow.CreateCell(8);
            dateCell.SetCellValue(customer.Customername);
            dateCell.CellStyle = dateStyle;
            CreateCell(dataRow, 9, "", dataStyle);
            CreateCell(dataRow, 10, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 11, 13));
            CreateCell(dataRow, 11, customer.Phoneno.ToString(), dataStyle);
            CreateCell(dataRow, 12, "", dataStyle);
            CreateCell(dataRow, 13, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 14, 15));
            CreateCell(dataRow, 14, customer.Totalorder.ToString(), dataStyle);
            CreateCell(dataRow, 15, "", dataStyle);

            rowIndex++;
        }

        for (int i = 0; i < 16; i++)
        {
            sheet.AutoSizeColumn(i);
        }
    }



    private ICellStyle CreateDataCellStyle(IWorkbook workbook)
    {
        ICellStyle style = workbook.CreateCellStyle();
        IFont font = workbook.CreateFont();
        font.FontName = "Arial";
        font.FontHeightInPoints = 10;

        style.SetFont(font);
        style.BorderTop = BorderStyle.Thin;
        style.BorderBottom = BorderStyle.Thin;
        style.BorderLeft = BorderStyle.Thin;
        style.BorderRight = BorderStyle.Thin;
        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;

        return style;
    }

    private ICellStyle CreateCurrencyStyle(IWorkbook workbook, ICellStyle baseStyle)
    {
        ICellStyle style = workbook.CreateCellStyle();
        style.CloneStyleFrom(baseStyle);
        IDataFormat format = workbook.CreateDataFormat();
        style.DataFormat = format.GetFormat("₹#,##0.00");
        return style;
    }

    private ICellStyle CreateDateStyle(IWorkbook workbook, ICellStyle baseStyle)
    {
        ICellStyle style = workbook.CreateCellStyle();
        style.CloneStyleFrom(baseStyle);
        IDataFormat format = workbook.CreateDataFormat();
        style.DataFormat = format.GetFormat("mm/dd/yyyy");
        return style;
    }

    private void CreateCell(IRow row, int column, string value, ICellStyle style)
    {
        ICell cell = row.CreateCell(column);
        cell.SetCellValue(value);
        cell.CellStyle = style;
    }

    public async Task<OrderMenuCustomerViewModel?> GetOrderCustomerDetailsAsync(int orderId)
    {
        var order = await _orderService.GetOrderbyId(orderId);

        if (order == null)
        {
            return null;
        }

        var customer = await _customerRepository.GetCustomerById(order.Customerid);

        if (customer == null)
        {
            return null;
        }

        var viewModel = new OrderMenuCustomerViewModel
        {
            OrderId = order.Orderid,
            CustomerId = customer.Customerid,
            Customername = customer.Customername,
            Email = customer.Email,
            Phoneno = customer.Phoneno,
            Noofpeople = order.Noofperson ?? 1
        };

        return viewModel;
    }
}
