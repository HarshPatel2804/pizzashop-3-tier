using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface ITaxRepository
{
    Task<(List<Taxis> taxes, int totalTaxes)> GetAllTaxesAsync(int page, int pageSize, string search);

    Task<TaxType> GetTaxTypeDetails(int TaxTypeId);
}
