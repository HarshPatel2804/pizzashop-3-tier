using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public partial class TableViewModel
{
    public int Tableid { get; set; }

    [Required(ErrorMessage = "Table Name is required.")]
    [StringLength(10, MinimumLength = 2, ErrorMessage = "Table name must be between 2 and 10 characters.")]
    public string Tablename { get; set; } = null!;

    [Required(ErrorMessage = "Section is required.")]
    public int? Sectionid { get; set; }

    [Required(ErrorMessage = "Capacity is required.")]
    public decimal Capacity { get; set; }

    [Required(ErrorMessage = "Table status is required.")]
    public tablestatus Tablestatus { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public virtual ICollection<Ordertable> Ordertables { get; set; } = new List<Ordertable>();

    public virtual Section? Section { get; set; }

    public List<SelectListItem> Sections { get; set; }

    public virtual ICollection<Waitingtablemapping> Waitingtablemappings { get; set; } = new List<Waitingtablemapping>();
}
