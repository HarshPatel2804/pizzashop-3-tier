using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class KOTOrderedItemsViewModel
{
    public int Ordereditemid { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public List<Modifier> Modifiers { get; set; }
    public string Instruction { get; set; }

}