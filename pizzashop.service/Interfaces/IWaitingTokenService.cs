using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IWaitingTokenService
{
    Task<int> SaveWaitingToken(WaitingtokenViewModel model);
}
