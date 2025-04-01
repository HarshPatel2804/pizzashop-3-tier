namespace pizzashop.repository.ViewModels
{
    public class ModifierDetailForOrder
    {
        public string ModifierName { get; set; }
        public decimal ModifierRate { get; set; }
        public int ModifierQuantity { get; set; }
         public int OrderedModifierPrice { get; set; }
    }
}
