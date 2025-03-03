using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class Country
{
    public int Countryid { get; set; }

    public string Countryname { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
