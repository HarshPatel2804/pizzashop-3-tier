namespace pizzashop.repository.ViewModels
{
    public class OrderDetailsView
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string CustomerEmail { get; set; }
        public int NoOfPerson { get; set; }
        public string Section { get; set; }
        public string Table { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public List<ItemDetailForOrder> ItemsInOrder { get; set; }
        public List<TaxForOrder> TaxesForOrder { get; set; }
        public string PaymentMethod { get; set; }
    }
}
