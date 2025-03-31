using System;
using System.Collections.Generic;

namespace pizzashop.repository.Models;

public enum orderstatus
{
    InProgress = 1,
    Pending = 2,
    Completed=3,
    Cancelled=4,
    OnHold = 5,
    Failed = 6,

    Served = 7
}
public enum paymentmode
{
    Online = 0,
    Cash = 1,
    Card = 2
}
public partial class Order
{
    public int Orderid { get; set; }

    public DateTime? Orderdate { get; set; }

    public int Customerid { get; set; }

    public paymentmode Paymentmode { get; set; }

    public orderstatus OrderStatus {get; set;}

    public string? Orderwisecomment { get; set; }

    public short? Noofperson { get; set; }

    public decimal? Rating { get; set; }

    public decimal? Subamount { get; set; }

    public decimal? Discount { get; set; }

    public decimal? Totaltax { get; set; }

    public decimal Totalamount { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Customerreview> Customerreviews { get; set; } = new List<Customerreview>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Ordereditem> Ordereditems { get; set; } = new List<Ordereditem>();

    public virtual ICollection<Ordertable> Ordertables { get; set; } = new List<Ordertable>();

    public virtual ICollection<Ordertaxmapping> Ordertaxmappings { get; set; } = new List<Ordertaxmapping>();
}
