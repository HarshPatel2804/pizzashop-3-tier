using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class TaxViewModel
{
    public int Taxid { get; set; }

    [Required(ErrorMessage = "Tax Name is required.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Tax Name must be between 2 and 30 characters.")]
    public string Taxname { get; set; } = null!;

    public bool Isenabled { get; set; }

    public bool Isdefault { get; set; }
    [Required(ErrorMessage = "Tax Type is required.")]
    public int TaxTypeId { get; set; }

    public string TaxTypeName { get; set; }

    [Required(ErrorMessage = "Tax Value is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Tax Value must not be negative")]
    public string Taxvalue { get; set; } = null!;

    public bool Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public virtual ICollection<Ordertaxmapping> Ordertaxmappings { get; set; } = new List<Ordertaxmapping>();

    [ForeignKey("TaxTypeId")]
    public TaxType TaxType { get; set; }

    public IEnumerable<SelectListItem> TaxTypes { get; set; }

}
