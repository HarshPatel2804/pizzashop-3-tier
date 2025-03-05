namespace pizzashop.repository.ViewModels;

public class CategoryViewModel
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; } = null!;

    public string? Description { get; set; }

    public int Createdby { get; set; }

    public int Modifiedby { get; set; }
}
