using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IWaitingTokenRepository
{
    Task<int> SaveWaitingToken(Waitingtoken model);

    Task<IEnumerable<Waitingtoken>> GetAllWaitingTokensWithCustomer(int section);

    Task<bool> IsCustomerInWaitingList(int customerId);

    Task WaitingToAssign(int tokenId);
}
