
using pizzashop.repository.Models;

namespace Pizzashop.repository.Models;
public class ModifierGroupModifierMapping
{
    public int ModifierGroupModifierMappingId { get; set; }
    public int ModifierGroupId { get; set; }
    public int ModifierId { get; set; }
    public Modifiergroup ModifierGroup { get; set; } = null!;
    public Modifier Modifier { get; set; } = null!;
}