using Microsoft.AspNetCore.Mvc;
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
}
