using pizzashop.repository.Models;
namespace pizzashop.repository.ViewModels;

public class OrderViewModel
{
    public int Orderid { get; set; }
    public DateTime? Orderdate { get; set; }
    public string Customername { get; set; }
    public paymentmode Paymentmode { get; set; }
    public orderstatus OrderStatus{get;set;}
    public decimal? Rating { get; set; }
    public decimal Totalamount { get; set; }
}
