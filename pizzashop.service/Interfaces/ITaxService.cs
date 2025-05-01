using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface ITaxService
{
    Task<(List<TaxViewModel> tableModel, int totalTaxes, int totalPages)> GetTaxes(int page, int pageSize, string search);

    Task<TaxViewModel> GetTaxById(int id);
    Task<TaxViewModel> PrepareNewTaxViewModel();
    Task<bool> CreateTax(TaxViewModel viewModel);
    Task<bool> UpdateTax(TaxViewModel viewModel);
    Task<bool> DeleteTax(int id);

    Task<Taxis> GetTaxByName(TaxViewModel model);

    Task<IEnumerable<TaxViewModel>> GetEnabledTaxes();
}
