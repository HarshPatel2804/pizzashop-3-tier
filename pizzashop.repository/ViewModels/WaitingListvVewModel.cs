using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class WaitingListViewModel{
    public List<SectionViewModel> sections { get;set; }

    public List<Waitingtoken> waitingTokens { get; set; }
    
}
