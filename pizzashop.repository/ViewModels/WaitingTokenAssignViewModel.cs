using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class WaitingTokenAssignViewModel
{
    public int WaitingTokenId { get; set; }
    public int SectionId { get; set; }
    public IEnumerable<Section>? Sections { get; set; }
    public IEnumerable<Table>? Tables { get; set; }
    public List<int> SelectedTableIds { get; set; }
}