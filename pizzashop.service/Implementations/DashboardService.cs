using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.service.Interfaces;
using pizzashop.repository.ViewModels;


namespace pizzashop.service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IWaitingTokenRepository _waitingTokenRepository;

        public DashboardService(IOrderRepository orderRepository, IWaitingTokenRepository waitingTokenRepository , ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _waitingTokenRepository = waitingTokenRepository;
            _customerRepository = customerRepository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(DateTime startDate, DateTime endDate)
        {
            DateTime inclusiveEndDate = endDate.Date.AddDays(1).AddTicks(-1);
            DateTime inclusiveStartDate = startDate.Date;

            var orders = await _orderRepository.GetOrdersByDateRangeAsync(inclusiveStartDate, inclusiveEndDate);
            var waitingTokens = await _waitingTokenRepository.GetActiveWaitingTokensByDateRangeAsync(inclusiveStartDate, inclusiveEndDate);
            var customers = await _customerRepository.GetCustomersByDateRangeAsync(inclusiveStartDate, inclusiveEndDate);

            var completedOrders = orders.Where(o => o.OrderStatus == orderstatus.Completed).ToList();

            decimal totalSales = completedOrders.Sum(o => o.Totalamount);
            int totalCompletedOrdersCount = completedOrders.Count();
            decimal averageOrderValue = totalCompletedOrdersCount > 0 ? totalSales / totalCompletedOrdersCount : 0;
            averageOrderValue = Math.Round(averageOrderValue, 2);

            int waitingListCount = waitingTokens.Count();

            var allSoldItemsAggregated = orders
               .SelectMany(o => o.Ordereditems)
               .Where(oi => oi.Item != null && oi.Item.Isdeleted != true)
               .GroupBy(oi => new { oi.Item.Itemid, oi.Item.Itemname, oi.Item.Itemimg })
               .Select(g => new DashboardItemViewModel
               {
                   ItemId = g.Key.Itemid,
                   ItemName = g.Key.Itemname,
                   ItemImg = g.Key.Itemimg,
                   QuantitySold = g.Sum(oi => oi.Quantity)
               })
               .ToList();

            var topSellingItems = allSoldItemsAggregated
                .OrderByDescending(item => item.QuantitySold)
                .Take(5)
                .ToList();

            var leastSellingItems = allSoldItemsAggregated
                .Where(item => item.QuantitySold > 0)
                .OrderBy(item => item.QuantitySold)
                .Take(5)
                .ToList();

            var dailyRevenue = completedOrders
                .GroupBy(o => o.Orderdate.Value.Date)
                .Select(g => new DailyRevenueViewModel
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.Totalamount)
                })
                .OrderBy(dr => dr.Date)
                .ToList();

            var Customers = customers
                .GroupBy(o => o.Createdat.Value.Date)
                .Select(g => new CustomerCountViewModel
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(dr => dr.Date)
                .ToList();

            return new DashboardViewModel
            {
                TotalSales = totalSales,
                TotalOrders = totalCompletedOrdersCount,
                AverageOrderValue = averageOrderValue,
                WaitingListCount = waitingListCount,
                TopSellingItems = topSellingItems,
                LeastSellingItems = leastSellingItems,
                DailyRevenueData = dailyRevenue,
                CustomerCount = Customers
            };
        }
    }
}