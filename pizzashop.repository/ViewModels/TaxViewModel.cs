using System.ComponentModel.DataAnnotations.Schema;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class TaxViewModel
{
    public int Taxid { get; set; }

    public string Taxname { get; set; } = null!;

    public bool? Isenabled { get; set; }

    public bool? Isdefault { get; set; }
    public int TaxTypeId { get; set; }

    public string TaxTypeName { get; set; }
    public string Taxvalue { get; set; } = null!;

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public virtual ICollection<Ordertaxmapping> Ordertaxmappings { get; set; } = new List<Ordertaxmapping>();

    [ForeignKey("TaxTypeId")]
    public TaxType TaxType { get; set; }

}
