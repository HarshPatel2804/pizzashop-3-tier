using System;
using System.Collections.Generic;
using Pizzashop.repository.Models;

namespace pizzashop.repository.Models;

public partial class Modifiergroup
{
    public int Modifiergroupid { get; set; }

    public string Modifiergroupname { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? SortOrder {get; set;}

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public virtual ICollection<Itemmodifiergroupmap> Itemmodifiergroupmaps { get; set; } = new List<Itemmodifiergroupmap>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();

    // public virtual ICollection<ModifierGroupModifierMapping> ModifierGroupModifierMappings {get; set;} = new List<ModifierGroupModifierMapping>();
}
