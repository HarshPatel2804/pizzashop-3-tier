using System.Collections.Generic;
using System.Threading.Tasks;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces
{
    public interface IKOTRepository
    {
        Task<List<KOTOrdersViewModel>> GetKOTOrdersByCategoryAndStatus(int? categoryId, string status, int skip, int take);

        Task UpdatePreparedQuantities(List<PreparedItemviewModel> updates , string status);
    }
}