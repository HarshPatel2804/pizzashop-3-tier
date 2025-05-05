using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels
{
public class OrderModifierViewModel
{
    public int ModifierId { get; set; }
    public string ModifierName { get; set; }
    public decimal Rate { get; set; }
}

public class OrderModifierGroupViewModel
{
    public int GroupId { get; set; }
    public int MappingId { get; set; }
    public string GroupName { get; set; }
    public int MinRequired { get; set; }
    public int MaxAllowed { get; set; }
    public Dictionary<string, OrderModifierViewModel> SelectedModifiers { get; set; } = new Dictionary<string, OrderModifierViewModel>();
}

public class OrderItemViewModel
{
    public string UniqueId { get; set; }
    public int ItemId { get; set; }
    public int OrderedItemId { get; set; }
    public string ItemName { get; set; }
    public decimal ItemRate { get; set; }
    public int Quantity { get; set; }
    public bool IsDefaultTax { get; set; }
    public decimal? TaxPercentage { get; set; }
    public Dictionary<string, OrderModifierGroupViewModel> Groups { get; set; } = new Dictionary<string, OrderModifierGroupViewModel>();
}

public class OrderTaxViewModel
{
    public int TaxId { get; set; }
    public decimal TaxValue { get; set; }
}

public class OrderSaveViewModel
{
    public int OrderId { get; set; }

    public decimal Subamount { get; set; }

    public decimal Totalamount { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    public List<OrderTaxViewModel> Taxes { get; set; } = new List<OrderTaxViewModel>();
}
}