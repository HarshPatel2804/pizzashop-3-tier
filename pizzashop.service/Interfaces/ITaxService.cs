using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface ITaxService
{
    Task<(List<TaxViewModel> tableModel, int totalTaxes, int totalPages)> GetTaxes(int page, int pageSize, string search);
}
