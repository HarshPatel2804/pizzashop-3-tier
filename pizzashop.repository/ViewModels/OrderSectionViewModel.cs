
namespace pizzashop.repository.ViewModels
{
public class OrderSectionViewModel
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string Description { get; set; }
        public List<OrderTableViewModel> Tables { get; set; }
        public int AvailableCount { get; set; }
        public int AssignedCount { get; set; }
        public int RunningCount { get; set; }
    }
}