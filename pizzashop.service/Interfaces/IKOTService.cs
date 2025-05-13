using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces
{
    public interface IKOTService
    {
        Task<KOTViewModel> GetKOTViewModel();
        Task<List<KOTOrdersViewModel>> GetKOTOrders(string categoryId, string status, int page, int itemsPerPage);

        Task UpdatePreparedQuantities(List<PreparedItemviewModel> updates , string status);
    }
}