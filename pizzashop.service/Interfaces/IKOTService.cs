using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IKOTService{
    Task<KOTViewModel> GetKOTViewModel(int categoryFil = -1);
}