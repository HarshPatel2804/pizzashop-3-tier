using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public partial class City
{
    public int Cityid { get; set; }

    public string Cityname { get; set; } = null!;

    public int Stateid { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public virtual State State { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
