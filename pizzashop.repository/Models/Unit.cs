using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Unit
{
    public int Unitid { get; set; }

    public string Unitname { get; set; } = null!;

    public string? Shortname { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
