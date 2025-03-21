using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class AddEditItemViewModel
{
    public int Itemid { get; set; }

    public string Itemname { get; set; } = null!;

    public int Categoryid { get; set; }

    public decimal Rate { get; set; }

    public short? Quantity { get; set; }

    public int Unitid { get; set; }

    public bool Isavailable { get; set; }

    public decimal? Taxpercentage { get; set; }

    public string? Shortcode { get; set; }

    public bool? Isfavourite { get; set; }

    public bool Isdefaulttax { get; set; }

    public string? Itemimg { get; set; }

    public string? Description { get; set; }

    public itemtype ItemType { get; set; }
    public List<SelectListItem> Category{get;set;}
    public List<SelectListItem> Units{get;set;}

    public List<SelectListItem> ModifierGroups{get;set;}

    public List<ItemModifierGroupMapping> itemModifierGroupMapping {get; set;}
}

