using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface ICustomerRepository
{
    Task<(List<Customer>, int totalCustomers)> GetPaginatedCustomersAsync(int page, int pageSize, string searchInput, string sortColumn, string sortOrder, DateTime? fromDate, DateTime? toDate);

    Task<List<Order>> GetOrdersByCustomer(int customerId);

    Task<Customer> GetCustomerById(int customerId);

    Task<(List<Customer>, int totalCustomers)> GetCustomersForExport(string searchInput, DateTime? fromDate, DateTime? toDate);

    Task<Customer> GetCustomerByEmail(string Email);

    Task<int> AddCustomer(Customer model);
}
