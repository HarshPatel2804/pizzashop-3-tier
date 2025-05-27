using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IWaitingTokenService
{
    Task<(bool , string)> SaveWaitingToken(WaitingtokenViewModel model);

    Task<IEnumerable<WaitingtokenViewModel>> GetAllWaitingTokens(List<int> sectionIds);

    Task<bool> IsCustomerInWaitingList(int customerId);

    Task WaitingToAssign(int tokenId);

    Task<WaitingListViewModel> GetWaitingData();

    Task<IEnumerable<Waitingtoken>> GetWaitingTokensBySectionAsync(int sectionId);

    Task<bool> RemoveWaitingTokenAsync(int tokenId);

    Task<(bool success, string message, WaitingtokenViewModel? model)> GetWaitingTokenForEditAsync(int tokenId);

    Task<(bool success, string message)> UpdateWaitingTokenDetailsAsync(WaitingtokenViewModel viewModel);
    Task<(string, int)> AssignTable(WaitingAssignViewModel model);
}
