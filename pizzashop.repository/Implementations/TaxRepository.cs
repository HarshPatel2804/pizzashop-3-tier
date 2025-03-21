using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

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
}
