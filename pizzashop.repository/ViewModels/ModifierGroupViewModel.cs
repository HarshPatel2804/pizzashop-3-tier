using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class ModifierGroupViewModel
{
    public int Modifiergroupid { get; set; }

    public string Modifiergroupname { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public List<int> SelectedModifierIds { get; set; } = new List<int>();

    public virtual ICollection<Itemmodifiergroupmap> Itemmodifiergroupmaps { get; set; } = new List<Itemmodifiergroupmap>();

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
