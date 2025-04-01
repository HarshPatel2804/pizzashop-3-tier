using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Itemmodifiergroupmap
{
    public int Itemmodifiergroupid { get; set; }

    public int Itemid { get; set; }

    public int Modifiergroupid { get; set; }

    public short? Minselectionrequired { get; set; }

    public short? Maxselectionallowed { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Modifiergroup Modifiergroup { get; set; } = null!;

    // public virtual ICollection<Ordereditemmodifer> Ordereditemmodifers { get; set; } = new List<Ordereditemmodifer>();
}
