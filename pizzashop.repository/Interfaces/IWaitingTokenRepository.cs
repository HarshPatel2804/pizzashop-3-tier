using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IWaitingTokenRepository
{
    Task<int> SaveWaitingToken(Waitingtoken model);

    Task<IEnumerable<Waitingtoken>> GetAllWaitingTokensWithCustomer(int section);

    Task<IEnumerable<Waitingtoken>> GetAllWaitingTokens(List<int> sectionIds);

    Task<bool> IsCustomerInWaitingList(int customerId);

    Task WaitingToAssign(int tokenId);

    Task<bool> DeleteAsync(int tokenId);

    Task<Waitingtoken?> GetTokenByIdWithCustomerAsync(int tokenId);

    Task<Waitingtoken?> GetByIdAsync(int tokenId);
    Task<bool> UpdateWaitingTokenAsync(Waitingtoken token);

    Task<IEnumerable<Waitingtoken>> GetActiveWaitingTokensByDateRangeAsync(DateTime startDate, DateTime endDate);


}
