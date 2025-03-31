using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class OrderService : IOrderService
{
     private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
     public async Task<(List<Order> orders, int totalOrders, int totalPages)> GetPaginatedOrdersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder, orderstatus? status, DateTime? fromDate, DateTime? toDate)
    {
        var (orders, totalOrders) = await _orderRepository.GetPaginatedOrdersAsync(page, pageSize, search, sortColumn, sortOrder, status, fromDate , toDate);

        int totalPages = (int)System.Math.Ceiling((double)totalOrders / pageSize);

        return (orders, totalOrders, totalPages);
    }
}
