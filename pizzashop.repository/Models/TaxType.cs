using System.ComponentModel.DataAnnotations;

namespace pizzashop.repository.Models;

public partial class TaxType
{
    [Key]
public int TaxTypeId { get; set; }
[Required]
[MaxLength(50)]
public string TaxName { get; set; }
[MaxLength(20)]
public string? ShortCode { get; set; }

[System.Text.Json.Serialization.JsonIgnore]
public ICollection<Taxis> Taxes { get; set; } = new List<Taxis>();
}
