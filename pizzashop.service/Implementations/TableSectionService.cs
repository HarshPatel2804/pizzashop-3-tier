using Microsoft.AspNetCore.Mvc;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class TableSectionService : ITableSectionService
{
    private readonly ITableSectionRepository _tableSectionRepository;
    private readonly ICustomerService _customerService;

    private readonly IOrderService _orderService;

    private readonly IWaitingTokenService _waitingTokenService;

    public TableSectionService(ITableSectionRepository tableSectionRepository, ICustomerService customerService, IOrderService orderService, IWaitingTokenService waitingTokenService)
    {
        _tableSectionRepository = tableSectionRepository;
        _customerService = customerService;
        _orderService = orderService;
        _waitingTokenService = waitingTokenService;
    }

    public async Task<List<SectionViewModel>> GetAllSections()
    {
        var model = await _tableSectionRepository.GetAllSetionsAsync();

        var viewModel = model.Select(u => new SectionViewModel
        {
            Sectionid = u.Sectionid,
            Sectionname = u.Sectionname,
            Description = u.Description

        }).ToList();

        return viewModel;

    }

    public async Task<(List<TableViewModel> tableModel, int totalTables, int totalPages)> GetTablesBySection(int sectionId, int page, int pageSize, string search)
    {
        var (model, totalTables) = await _tableSectionRepository.GetTablesBySectionAsync(sectionId, page, pageSize, search);

        int totalPages = (int)System.Math.Ceiling((double)totalTables / pageSize);

        var tableModel = model.Select(u => new TableViewModel
        {
            Tableid = u.Tableid,
            Tablename = u.Tablename,
            Tablestatus = u.Tablestatus,
            Capacity = u.Capacity,
            Sectionid = u.Sectionid
        }).ToList();

        return (tableModel, totalTables, totalPages);
    }

    public async Task AddSection(SectionViewModel model)
    {
        var section = new Section
        {
            Sectionname = model.Sectionname,
            Description = model.Description,
        };
        await _tableSectionRepository.AddSectionAsync(section);
    }

    public async Task<SectionViewModel> GetSectionById(int sectionId)
    {
        var model = await _tableSectionRepository.GetSectionById(sectionId);

        var viewModel = new SectionViewModel
        {
            Sectionid = model.Sectionid,
            Sectionname = model.Sectionname,
            Description = model.Description
        };

        return viewModel;
    }

    public async Task EditSection(SectionViewModel model)
    {
        // Console.WriteLine(model.Description + "Description");
        var section = await _tableSectionRepository.GetSectionById(model.Sectionid);
        section.Sectionname = model.Sectionname;
        section.Description = model.Description;
        await _tableSectionRepository.EditSectionAsync(section);
    }

   public async Task<(bool Success, string Message)> DeleteSection(int sectionId)
{
    var areTablesOccupied = await _tableSectionRepository.AnyTableOccupied(sectionId);
    
    if (areTablesOccupied)
    {
        return (false, "Cannot delete section because some tables are currently occupied.");
    }
    
    var section = await _tableSectionRepository.GetSectionById(sectionId);
    if (section == null)
    {
        return (false, "Section not found.");
    }
    
    section.Isdeleted = true;
    await _tableSectionRepository.EditSectionAsync(section);
    await _tableSectionRepository.DeleteTablesBySectionAsync(sectionId);
    
    return (true, "Section deleted successfully.");
}

    public async Task<TableViewModel> GetSections()
    {
        var model = new TableViewModel
        {
            Sections = await _tableSectionRepository.GetSectionListAsync()
        };
        return model;
    }

    public async Task<WaitingtokenViewModel> GetSectionList()
    {
        var model = new WaitingtokenViewModel
        {
            Sections = await _tableSectionRepository.GetSectionListAsync()
        };
        return model;
    }

    public async Task AddTable(TableViewModel model)
    {
        var Table = new Table
        {
            Tablename = model.Tablename,
            Tablestatus = model.Tablestatus,
            Sectionid = model.Sectionid,
            Capacity = model.Capacity
        };
        await _tableSectionRepository.AddTableAsync(Table);
    }

    public async Task<(string message , bool success)> DeleteTable(int tableId)
    {
        var table = await _tableSectionRepository.GetTableById(tableId);
        if(table.Tablestatus == pizzashop.repository.Models.tablestatus.Occupied){
            return ("Cannot delete this table because table is occupied" , false);
        }
        await _tableSectionRepository.DeleteTableAsync(tableId);
        return ("Table deleted successfully" , true);
    }

    public async Task<TableViewModel> GetTableById(int tableId)
    {
        var table = await _tableSectionRepository.GetTableById(tableId);
        var model = new TableViewModel
        {
            Tableid = table.Tableid,
            Tablename = table.Tablename,
            Tablestatus = table.Tablestatus,
            Capacity = table.Capacity,
            Sectionid = table.Sectionid,
            Sections = await _tableSectionRepository.GetSectionListAsync()
        };
        return model;
    }

    public async Task<(string message , bool success)> EditTable(TableViewModel tableViewModel)
    {
        var table = await _tableSectionRepository.GetTableById(tableViewModel.Tableid);
        Console.WriteLine(table.Tablestatus);
        if(table.Tablestatus == pizzashop.repository.Models.tablestatus.Occupied){
            return ("Cannot edit this table because table is occupied" , false);
        }

        Console.WriteLine(table.Tablestatus);
        table.Tablename = tableViewModel.Tablename;
        table.Capacity = tableViewModel.Capacity;
        table.Tablestatus = tableViewModel.Tablestatus;
        table.Sectionid = tableViewModel.Sectionid;

        await _tableSectionRepository.EditTableAsync(table);
        return ("Table edited successfully" , true);
    }

    public async Task UpdateSectionSortOrder(List<int> sortOrder)
    {
        await _tableSectionRepository.UpdateSortOrderOfSection(sortOrder);
    }

    public async Task<(bool success, string message)> DeleteMultipleTables(List<int> tableIds)
    {
        bool hasOccupiedTables = await _tableSectionRepository.AreTablesOccupied(tableIds);
    
    if (hasOccupiedTables)
    {
        return (false, "Cannot delete tables because some tables are occupied");
    }
    
    await _tableSectionRepository.MassDeleteTable(tableIds);
    return (true, "Tables deleted successfully");
    }

    public async Task<Table> GetTableByName(TableViewModel model)
    {
        return await _tableSectionRepository.GetTableByName(model);
    }

    public async Task<int> FirstSectionId()
    {
        return await _tableSectionRepository.GetSectionIdWithLeastOrderField();
    }

    public async Task<List<OrderSectionViewModel>> GetAllSectionsWithTablesAsync()
    {
        var sections = await _tableSectionRepository.GetAllSectionsWithTablesAndOrdersAsync();
        return sections.Select(MapSectionToViewModel).ToList();
    }

    private OrderSectionViewModel MapSectionToViewModel(Section section)
    {
        var viewModel = new OrderSectionViewModel
        {
            SectionId = section.Sectionid,
            SectionName = section.Sectionname,
            Description = section.Description,
            Tables = section.Tables.Select(t => new OrderTableViewModel
            {
                TableId = t.Tableid,
                TableName = t.Tablename,
                Capacity = t.Capacity,
                Status = DetermineTableStatus(t),
                CurrentOrderAmount = GetCurrentOrderAmount(t),
                NumberOfPersons = GetNumberOfPersons(t),
                OrderDate = t.Ordertables
                                .Where(ot => ot.Order != null)
                                .OrderByDescending(ot => ot.Order.Orderdate)
                                .Select(ot => ot.Order.Orderdate)
                                .FirstOrDefault()
            }).ToList(),
            AvailableCount = section.Tables.Count(t => DetermineTableStatus(t) == TableViewStatus.Available),
            AssignedCount = section.Tables.Count(t => DetermineTableStatus(t) == TableViewStatus.Assigned),
            RunningCount = section.Tables.Count(t => DetermineTableStatus(t) == TableViewStatus.Running),
        };

        return viewModel;
    }

    private TableViewStatus DetermineTableStatus(Table table)
    {
        bool hasInProgressOrders = table.Ordertables.Any(ot => ot.Order.OrderStatus == orderstatus.InProgress || ot.Order.OrderStatus == orderstatus.Served);
        bool hasPendingOrders = table.Ordertables.Any(ot => ot.Order.OrderStatus == orderstatus.Pending);

        if (hasInProgressOrders)
            return TableViewStatus.Running;
        else if (hasPendingOrders)
            return TableViewStatus.Assigned;

        return TableViewStatus.Available;
    }

    private decimal GetCurrentOrderAmount(Table table)
    {
        var activeOrder = table.Ordertables
            .Select(ot => ot.Order)
            .Where(o => o.OrderStatus == orderstatus.InProgress || o.OrderStatus == orderstatus.Pending || o.OrderStatus == orderstatus.Served)
            .OrderByDescending(o => o.Orderdate)
            .FirstOrDefault();

        return activeOrder?.Totalamount ?? 0;
    }

    private int GetNumberOfPersons(Table table)
    {
        var activeOrder = table.Ordertables
            .Select(ot => ot.Order)
            .Where(o => o.OrderStatus == orderstatus.InProgress || o.OrderStatus == orderstatus.Pending || o.OrderStatus == orderstatus.Served)
            .OrderByDescending(o => o.Orderdate)
            .FirstOrDefault();

        return activeOrder?.Noofperson ?? 0;
    }

    public async Task<string> AssignTable(AssignTableViewModel model)
    {
        int totalCapacity = 0;
        foreach (int tableId in model.selectedTableIds)
        {
            var tableDetails = await GetTableById(tableId);
            totalCapacity += (int)tableDetails.Capacity;
        }

        if (totalCapacity < model.Noofpeople)
        {
            var result = $"Selected tables don't have enough capacity. Required: {model.Noofpeople}, Available: {totalCapacity}";
            return result;
        }
        //Add or update Customer details
        var customer = await _customerService.GetCustomerByEmail(model.Email);
        int customerId = 0;
        var CustomerModel = new Customer
        {
            Customername = model.Customername,
            Email = model.Email,
            Phoneno = model.Phoneno,
        };
        if (customer == null)
        {
            customerId = await _customerService.AddCustomer(CustomerModel);
        }
        else
        {
            customerId = await _customerService.UpdateCustomer(CustomerModel);
        }

        //Create Order
        int orderId = await _orderService.createOrderbycustomerId(customerId);

        //Order Table mapping
        List<Ordertable> orderTables = new List<Ordertable>();
        foreach (int tableId in model.selectedTableIds)
        {
            orderTables.Add(new Ordertable
            {
                Orderid = orderId,
                Tableid = tableId
            });

            //update Table status to occupied
            await _tableSectionRepository.UpdateTableStatusToOccupied(tableId);
        }

        await _tableSectionRepository.AddOrderTables(orderTables);

        if (model.Waitingtokenid != null)
        {
            await _waitingTokenService.WaitingToAssign((int)model.Waitingtokenid);
        }

        return "true";
    }
}
