using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Ordereditem
{
    public int Ordereditemid { get; set; }

    public int Orderid { get; set; }

    public int Itemid { get; set; }

    public string? Itemwisecomment { get; set; }

    public int ReadyQuantity { get; set; }
    public int Quantity { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<Ordereditemmodifer> Ordereditemmodifers { get; set; } = new List<Ordereditemmodifer>();
}
