using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IOrderService
{
    Task<(List<Order> orders, int totalOrders, int totalPages)> GetPaginatedOrdersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder, orderstatus? status, DateTime? fromDate, DateTime? toDate);

    Task<(List<Order> ,int totalOrders)> GetOrdersForExport(string searchString,  orderstatus? status , DateTime? fromDate, DateTime? toDate);

    Task<(string fileName, byte[] fileContent)> GenerateOrderExcel(string searchString, orderstatus? status, DateTime? fromDate, DateTime? toDate);

    Task<OrderDetailsView> GetOrderDetailsViewService(int orderid);

    Task<bool> HasCustomerActiveOrder(int customerId);

    Task<int> createOrderbycustomerId(int customerId);
}
