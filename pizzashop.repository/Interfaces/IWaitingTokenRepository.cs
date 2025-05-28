using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface IWaitingTokenRepository
{
    Task<int> SaveWaitingToken(Waitingtoken model);

    Task<IEnumerable<WaitingTokenWithCustomerViewModel>> GetAllWaitingTokensWithCustomer(int section);

    Task<IEnumerable<Waitingtoken>> GetAllWaitingTokens(List<int> sectionIds);

    Task<bool> IsCustomerInWaitingList(int customerId);

    Task WaitingToAssign(int tokenId);

    Task<bool> DeleteAsync(int tokenId);

    Task<Waitingtoken?> GetTokenByIdWithCustomerAsync(int tokenId);

    Task<Waitingtoken?> GetByIdAsync(int tokenId);
    Task<SaveWaitingTokenRawViewModel> UpdateWaitingTokenAsync(WaitingtokenViewModel token);

    Task<IEnumerable<Waitingtoken>> GetActiveWaitingTokensByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<SaveWaitingTokenRawViewModel> AddWaitingTokenAsync(WaitingtokenViewModel model);

    Task<(string, int)> WaitingtoAssignTable(WaitingAssignViewModel model);
}
