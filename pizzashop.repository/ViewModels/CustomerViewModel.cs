using System;
using System.Collections.Generic;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public partial class CustomerViewModel
{
    public int Customerid { get; set; }

    public string Customername { get; set; } = null!;

    public string? Email { get; set; }

    public string Phoneno { get; set; } = null!;

    public int? Totalorder { get; set; }

    public short? Visitcount { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public decimal? AverageBill {get; set;}

    public decimal? MaxOrder {get; set;}

    public DateTime? ComingSince {get; set;}

    public List<Order> OrderHistory { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Waitingtoken> Waitingtokens { get; set; } = new List<Waitingtoken>();
}
