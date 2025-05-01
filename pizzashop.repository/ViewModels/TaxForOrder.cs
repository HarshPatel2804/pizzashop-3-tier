using System.Security;

namespace pizzashop.repository.ViewModels
{
    public class TaxForOrder
    {
        public int OrderId { get; set; }
        public string TaxName { get; set; }
        public decimal TaxValue { get; set; }

        public string TaxTypeName { get; set; }
    }
}
