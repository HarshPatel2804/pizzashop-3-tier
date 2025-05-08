using pizzashop.repository.ViewModels;
using System; // Required for DateTime
using System.Threading.Tasks;

namespace pizzashop.service.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(DateTime startDate, DateTime endDate);
    }
}