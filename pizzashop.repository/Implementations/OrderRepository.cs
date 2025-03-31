using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class OrderRepository :  IOrderRepository
{
    private readonly PizzaShopContext _context;

    public OrderRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<(List<Order>, int totalOrders)> GetPaginatedOrdersAsync( int page, int pageSize, string searchInput, string sortColumn, string sortOrder,orderstatus? status, DateTime? fromDate, DateTime? toDate)
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
            // Ensure toDate includes the entire day
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
}
