using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class WaitingTokenRepository : IWaitingTokenRepository
{
    private readonly PizzaShopContext _context;

    public WaitingTokenRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<int> SaveWaitingToken(Waitingtoken model){
        await _context.AddAsync(model);
        await _context.SaveChangesAsync();
        return model.Waitingtokenid;
    }

     public async Task<IEnumerable<Waitingtoken>> GetAllWaitingTokensWithCustomer(int section)
    {
        return await _context.Waitingtokens
                        .Where(u => u.Sectionid == section)
                       .Include(w => w.Customer)
                       .ToListAsync();
    }

    public async Task<bool> IsCustomerInWaitingList(int customerId)
    {
        return await _context.Waitingtokens
            .AnyAsync(w => w.Customerid == customerId && w.Isassigned == false);
    }
}
