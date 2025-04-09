using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class AddEditItemViewModel
{
    public int Itemid { get; set; }

    [Required(ErrorMessage = "Item Name is required.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Item Name must be between 2 and 30 characters.")]
    public string Itemname { get; set; } = null!;

    public int ModifierGroupId {get; set;}

    [Required(ErrorMessage = "Category is required.")]
    public int Categoryid { get; set; }

    [Required(ErrorMessage = "Rate is required.")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    public short? Quantity { get; set; }

    [Required(ErrorMessage = "Unit id is required.")]
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

    public List<ItemModifierGroupMapping> SelectedModifierGroups {get; set;}
}
