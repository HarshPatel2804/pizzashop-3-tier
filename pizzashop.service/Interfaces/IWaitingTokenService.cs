using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IWaitingTokenService
{
    Task<int> SaveWaitingToken(WaitingtokenViewModel model);

    Task<IEnumerable<WaitingtokenViewModel>> GetAllWaitingTokens(int section);

    Task<bool> IsCustomerInWaitingList(int customerId);
}
