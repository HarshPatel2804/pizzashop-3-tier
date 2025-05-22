public class KOTRawDataviewModel
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string OrderInstruction { get; set; } = string.Empty;
    public int OrderedItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int ItemQuantity { get; set; }
    public string ItemInstruction { get; set; } = string.Empty;
    public int ModifierId { get; set; }
    public string ModifierName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
}