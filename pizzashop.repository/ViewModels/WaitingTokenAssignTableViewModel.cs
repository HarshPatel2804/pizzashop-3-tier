using System.ComponentModel.DataAnnotations;
using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public partial class WaitingTokenAssignTableViewModel
{
    public int? Waitingtokenid { get; set; }
    [Required]
    public int Sectionid { get; set; }

    public string? TableIds { get; set; }
    public List<Section>? SectionList { get; set; }
}