using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;
public class SectionRawViewModel
{
    public int Sectionid { get; set; }

    public string Sectionname { get; set; } = null!;

    public string? Description { get; set; }

    public int TokenCount { get; set; }
}

