using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzaShopContext _context;

    public OrderRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<(List<Order>, int totalOrders)> GetPaginatedOrdersAsync(int page, int pageSize, string searchInput, string sortColumn, string sortOrder, orderstatus? status, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Orders
           .Include(u => u.Customer)
           .OrderBy(u => u.Orderid)
           .Where(u => string.IsNullOrEmpty(searchInput) ||
               u.Customer.Customername.ToLower().Contains(searchInput.ToLower()) ||
               u.Totalamount.ToString().Contains(searchInput.ToLower()));

        switch (sortColumn)
        {
            case "orderid":
                query = sortOrder == "asc" ? query.OrderBy(u => u.Orderid) : query.OrderByDescending(u => u.Orderid);
                break;
            case "orderdate":
                query = sortOrder == "asc" ? query.OrderBy(u => u.Orderdate) : query.OrderByDescending(u => u.Orderdate);
                break;
            case "customername":
                query = sortOrder == "asc" ? query.OrderBy(u => u.Customer.Customername) : query.OrderByDescending(u => u.Customer.Customername);
                break;
            case "totalamount":
                query = sortOrder == "asc" ? query.OrderBy(u => u.Totalamount) : query.OrderByDescending(u => u.Totalamount);
                break;
            default:
                query = query.OrderBy(u => u.Orderid);
                break;
        }
        if (fromDate.HasValue && toDate.HasValue)
        {
            // To include entire day
            toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);

            query = query.Where(u =>
                u.Orderdate >= fromDate.Value.Date &&
                u.Orderdate <= toDate.Value);
        }
        else if (fromDate.HasValue)
        {
            query = query.Where(u => u.Orderdate >= fromDate.Value.Date);
        }
        else if (toDate.HasValue)
        {
            query = query.Where(u => u.Orderdate <= toDate.Value.Date);
        }
        if (status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }
        int totalOrders = await query.CountAsync();
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalOrders);
    }

    public async Task<(List<Order>, int totalOrders)> GetOrdersForExport(DateTime? fromDate, DateTime? toDate, orderstatus? status, string searchInput)
    {
        var query = _context.Orders
           .Include(u => u.Customer)
           .OrderBy(u => u.Orderid)
           .Where(u => string.IsNullOrEmpty(searchInput) ||
               u.Customer.Customername.ToLower().Contains(searchInput.ToLower()) ||
               u.Totalamount.ToString().Contains(searchInput.ToLower()));

        if (fromDate.HasValue && toDate.HasValue)
        {
            toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);

            query = query.Where(u =>
                u.Orderdate >= fromDate.Value.Date &&
                u.Orderdate <= toDate.Value);
        }
        else if (fromDate.HasValue)
        {
            query = query.Where(u => u.Orderdate >= fromDate.Value.Date);
        }
        else if (toDate.HasValue)
        {
            query = query.Where(u => u.Orderdate <= toDate.Value.Date);
        }
        if (status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }
        int totalOrders = await query.CountAsync();
        var orders = await query.ToListAsync();

        return (orders, totalOrders);
    }

    public async Task<OrderDetailsView> GetOrderDetailsView(int orderId)
    {
        var order = await _context.Orders
            .Where(o => o.Orderid == orderId)
            .Include(o => o.Customer)
            .Include(o => o.Ordereditems)
                .ThenInclude(oi => oi.Item)
            .Include(o => o.Ordereditems)
                .ThenInclude(O => O.Ordereditemmodifers)
                    .ThenInclude(O => O.Modifiers)
            .Include(o => o.Ordertables)
                .ThenInclude(ot => ot.Table)
                    .ThenInclude(s => s.Section)
            .Include(o => o.Ordertaxmappings)
                .ThenInclude(otm => otm.Tax)
                    .ThenInclude(t => t.TaxType)
            .FirstOrDefaultAsync();

        if (order == null)
            return null;

        var orderDetailsView = new OrderDetailsView
        {

            OrderId = order.Orderid,
            CustomerName = order.Customer.Customername,
            ContactNumber = order.Customer.Phoneno,
            CustomerEmail = order.Customer.Email,
            NoOfPerson = order.Noofperson ?? 0,
            Section = order.Ordertables.Any()
                ? string.Join(", ", order.Ordertables.Select(ot => ot.Table?.Section?.Sectionname))
                : "N/A",
            Table = order.Ordertables.Any()
                ? string.Join(", ", order.Ordertables.Select(ot => ot.Table?.Tablename))
                : "N/A",
            OrderDate = order.Orderdate ?? DateTime.MinValue,
            ModifiedDate = order.Modifiedat ?? DateTime.MinValue,
            SubTotal = order.Subamount ?? 0,
            Total = order.Totalamount,
            Paymentmode = order.Paymentmode,
            OrderStatus = order.OrderStatus,
            ItemsInOrder = order.Ordereditems.Select(oi => new ItemDetailForOrder
            {
                OrderToItemId = oi.Ordereditemid,
                ItemName = oi.Item.Itemname,
                ItemAmount = oi.Item.Rate,
                ItemQuantity = oi.Quantity,
                TotalPrice = oi.Item.Rate * oi.Quantity,
                ItemModifiers = oi.Ordereditemmodifers
        .Select(oim => new ModifierDetailForOrder
        {
            ModifierName = oim.Modifiers.Modifiername,
            ModifierRate = oim.Modifiers.Rate,
            ModifierQuantity = oim.Modifiers.Quantity ?? 0,
            OrderedModifierPrice = (int)(oim.Modifiers.Rate * oi.Quantity)
        }).ToList()
            }).ToList(),
            TaxesForOrder = order.Ordertaxmappings.Select(otm => new TaxForOrder
            {
                TaxName = otm.Tax.Taxname,
                TaxValue = (decimal)otm.Taxvalue,
                TaxTypeName = otm.Tax.TaxType.TaxName
            }).ToList()
        };

        return orderDetailsView;
    }

    public async Task<bool> HasCustomerActiveOrder(int customerId)
    {
        return await _context.Orders
            .AnyAsync(o => o.Customerid == customerId &&
                          (o.OrderStatus == orderstatus.InProgress ||
                           o.OrderStatus == orderstatus.Pending ||
                           o.OrderStatus == orderstatus.Served));
    }

    public async Task<int> createOrder(Order order)
    {
        await _context.AddAsync(order);
        await _context.SaveChangesAsync();
        return order.Orderid;
    }

    public async Task<Order?> GetOrderWithDetailsByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Ordereditems)
                .ThenInclude(oi => oi.Item)
                    .ThenInclude(item => item.Itemmodifiergroupmaps)
                        .ThenInclude(imgm => imgm.Modifiergroup)
            .Include(o => o.Ordereditems)
                .ThenInclude(oi => oi.Ordereditemmodifers)
                    .ThenInclude(oim => oim.Modifiers)
                        .ThenInclude(m => m.Modifiergroup)
            .Include(o => o.Ordertables)
                .ThenInclude(ot => ot.Table)
                    .ThenInclude(t => t.Section)
            .Include(o => o.Ordertaxmappings)
                .ThenInclude(otm => otm.Tax)
                    .ThenInclude(t => t.TaxType)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Orderid == orderId);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }

    public async Task<Order?> GetOrderTablesAsync(int orderId)
    {
        return await _context.Orders
                .Include(o => o.Ordertables)
                    .ThenInclude(ot => ot.Table)
                .FirstOrDefaultAsync(o => o.Orderid == orderId);
    }

    public async Task<List<Ordereditem>> GetOrderedItemsWithModifiersAsync(int orderId)
    {
        return await _context.Ordereditems
                             .Include(oi => oi.Ordereditemmodifers)
                             .Where(oi => oi.Orderid == orderId)
                             .ToListAsync();
    }

    public async Task<List<Ordertaxmapping>> GetOrderTaxMappingsAsync(int orderId)
    {
        return await _context.Ordertaxmappings
                             .Where(otm => otm.Orderid == orderId)
                             .ToListAsync();
    }

    public async Task<int> updateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order.Orderid;
    }
    public async Task<int> UpdateOrderedItemAsync(Ordereditem orderedItem)
    {
        _context.Ordereditems.Update(orderedItem);
        await _context.SaveChangesAsync();
        return orderedItem.Orderid;
    }

    public void UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
    }

    public void UpdateTable(Table table)
    {
        _context.Tables.Update(table);
    }

    public void AddOrderedItem(Ordereditem item)
    {
        _context.Ordereditems.Add(item);
    }

    public void UpdateOrderedItem(Ordereditem item)
    {
        _context.Ordereditems.Update(item);
    }

    public void RemoveOrderedItems(IEnumerable<Ordereditem> items)
    {
        _context.Ordereditems.RemoveRange(items);
    }

    public void RemoveOrderedItemModifiers(IEnumerable<Ordereditemmodifer> modifiers)
    {
        _context.Ordereditemmodifers.RemoveRange(modifiers);
    }

    public void AddOrderedItemModifiers(IEnumerable<Ordereditemmodifer> modifiers)
    {
        _context.Ordereditemmodifers.AddRange(modifiers);
    }

    public void RemoveOrderTaxMappings(IEnumerable<Ordertaxmapping> taxMappings)
    {
        _context.Ordertaxmappings.RemoveRange(taxMappings);
    }

    public void AddOrderTaxMappings(IEnumerable<Ordertaxmapping> taxMappings)
    {
        _context.Ordertaxmappings.AddRange(taxMappings);
    }

    public void UpdateOrderTaxMapping(Ordertaxmapping taxMapping)
    {
        _context.Ordertaxmappings.Update(taxMapping);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<Ordereditem> GetOrderedItem(int orderItemId)
    {
        return await _context.Ordereditems.FirstOrDefaultAsync(oi => oi.Ordereditemid == orderItemId);
    }

    public async Task<int> AddReviewAsync(Customerreview review)
    {
        await _context.Customerreviews.AddAsync(review);
        int savedChanges = await _context.SaveChangesAsync();
        return review.Reviewid;
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var inclusiveEndDate = endDate.Date.AddDays(1).AddTicks(-1);
            Console.WriteLine(startDate);
            Console.WriteLine(endDate);

            return await _context.Orders
                .Where(o => o.Orderdate >= startDate.Date && o.Orderdate <= inclusiveEndDate)
                .Include(o => o.Ordereditems)      
                    .ThenInclude(oi => oi.Item) 
                .Include(o => o.Customer) 
                .ToListAsync();
        }
}

