using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Ordertaxmapping
{
    public int Ordertaxid { get; set; }

    public int Orderid { get; set; }

    public int Taxid { get; set; }

    public decimal? Taxvalue { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Taxis Tax { get; set; } = null!;
}
