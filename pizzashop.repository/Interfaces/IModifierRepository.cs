using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IModifierRepository
{
    Task<List<Modifiergroup>> GetAllModifierGroupAsync();

    Task<List<Modifier>> GetModifierByGroupAsync(int ModifierGroupId);

    Task DeleteModifier(int modifierId);

    Task<List<SelectListItem>> GetAllmodifierGroups();
}
