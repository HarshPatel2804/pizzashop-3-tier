using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class State
{
    public int Stateid { get; set; }

    public string Statename { get; set; } = null!;

    public int Countryid { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
