using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Ordereditemmodifer
{
    public int Modifieditemid { get; set; }

    public int Ordereditemid { get; set; }

    public int Modifierid { get; set; }

    public virtual Modifier Modifiers { get; set; } = null!;

    public virtual Ordereditem Ordereditem { get; set; } = null!;
}
