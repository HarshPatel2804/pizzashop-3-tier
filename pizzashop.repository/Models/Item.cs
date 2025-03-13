using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public enum itemtype
{
    Veg = 0,
    Nonveg = 1,

    Vegan = 2
}
public partial class Item
{
    public int Itemid { get; set; }

    public string Itemname { get; set; } = null!;

    public itemtype Itemtype { get; set; }

    public int Categoryid { get; set; }

    public decimal Rate { get; set; }

    public short? Quantity { get; set; }

    public int Unitid { get; set; }

    public bool? Isavailable { get; set; }

    public decimal? Taxpercentage { get; set; }

    public string? Shortcode { get; set; }

    public bool? Isfavourite { get; set; }

    public bool? Isdefaulttax { get; set; }

    public string? Itemimg { get; set; }

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Itemmodifiergroupmap> Itemmodifiergroupmaps { get; set; } = new List<Itemmodifiergroupmap>();

    public virtual ICollection<Ordereditem> Ordereditems { get; set; } = new List<Ordereditem>();

    public virtual Unit Unit { get; set; } = null!;
}
