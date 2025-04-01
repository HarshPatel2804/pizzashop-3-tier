using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Modifier
{
    public int Modifierid { get; set; }

    public string Modifiername { get; set; } = null!;

    public int Modifiergroupid { get; set; }

    public decimal Rate { get; set; }

    public short? Quantity { get; set; }

    public int Unitid { get; set; }

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public virtual Modifiergroup Modifiergroup { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;

    public virtual ICollection<Ordereditemmodifer> Ordereditemmodifers { get; set; } = new List<Ordereditemmodifer>();
}
