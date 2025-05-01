using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface ITaxRepository
{
    Task<(List<Taxis> taxes, int totalTaxes)> GetAllTaxesAsync(int page, int pageSize, string search);

    Task<TaxType> GetTaxTypeDetails(int TaxTypeId);
    Task<Taxis> GetTaxByIdAsync(int id);
    Task<List<TaxType>> GetAllTaxTypesAsync();
    Task<bool> CreateTaxAsync(Taxis tax);
    Task<bool> UpdateTaxAsync(Taxis tax);
    Task<bool> DeleteTaxAsync(int id);

    Task<Taxis> GetTaxByName(TaxViewModel model);

    Task<IEnumerable<Taxis>> GetEnabledTaxesAsync();
}
