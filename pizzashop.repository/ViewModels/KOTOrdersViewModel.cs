namespace pizzashop.repository.ViewModels;

public class KOTOrdersViewModel
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string TableName { get; set; }
    public string SectionName { get; set; }
    public List<KOTOrderedItemsViewModel> Items { get; set; }
    public string Instruction { get; set; }
}