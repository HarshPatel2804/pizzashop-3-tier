using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public partial class ModifierViewModel
{
    public int Modifierid { get; set; }

    [Required(ErrorMessage = "Modifier Name is required.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Modifier Name must be between 2 and 30 characters.")]
    public string Modifiername { get; set; } = null!;

    public int Modifiergroupid { get; set; }

    [Required(ErrorMessage = "Rate is required.")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    public short? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    public int Unitid { get; set; }

    public string UnitName {get; set;}

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public List<SelectListItem> ModifierGroups{get;set;}

    public List<SelectListItem> Units{get;set;}

    public List<Modifiergroup> Groups{get; set;}

    public List<int> SelectedModifierGroups{get;set;}

    public virtual Modifiergroup Modifiergroup { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;
}
