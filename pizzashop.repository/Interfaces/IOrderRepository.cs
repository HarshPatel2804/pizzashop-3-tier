using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IOrderRepository
{
    Task<(List<Order>, int totalOrders)> GetPaginatedOrdersAsync( int page, int pageSize, string searchInput, string sortColumn, string sortOrder,orderstatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);
}
