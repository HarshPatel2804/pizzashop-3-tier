using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IWaitingTokenRepository
{
    Task<int> SaveWaitingToken(Waitingtoken model);
}
