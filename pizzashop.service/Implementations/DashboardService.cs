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
                // Generate all hours in the range
                var allHours = new List<DateTime>();
                for (int h = 0; h < 24; h++)
                {
                    allHours.Add(new DateTime(startDate.Year, startDate.Month, startDate.Day, h, 0, 0));
                }

                // Group completed orders by hour
                var ordersByHour = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => new DateTime(o.Orderdate.Value.Year, o.Orderdate.Value.Month, o.Orderdate.Value.Day, o.Orderdate.Value.Hour, 0, 0))
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.Totalamount));

                dailyRevenue = allHours
                    .Select(hour => new DailyRevenueViewModel
                    {
                        Date = hour,
                        Revenue = ordersByHour.ContainsKey(hour) ? ordersByHour[hour] : 0,
                        GroupingType = "Hour"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                // Group customers by hour
                var customersByHour = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => new DateTime(c.Createdat.Value.Year, c.Createdat.Value.Month, c.Createdat.Value.Day, c.Createdat.Value.Hour, 0, 0))
                    .ToDictionary(g => g.Key, g => g.Count());

                customerData = allHours
                    .Select(hour => new CustomerCountViewModel
                    {
                        Date = hour,
                        Count = customersByHour.ContainsKey(hour) ? customersByHour[hour] : 0,
                        GroupingType = "Hour"
                    })
                    .OrderBy(cc => cc.Date)
                    .ToList();
            }
            // More than 2 years - group by year
            else if (dateRange.TotalDays > 730)
            {
                int startYear = startDate.Year;
                int endYear = endDate.Year;

                // Generate all years in the range
                var allYears = new List<DateTime>();
                for (int year = startYear; year <= endYear; year++)
                {
                    allYears.Add(new DateTime(year, 1, 1));
                }
                // Group completed orders by year
                var ordersByYear = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => new DateTime(o.Orderdate.Value.Year, 1, 1))
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.Totalamount));

                dailyRevenue = allYears
                    .Select(year => new DailyRevenueViewModel
                    {
                        Date = year,
                        Revenue = ordersByYear.ContainsKey(year) ? ordersByYear[year] : 0,
                        GroupingType = "Year"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                // Group customers by year
                var customersByYear = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => new DateTime(c.Createdat.Value.Year, 1, 1))
                    .ToDictionary(g => g.Key, g => g.Count());

                customerData = allYears
                    .Select(year => new CustomerCountViewModel
                    {
                        Date = year,
                        Count = customersByYear.ContainsKey(year) ? customersByYear[year] : 0,
                        GroupingType = "Year"
                    })
                    .OrderBy(cc => cc.Date)
                    .ToList();
            }
            // Between 1 month and 24 months - group by month
            else if (dateRange.TotalDays >= 30 && dateRange.TotalDays <= 730)
            {
                var allMonths = new List<DateTime>();
                for (var date = new DateTime(startDate.Year, startDate.Month, 1);
                     date <= new DateTime(endDate.Year, endDate.Month, 1);
                     date = date.AddMonths(1))
                {
                    allMonths.Add(date);
                }

                // Group completed orders by month
                var ordersByMonth = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => new DateTime(o.Orderdate.Value.Year, o.Orderdate.Value.Month, 1))
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.Totalamount));

                // Join all months with order data
                dailyRevenue = allMonths
                    .Select(month => new DailyRevenueViewModel
                    {
                        Date = month,
                        Revenue = ordersByMonth.ContainsKey(month) ? ordersByMonth[month] : 0,
                        GroupingType = "Month"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                // Group customers by month
                var customersByMonth = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => new DateTime(c.Createdat.Value.Year, c.Createdat.Value.Month, 1))
                    .ToDictionary(g => g.Key, g => g.Count());

                customerData = allMonths
                    .Select(month => new CustomerCountViewModel
                    {
                        Date = month,
                        Count = customersByMonth.ContainsKey(month) ? customersByMonth[month] : 0,
                        GroupingType = "Month"
                    })
                    .OrderBy(cc => cc.Date)
                    .ToList();
            }
            // Default - group by day (less than a month)
            else
            {
                // Generate all days in the range
                var allDays = new List<DateTime>();
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    allDays.Add(date);
                }
                // Group completed orders by day
                var ordersByDay = completedOrders
                    .Where(o => o.Orderdate.HasValue)
                    .GroupBy(o => o.Orderdate.Value.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.Totalamount));

                dailyRevenue = allDays
                    .Select(day => new DailyRevenueViewModel
                    {
                        Date = day,
                        Revenue = ordersByDay.ContainsKey(day) ? ordersByDay[day] : 0,
                        GroupingType = "Day"
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                // Group customers by day
                var customersByDay = customers
                    .Where(c => c.Createdat.HasValue)
                    .GroupBy(c => c.Createdat.Value.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                customerData = allDays
                    .Select(day => new CustomerCountViewModel
                    {
                        Date = day,
                        Count = customersByDay.ContainsKey(day) ? customersByDay[day] : 0,
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