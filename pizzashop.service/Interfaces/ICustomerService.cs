using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface ICustomerService
{
    Task<(List<Customer> customers, int totalCustomers, int totalPages)> GetPaginatedCustomersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder, DateTime? fromDate, DateTime? toDate);

    Task<CustomerViewModel> GetCustomerHistory(int customerId);

    Task<(string fileName, byte[] fileContent)> GenerateCustomerExcel(string searchString, DateTime? fromDate, DateTime? toDate);

    Task<Customer> GetCustomerByEmail(string Email);

    Task<int> AddCustomer(Customer model);

    Task<int> UpdateCustomer(Customer model);

    Task<OrderMenuCustomerViewModel?> GetOrderCustomerDetailsAsync(int orderId);

    Task<(bool Success , string Message)> updateOrderMenuCustomer(OrderMenuCustomerViewModel model);
}
