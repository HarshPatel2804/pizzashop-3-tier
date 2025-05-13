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

        public DashboardService(IOrderRepository orderRepository, IWaitingTokenRepository waitingTokenRepository, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _waitingTokenRepository = waitingTokenRepository;
            _customerRepository = customerRepository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(DateTime startDate, DateTime endDate)
        {
            DateTime inclusiveEndDate = endDate.Date.AddDays(1).AddTicks(-1);
            DateTime inclusiveStartDate = startDate.Date;
            TimeSpan dateRange = inclusiveEndDate - inclusiveStartDate;

            var orders = await _orderRepository.GetOrdersByDateRangeAsync(inclusiveStartDate, inclusiveEndDate);
            var waitingTokens = await _waitingTokenRepository.GetActiveWaitingTokensByDateRangeAsync(inclusiveStartDate, inclusiveEndDate);
            var customers = await _customerRepository.GetCustomersByDateRangeAsync(inclusiveStartDate, inclusiveEndDate);

            var completedOrders = orders.Where(o => o.OrderStatus == orderstatus.Completed).ToList();

            decimal totalSales = completedOrders.Sum(o => o.Totalamount);
            int totalCompletedOrdersCount = completedOrders.Count();
            decimal averageOrderValue = totalCompletedOrdersCount > 0 ? totalSales / totalCompletedOrdersCount : 0;
            averageOrderValue = Math.Round(averageOrderValue, 2);

            int waitingListCount = waitingTokens.Count();

            var waitingTimes = orders
            .Where(o => o.ServedTime.HasValue && o.ServedTime.Value >= o.Orderdate)
            .Select(o => (o.ServedTime.Value - o.Orderdate.Value).TotalMinutes);

            double averageTime = 0;
            if (waitingTimes.Any())
            {
                averageTime = waitingTimes.Average(t => t);
            }

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

            IEnumerable<DailyRevenueViewModel> dailyRevenue;
            IEnumerable<CustomerCountViewModel> customerData;

            // Same day - group by hours
            if (startDate.Date == endDate.Date)
            {
                dailyRevenue = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => new DateTime(o.Orderdate.Value.Year, o.Orderdate.Value.Month, o.Orderdate.Value.Day, o.Orderdate.Value.Hour, 0, 0))
                    .Select(g => new DailyRevenueViewModel
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.Totalamount),
                        GroupingType = "Hour"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                customerData = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => new DateTime(c.Createdat.Value.Year, c.Createdat.Value.Month, c.Createdat.Value.Day, c.Createdat.Value.Hour, 0, 0))
                    .Select(g => new CustomerCountViewModel
                    {
                        Date = g.Key,
                        Count = g.Count(),
                        GroupingType = "Hour"
                    })
                    .OrderBy(cc => cc.Date)
                    .ToList();
            }
            // More than 2 years - group by year
            else if (dateRange.TotalDays > 730)
            {
                dailyRevenue = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => new DateTime(o.Orderdate.Value.Year, 1, 1))
                    .Select(g => new DailyRevenueViewModel
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.Totalamount),
                        GroupingType = "Year"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                customerData = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => new DateTime(c.Createdat.Value.Year, 1, 1))
                    .Select(g => new CustomerCountViewModel
                    {
                        Date = g.Key,
                        Count = g.Count(),
                        GroupingType = "Year"
                    })
                    .OrderBy(cc => cc.Date)
                    .ToList();
            }
            // Between 1 month and 24 months - group by month
            else if (dateRange.TotalDays >= 30 && dateRange.TotalDays <= 730)
            {
                dailyRevenue = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => new DateTime(o.Orderdate.Value.Year, o.Orderdate.Value.Month, 1))
                    .Select(g => new DailyRevenueViewModel
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.Totalamount),
                        GroupingType = "Month"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                customerData = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => new DateTime(c.Createdat.Value.Year, c.Createdat.Value.Month, 1))
                    .Select(g => new CustomerCountViewModel
                    {
                        Date = g.Key,
                        Count = g.Count(),
                        GroupingType = "Month"
                    })
                    .OrderBy(cc => cc.Date)
                    .ToList();
            }
            // Default - group by day (less than a month)
            else
            {
                dailyRevenue = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => o.Orderdate.Value.Date)
                    .Select(g => new DailyRevenueViewModel
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.Totalamount),
                        GroupingType = "Day"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                customerData = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => c.Createdat.Value.Date)
                    .Select(g => new CustomerCountViewModel
                    {
                        Date = g.Key,
                        Count = g.Count(),
                        GroupingType = "Day"
                    })
                    .OrderBy(cc => cc.Date)
                    .ToList();
            }

            return new DashboardViewModel
            {
                TotalSales = totalSales,
                TotalOrders = totalCompletedOrdersCount,
                AverageOrderValue = averageOrderValue,
                AverageWaitingTime = averageTime,
                WaitingListCount = waitingListCount,
                TopSellingItems = topSellingItems,
                LeastSellingItems = leastSellingItems,
                DailyRevenueData = dailyRevenue.ToList(),
                CustomerCount = customerData.ToList()
            };
        }
    }
}