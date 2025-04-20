using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.repository.Interfaces;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class KOTService : IKOTService
{
    private readonly IMenuService _menuService;

    public KOTService(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<KOTViewModel> GetKOTViewModel(int categoryFil = -1)
    {
        try
        {
            KOTViewModel kOTViewModel = new KOTViewModel { };

            List<CategoryViewModel> categories = await _menuService.GetAllCategories();
            kOTViewModel.categories = categories;

            // kOTViewModel.orders = GetOrderViewModelForKOT(categoryFil, "InProgress");
            return kOTViewModel;
        }
        catch (Exception e)
        {
            return new KOTViewModel { };
        }
    }

   



    
}