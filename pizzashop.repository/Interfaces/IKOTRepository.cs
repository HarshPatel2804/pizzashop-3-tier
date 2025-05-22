using System.Collections.Generic;
using System.Threading.Tasks;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces
{
    public interface IKOTRepository
    {
        Task<(List<KOTRawDataviewModel>,int totalOrders)> GetKOTOrdersByCategoryAndStatus(int? categoryId, string status, int skip, int take);

        Task<int> UpdatePreparedQuantities(List<PreparedItemviewModel> updates , string status);
    }
}