using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface IOrderRepository
{
    Task<(List<Order>, int totalOrders)> GetPaginatedOrdersAsync( int page, int pageSize, string searchInput, string sortColumn, string sortOrder,orderstatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);

    Task<(List<Order>, int totalOrders)> GetOrdersForExport( DateTime? fromDate, DateTime? toDate , orderstatus? status,string searchInput);

   Task<OrderDetailsView> GetOrderDetailsView(int orderId);

   Task<bool> HasCustomerActiveOrder(int customerId);
}
