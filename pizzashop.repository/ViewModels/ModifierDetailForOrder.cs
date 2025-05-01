namespace pizzashop.repository.ViewModels
{
    public class ModifierDetailForOrder
    {
        public int ModifierItemId { get; set; }

        public int ModifierId { get; set; }

        public int Modifiergroupid { get; set; }

        public int ItemModifierMappingId { get; set; }

        public string ModifierGroupName { get; set; }

        public int MinRequired { get; set; }

        public int MaxRequired { get; set; }
        public string ModifierName { get; set; }

        public decimal ModifierRate { get; set; }

        public int ModifierQuantity { get; set; }

         public int OrderedModifierPrice { get; set; }
    }
}
