using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public partial class ModifierViewModel
{
    public int Modifierid { get; set; }

    public string Modifiername { get; set; } = null!;

    public int Modifiergroupid { get; set; }

    public decimal Rate { get; set; }

    public short? Quantity { get; set; }

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
