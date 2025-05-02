namespace pizzashop.repository.ViewModels
{
    public class ItemDetailForOrder
    {
        public int OrderToItemId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public decimal ItemAmount { get; set; }

        public int ItemQuantity { get; set; }

        public int ReadyQuantity { get; set; }

        public string ItemWiseComment { get; set; }

        public List<ModifierDetailForOrder> ItemModifiers { get; set; }

        public decimal TotalPrice { get; set; }

        public bool? Isdefaulttax { get; set; }

        public decimal? Taxpercentage { get; set; }

    }
}
