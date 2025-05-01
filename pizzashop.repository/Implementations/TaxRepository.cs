using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class TaxRepository : ITaxRepository
{
    private readonly PizzaShopContext _context;

    public TaxRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<(List<Taxis> taxes, int totalTaxes)> GetAllTaxesAsync(int page, int pageSize, string search)
    {
        var query = _context.Taxes
        .Where(u => u.Isdeleted != true)
        .Where(u => string.IsNullOrEmpty(search) ||
                        u.Taxname.ToLower().Contains(search.ToLower()));

        int totalTaxes = await query.CountAsync();

        var taxes = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (taxes, totalTaxes);
    }

    public async Task<TaxType> GetTaxTypeDetails(int TaxTypeId)
    {
        return await _context.TaxTypes.FirstOrDefaultAsync(u => u.TaxTypeId == TaxTypeId);
    }

    public async Task<Taxis> GetTaxByIdAsync(int id)
    {
        return await _context.Taxes
            .FirstOrDefaultAsync(t => t.Taxid == id && t.Isdeleted != true);
    }

    public async Task<List<TaxType>> GetAllTaxTypesAsync()
    {
        return await _context.TaxTypes
            .OrderBy(t => t.TaxName)
            .ToListAsync();
    }

    public async Task<bool> CreateTaxAsync(Taxis tax)
    {
        await _context.Taxes.AddAsync(tax);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTaxAsync(Taxis tax)
    {
        _context.Taxes.Update(tax);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTaxAsync(int id)
    {
        var tax = await GetTaxByIdAsync(id);
        if (tax == null)
            return false;

        // Soft delete
        tax.Isdeleted = true;

        return await UpdateTaxAsync(tax);
    }

    public async Task<Taxis> GetTaxByName(TaxViewModel model)
        {
            return await _context.Taxes
                .FirstOrDefaultAsync(mg => 
                    mg.Taxname.ToLower() == model.Taxname.ToLower() && 
                    mg.Taxid != model.Taxid && 
                    mg.Isdeleted != true);
        }

         public async Task<IEnumerable<Taxis>> GetEnabledTaxesAsync()
        {
            return await _context.Taxes
                .Include(t => t.TaxType) 
                .Where(t => t.Isenabled == true &&  t.Isdeleted != true)
                .AsNoTracking() 
                .ToListAsync();
        }
}
