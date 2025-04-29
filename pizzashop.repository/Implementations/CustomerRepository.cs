using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly PizzaShopContext _context;

    public CustomerRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<(List<Customer>, int totalCustomers)> GetPaginatedCustomersAsync(int page, int pageSize, string searchInput, string sortColumn, string sortOrder, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Customers
           .OrderBy(u => u.Customerid)
           .Where(u => string.IsNullOrEmpty(searchInput) ||
               u.Customername.ToLower().Contains(searchInput.ToLower()) ||
               u.Email.ToLower().Contains(searchInput.ToLower()) ||
               u.Phoneno.ToLower().Contains(searchInput.ToLower()) ||
               u.Createdat.ToString().Contains(searchInput.ToLower()) ||
               u.Totalorder.ToString().Contains(searchInput.ToLower())
               );

        switch (sortColumn)
        {
            case "date":
                query = sortOrder == "asc" ? query.OrderBy(u => u.Createdat) : query.OrderByDescending(u => u.Createdat);
                break;
            case "customername":
                query = sortOrder == "asc" ? query.OrderBy(u => u.Customername) : query.OrderByDescending(u => u.Customername);
                break;
            case "totalorder":
                query = sortOrder == "asc" ? query.OrderBy(u => u.Totalorder) : query.OrderByDescending(u => u.Totalorder);
                break;
            default:
                query = query.OrderBy(u => u.Customerid);
                break;
        }
        if (fromDate.HasValue && toDate.HasValue)
        {
            // Ensure toDate includes the entire day
            toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);

            query = query.Where(u =>
                u.Createdat >= fromDate.Value.Date &&
                u.Createdat <= toDate.Value);
        }
        else if (fromDate.HasValue)
        {
            query = query.Where(u => u.Createdat >= fromDate.Value.Date);
        }
        else if (toDate.HasValue)
        {
            query = query.Where(u => u.Createdat <= toDate.Value.Date);
        }
        int totalCustomers = await query.CountAsync();
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCustomers);
    }

    public async Task<Customer> GetCustomerById(int customerId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Customerid == customerId);
    }

    public async Task<List<Order>> GetOrdersByCustomer(int customerId)
    {
        return await _context.Orders
                .Where(o => o.Customerid == customerId)
                .Include(o => o.Ordereditems).ToListAsync();
    }

    public async Task<(List<Customer>, int totalCustomers)> GetCustomersForExport(string searchInput, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Customers
           .OrderBy(u => u.Customerid)
           .Where(u => string.IsNullOrEmpty(searchInput) ||
               u.Customername.ToLower().Contains(searchInput.ToLower()) ||
               u.Email.ToLower().Contains(searchInput.ToLower()) ||
               u.Phoneno.ToLower().Contains(searchInput.ToLower()) ||
               u.Createdat.ToString().Contains(searchInput.ToLower()) ||
               u.Totalorder.ToString().Contains(searchInput.ToLower())
               );

        if (fromDate.HasValue && toDate.HasValue)
        {
            // Ensure toDate includes the entire day
            toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);

            query = query.Where(u =>
                u.Createdat >= fromDate.Value.Date &&
                u.Createdat <= toDate.Value);
        }
        else if (fromDate.HasValue)
        {
            query = query.Where(u => u.Createdat >= fromDate.Value.Date);
        }
        else if (toDate.HasValue)
        {
            query = query.Where(u => u.Createdat <= toDate.Value.Date);
        }
        int totalCustomers = await query.CountAsync();
        var orders = await query
            .ToListAsync();

        return (orders, totalCustomers);
    }

    public async Task<Customer> GetCustomerByEmail(string Email)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email == Email);
    }

    public async Task<int> AddCustomer(Customer model)
    {
        await _context.Customers.AddAsync(model);
        await _context.SaveChangesAsync();

        return model.Customerid;
    }

    public async Task<int> UpdateCustomer(Customer model)
    {
        Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == model.Email);
        customer.Email = model.Email;
        customer.Phoneno = model.Phoneno;
        customer.Customername = model.Customername;

        await _context.SaveChangesAsync();

        return customer.Customerid;
    }

}
