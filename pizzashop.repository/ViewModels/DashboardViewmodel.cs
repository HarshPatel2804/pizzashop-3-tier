

namespace pizzashop.repository.ViewModels;
public class DashboardViewModel
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int WaitingListCount { get; set; }

    public double AverageWaitingTime { get; set; }

    public List<DashboardItemViewModel> TopSellingItems { get; set; }
    public List<DashboardItemViewModel> LeastSellingItems { get; set; }

    public List<DailyRevenueViewModel> DailyRevenueData { get; set; }
    public List<CustomerCountViewModel> CustomerCount { get; set; }
}

public class DashboardItemViewModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemImg { get; set; }
    public int QuantitySold { get; set; }
}

public class DailyRevenueViewModel
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }

    public string GroupingType { get; set; }
}

public class CustomerCountViewModel
{
    public DateTime Date { get; set; }
    public int Count { get; set; }

    public string GroupingType { get; set; }
}