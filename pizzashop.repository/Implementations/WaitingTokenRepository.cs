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

    public async Task<int> SaveWaitingToken(Waitingtoken model)
    {
        await _context.AddAsync(model);
        await _context.SaveChangesAsync();
        return model.Waitingtokenid;
    }

    public async Task<IEnumerable<Waitingtoken>> GetAllWaitingTokensWithCustomer(int section)
    {
        if (section == 0)
        {
            return await _context.Waitingtokens
                .Where(u => u.Isassigned == false)
                .Include(w => w.Customer)
                .ToListAsync();
        }
        else
        {
            return await _context.Waitingtokens
                .Where(u => u.Sectionid == section && u.Isassigned == false)
                .Include(w => w.Customer)
                .ToListAsync();
        }
    }

    public async Task<bool> IsCustomerInWaitingList(int customerId)
    {
        return await _context.Waitingtokens
            .AnyAsync(w => w.Customerid == customerId && w.Isassigned == false);
    }

    public async Task WaitingToAssign(int tokenId)
    {
        var token = await _context.Waitingtokens.FirstOrDefaultAsync(w => w.Waitingtokenid == tokenId);
        token.Isassigned = true;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int tokenId)
    {
        var token = await _context.Waitingtokens.FindAsync(tokenId);
        if (token == null)
        {
            return false;
        }

        _context.Waitingtokens.Remove(token);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Waitingtoken?> GetTokenByIdWithCustomerAsync(int tokenId)
    {
        return await _context.Waitingtokens
                             .Include(wt => wt.Customer)
                             .FirstOrDefaultAsync(wt => wt.Waitingtokenid == tokenId);
    }

    public async Task<Waitingtoken?> GetByIdAsync(int tokenId)
        {
            return await _context.Waitingtokens.FindAsync(tokenId);
        }

        public async Task<bool> UpdateWaitingTokenAsync(Waitingtoken token)
        {
            _context.Waitingtokens.Update(token);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Waitingtoken>> GetActiveWaitingTokensByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var inclusiveEndDate = endDate.Date.AddDays(1).AddTicks(-1);
            
            return await _context.Waitingtokens
                .Where(wt => wt.Createdat >= startDate.Date && 
                             wt.Createdat <= inclusiveEndDate && 
                             wt.Isassigned == false)
                .ToListAsync();
        }
}
