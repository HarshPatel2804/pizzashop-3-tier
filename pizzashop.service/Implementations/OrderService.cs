using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> HasCustomerActiveOrder(int customerId)
    {
        return await _orderRepository.HasCustomerActiveOrder(customerId);
    }

    public async Task<int> createOrderbycustomerId(int customerId){

        Order order = new Order{
            Customerid = customerId,
            OrderStatus = orderstatus.Pending,
            Createdat = DateTime.Now
        };
        return await _orderRepository.createOrder(order);
    }
    
    public async Task<(List<Order> orders, int totalOrders, int totalPages)> GetPaginatedOrdersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder, orderstatus? status, DateTime? fromDate, DateTime? toDate)
    {
        var (orders, totalOrders) = await _orderRepository.GetPaginatedOrdersAsync(page, pageSize, search, sortColumn, sortOrder, status, fromDate, toDate);

        int totalPages = (int)System.Math.Ceiling((double)totalOrders / pageSize);

        return (orders, totalOrders, totalPages);
    }

    public async Task<(List<Order>, int totalOrders)> GetOrdersForExport(string searchString, orderstatus? status, DateTime? fromDate, DateTime? toDate)
    {
        return await _orderRepository.GetOrdersForExport(fromDate, toDate, status, searchString);
    }

    public async Task<(string fileName, byte[] fileContent)> GenerateOrderExcel(string searchString, orderstatus? status, DateTime? fromDate, DateTime? toDate)
    {
        var (orders, totalOrders) = await GetOrdersForExport(searchString, status, fromDate, toDate);

        string templatePath = Path.Combine("wwwroot", "Templates", "OrderTemplate.xlsx");

        string fileName = $"PizzaShopOrders_{DateTime.Now:yyyyMMdd}.xlsx";

        using (var templateFile = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(templateFile);
            ISheet sheet = workbook.GetSheetAt(0);

            FillHeaderData(sheet, status, searchString, totalOrders, fromDate, toDate);

            FillOrderData(workbook, sheet, orders);

            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                return (fileName, exportData.ToArray());
            }
        }
    }

    private void FillHeaderData(ISheet sheet, orderstatus? status, string searchString, int recordCount, DateTime? fromDate, DateTime? toDate)
    {
        sheet.AddMergedRegion(new CellRangeAddress(1, 2, 2, 5));
        ICell statusCell = sheet.GetRow(1).GetCell(2);
        statusCell.SetCellValue(status != null ? status.ToString() : "All Status");

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

        // Number of Records (F4)
        sheet.AddMergedRegion(new CellRangeAddress(4, 5, 9, 12));
        ICell recordsCell = sheet.GetRow(4).GetCell(9);
        recordsCell.SetCellValue(recordCount);
    }

    private void FillOrderData(IWorkbook workbook, ISheet sheet, IEnumerable<Order> orders)
    {
        ICellStyle dataStyle = CreateDataCellStyle(workbook);
        ICellStyle currencyStyle = CreateCurrencyStyle(workbook, dataStyle);
        ICellStyle dateStyle = CreateDateStyle(workbook, dataStyle);

        int rowIndex = 9;
        foreach (var order in orders)
        {
            IRow dataRow = sheet.CreateRow(rowIndex);

            CreateCell(dataRow, 0, order.Orderid.ToString(), dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 1, 3));
            ICell dateCell = dataRow.CreateCell(1);
            dateCell.SetCellValue((DateTime)order.Orderdate);
            dateCell.CellStyle = dateStyle;
            CreateCell(dataRow, 2, "", dataStyle);
            CreateCell(dataRow, 3, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 6));
            CreateCell(dataRow, 4, order.Customer.Customername, dataStyle);
            CreateCell(dataRow, 5, "", dataStyle);
            CreateCell(dataRow, 6, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 7, 9));
            CreateCell(dataRow, 7, order.OrderStatus.ToString(), dataStyle);
            CreateCell(dataRow, 8, "", dataStyle);
            CreateCell(dataRow, 9, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 11));
            CreateCell(dataRow, 10, order.Paymentmode.ToString(), dataStyle);
            CreateCell(dataRow, 11, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 12, 13));
            CreateCell(dataRow, 12, order.Rating?.ToString(), dataStyle);
            CreateCell(dataRow, 13, "", dataStyle);

            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 14, 15));
            ICell amountCell = dataRow.CreateCell(14);
            amountCell.SetCellValue((double)order.Totalamount);
            amountCell.CellStyle = currencyStyle;
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

    public async Task<OrderDetailsView> GetOrderDetailsViewService(int orderid){
        return await _orderRepository.GetOrderDetailsView(orderid);
    }

    public async Task<OrderDetailsView?> GetOrderDetailsForViewAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsByIdAsync(orderId);

            if (order == null)
            {
                return null; // Order not found
            }

            var orderDetailsView = new OrderDetailsView
            {
                OrderId = order.Orderid,
                CustomerId = order.Customerid,

                CustomerEmail = order.Customer?.Email ?? "N/A", 
                NoOfPerson = order.Noofperson ?? 0,
                OrderDate = order.Orderdate ?? DateTime.MinValue, 
                ModifiedDate = order.Modifiedat ?? order.Createdat ?? DateTime.MinValue, 
                SubTotal = order.Subamount ?? 0,
                Total = order.Totalamount,
                Paymentmode = order.Paymentmode,
                OrderStatus = order.OrderStatus,
                OrderWiseComment = order.Orderwisecomment ?? string.Empty,

                Section = order.Ordertables.FirstOrDefault()?.Table?.Section?.Sectionname ?? "N/A",
                Table = order.Ordertables.Any() 
                    ? string.Join(", ", order.Ordertables.Select(ot => ot.Table?.Tablename)) 
                    : "N/A",

                ItemsInOrder = order.Ordereditems.Select(oi => new ItemDetailForOrder
                {
                    OrderToItemId = oi.Ordereditemid,
                    
                    ItemName = oi.Item?.Itemname ?? "Unknown Item",
                    ItemId = oi.Item?.Itemid ?? 0,
                    ItemAmount = oi.Item?.Rate ?? 0,
                    ItemQuantity = oi.Quantity,
                    ReadyQuantity = oi.ReadyQuantity,
                    ItemWiseComment = oi.Itemwisecomment ?? string.Empty,
                    Isdefaulttax = oi.Item?.Isdefaulttax,
                    Taxpercentage = oi.Item?.Taxpercentage ?? 0,

                    ItemModifiers = oi.Ordereditemmodifers.Select(oim =>
                    {
                        var itemGroupMap = oi.Item?.Itemmodifiergroupmaps?
                            .FirstOrDefault(map => map.Modifiergroupid == oim.Modifiers?.Modifiergroupid);

                        return new ModifierDetailForOrder
                        {
                            ModifierItemId = oim.Modifieditemid,
                            ModifierId = oim.Modifierid,
                            Modifiergroupid = oim.Modifiers?.Modifiergroupid ?? 0,
                            ModifierName = oim.Modifiers?.Modifiername ?? "Unknown Modifier",
                            ModifierRate = oim.Modifiers?.Rate ?? 0,

                            ItemModifierMappingId = itemGroupMap?.Itemmodifiergroupid ?? 0, 
                            ModifierGroupName = oim.Modifiers?.Modifiergroup?.Modifiergroupname
                                                ?? itemGroupMap?.Modifiergroup?.Modifiergroupname 
                                                ?? "Unknown Group",
                            MinRequired = itemGroupMap?.Minselectionrequired ?? 0, 
                            MaxRequired = itemGroupMap?.Maxselectionallowed ?? 0,

                        };
                    }).ToList(), 
                }).ToList(), 

                TaxesForOrder = order.Ordertaxmappings.Select(otm => new TaxForOrder
                {
                    OrderId = otm.Orderid,
                    TaxName = otm.Tax?.Taxname ?? "N/A",
                    TaxValue = (decimal)(otm.Taxvalue ?? 0),
                    TaxTypeName = otm.Tax?.TaxType?.TaxName ?? "N/A" 
                }).ToList(), 

                Taxes = null 
            };

            return orderDetailsView;
        }


    public async Task<bool> SaveOrderAsync(OrderSaveViewModel model)
        {
            if (model == null)
            {
                return false; 
            }

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(model.OrderId);
                if (order == null)
                {
                    return false; 
                }

                order.OrderStatus = orderstatus.InProgress;
                _orderRepository.UpdateOrder(order);

                await ProcessOrderedItemsAsync(model.OrderId, model.Items);
                await ProcessOrderTaxesAsync(model.OrderId, model.Taxes);

                await _orderRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error saving order {model.OrderId}: {ex.Message}"); 
                return false;
            }
        }

        private async Task ProcessOrderedItemsAsync(int orderId, List<OrderItemViewModel> itemModels)
        {
            var existingDbItems = await _orderRepository.GetOrderedItemsWithModifiersAsync(orderId);
            var modelItemIds = itemModels.Select(vm => vm.OrderedItemId).Where(id => id > 0).ToHashSet(); 
            var dbItemMap = existingDbItems.ToDictionary(db => db.Ordereditemid);

            var itemsToDelete = existingDbItems.Where(db => !modelItemIds.Contains(db.Ordereditemid)).ToList();
            if (itemsToDelete.Any())
            {
                _orderRepository.RemoveOrderedItems(itemsToDelete);
            }

            foreach (var itemViewModel in itemModels)
            {
                if (itemViewModel.OrderedItemId > 0 && dbItemMap.TryGetValue(itemViewModel.OrderedItemId, out var existingItem))
                {
                    bool itemUpdated = false;
                    if (existingItem.Quantity != itemViewModel.Quantity)
                    {
                         existingItem.Quantity = itemViewModel.Quantity;
                         itemUpdated = true;
                    }
                    if (existingItem.Itemid != itemViewModel.ItemId)
                    {
                        existingItem.Itemid = itemViewModel.ItemId;
                        itemUpdated = true;
                    }

                    if (itemUpdated)
                    {
                        _orderRepository.UpdateOrderedItem(existingItem);
                    }


                    ProcessItemModifiers(itemViewModel, existingItem);
                }
                else
                {
                    var newItem = new Ordereditem
                    {
                        Orderid = orderId,
                        Itemid = itemViewModel.ItemId,
                        Quantity = itemViewModel.Quantity,
                    };

                    var newModifiers = CreateModifiersFromViewModel(itemViewModel, 0); 
                    foreach(var mod in newModifiers)
                    {
                        newItem.Ordereditemmodifers.Add(mod); 
                    }

                    _orderRepository.AddOrderedItem(newItem);
                }
            }
        }

        private void ProcessItemModifiers(OrderItemViewModel itemViewModel, Ordereditem existingItem)
        {
            var existingModifiers = existingItem.Ordereditemmodifers.ToList();
            var modelModifierIds = new HashSet<int>();

            if (itemViewModel.Groups != null)
            {
                foreach (var groupPair in itemViewModel.Groups)
                {
                    if (groupPair.Value.SelectedModifiers != null)
                    {
                        foreach (var modPair in groupPair.Value.SelectedModifiers)
                        {
                            modelModifierIds.Add(modPair.Value.ModifierId);
                        }
                    }
                }
            }

            var modifiersToDelete = existingModifiers
                .Where(dbMod => !modelModifierIds.Contains(dbMod.Modifierid))
                .ToList();

            var existingModifierIds = existingModifiers.Select(dbMod => dbMod.Modifierid).ToHashSet();
            var modifierIdsToAdd = modelModifierIds
                .Where(modelModId => !existingModifierIds.Contains(modelModId))
                .ToList();

            if (modifiersToDelete.Any())
            {
                _orderRepository.RemoveOrderedItemModifiers(modifiersToDelete);
            }

            if (modifierIdsToAdd.Any())
            {
                var modifiersToAdd = modifierIdsToAdd.Select(modId => new Ordereditemmodifer
                {
                    Ordereditemid = existingItem.Ordereditemid, 
                    Modifierid = modId
                }).ToList();
                _orderRepository.AddOrderedItemModifiers(modifiersToAdd);
            }
        }

        private List<Ordereditemmodifer> CreateModifiersFromViewModel(OrderItemViewModel itemViewModel, int orderedItemId)
        {
             var modifiers = new List<Ordereditemmodifer>();
             if (itemViewModel.Groups != null)
             {
                foreach (var groupPair in itemViewModel.Groups)
                {
                    if (groupPair.Value.SelectedModifiers != null)
                    {
                        foreach (var modPair in groupPair.Value.SelectedModifiers)
                        {
                            modifiers.Add(new Ordereditemmodifer
                            {
                                Ordereditemid = orderedItemId, 
                                Modifierid = modPair.Value.ModifierId
                            });
                        }
                    }
                }
             }
             return modifiers;
        }

        private async Task ProcessOrderTaxesAsync(int orderId, List<OrderTaxViewModel> taxModels)
        {
            var existingDbTaxes = await _orderRepository.GetOrderTaxMappingsAsync(orderId);
            var modelTaxesMap = taxModels.ToDictionary(vm => vm.TaxId); 
            var existingDbTaxesMap = existingDbTaxes.ToDictionary(db => db.Taxid);

            var taxesToDelete = existingDbTaxes
                .Where(dbTax => !modelTaxesMap.ContainsKey(dbTax.Taxid))
                .ToList();

            if (taxesToDelete.Any())
            {
                _orderRepository.RemoveOrderTaxMappings(taxesToDelete);
            }

            var taxesToAdd = new List<Ordertaxmapping>();
            var taxesToUpdate = new List<Ordertaxmapping>(); 

            foreach (var modelTax in taxModels)
            {

                int? calculatedTaxValue = (int?)Math.Round(modelTax.TaxValue);

                if (existingDbTaxesMap.TryGetValue(modelTax.TaxId, out var existingTax))
                {

                    if (existingTax.Taxvalue != calculatedTaxValue)
                    {
                        existingTax.Taxvalue = calculatedTaxValue;
                        _orderRepository.UpdateOrderTaxMapping(existingTax); 
                        taxesToUpdate.Add(existingTax);
                    }
                }
                else
                {
                    taxesToAdd.Add(new Ordertaxmapping
                    {
                        Orderid = orderId,
                        Taxid = modelTax.TaxId,
                        Taxvalue = calculatedTaxValue
                    });
                }
            }

            if (taxesToAdd.Any())
            {
                _orderRepository.AddOrderTaxMappings(taxesToAdd);
            }
        }

    public async Task<List<int>> GetTaxIdsByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return new List<int>();
            }

                var taxMappings = await _orderRepository.GetOrderTaxMappingsAsync(orderId);

                if (taxMappings != null)
                {
                    return taxMappings.Select(mapping => mapping.Taxid)
                                      .ToList();
                }
                else
                {
                    return new List<int>();
                }

        }
}