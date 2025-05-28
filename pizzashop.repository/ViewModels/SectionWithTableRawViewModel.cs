namespace pizzashop.repository.ViewModels;

public class SectionWithTableRawViewModel
{
    public int SectionId { get; set; }
    public string SectionName { get; set; }
    public string Description { get; set; }
    public int TableId { get; set; }
    public string TableName { get; set; }
    public decimal Capacity { get; set; }
    public int TableViewStatus { get; set; } 
    public decimal CurrentOrderAmount { get; set; }
    public int NumberOfPersons { get; set; }
    public int? OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public int AvailableCount { get; set; }
    public int AssignedCount { get; set; }
    public int RunningCount { get; set; }
}