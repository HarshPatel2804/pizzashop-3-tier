namespace pizzashop.repository.ViewModels;
public class WaitingTokenWithCustomerViewModel
{
    public int Waitingtokenid { get; set; }
    public DateTime Createdat { get; set; }
    public int? Createdby { get; set; }
    public int Customerid { get; set; }
    public bool Isassigned { get; set; }
    public DateTime Modifiedat { get; set; }
    public int? Modifiedby { get; set; }
    public int Noofpeople { get; set; }
    public int Sectionid { get; set; }
    public string Customername { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phoneno { get; set; } = string.Empty;
    public int? Totalorder { get; set; }
    public int Visitcount { get; set; }
}