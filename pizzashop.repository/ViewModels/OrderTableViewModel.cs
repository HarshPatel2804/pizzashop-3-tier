
namespace pizzashop.repository.ViewModels
{
 public enum TableViewStatus
    {
        Available,
        Assigned,
        Running
    }

public class OrderTableViewModel
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public decimal Capacity { get; set; }
        public TableViewStatus Status { get; set; }
        public decimal CurrentOrderAmount { get; set; }
        public int NumberOfPersons { get; set; }

        public DateTime? OrderDate {get; set;}
    }
}