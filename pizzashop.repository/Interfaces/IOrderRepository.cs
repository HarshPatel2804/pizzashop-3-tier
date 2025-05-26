using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface IOrderRepository
{
    Task<(List<Order>, int totalOrders)> GetPaginatedOrdersAsync(int page, int pageSize, string searchInput, string sortColumn, string sortOrder, orderstatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);

    Task<(List<Order>, int totalOrders)> GetOrdersForExport(DateTime? fromDate, DateTime? toDate, orderstatus? status, string searchInput);

    Task<OrderDetailsView> GetOrderDetailsView(int orderId);

    Task<bool> HasCustomerActiveOrder(int customerId);

    Task<int> createOrder(Order order);

    Task<Order?> GetOrderWithDetailsByIdAsync(int orderId);

    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<List<Ordereditem>> GetOrderedItemsWithModifiersAsync(int orderId);
    Task<List<Ordertaxmapping>> GetOrderTaxMappingsAsync(int orderId);

    void UpdateOrder(Order order);
    void AddOrderedItem(Ordereditem item);
    void UpdateOrderedItem(Ordereditem item);
    void RemoveOrderedItems(IEnumerable<Ordereditem> items);

    void RemoveOrderedItemModifiers(IEnumerable<Ordereditemmodifer> modifiers);
    void AddOrderedItemModifiers(IEnumerable<Ordereditemmodifer> modifiers);

    void RemoveOrderTaxMappings(IEnumerable<Ordertaxmapping> taxMappings);
    void AddOrderTaxMappings(IEnumerable<Ordertaxmapping> taxMappings);
    void UpdateOrderTaxMapping(Ordertaxmapping taxMapping);

    Task<int> SaveChangesAsync();

    Task<Order?> GetOrderTablesAsync(int orderId);

    Task<int> updateOrderAsync(Order order);

    Task<Ordereditem> GetOrderedItem(int orderItemId);

    Task<int> UpdateOrderedItemAsync(Ordereditem orderedItem);

    void UpdateTable(Table table);

    Task<int> AddReviewAsync(Customerreview review);

    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

    DateTime? GetFirstOrderDate();
}
