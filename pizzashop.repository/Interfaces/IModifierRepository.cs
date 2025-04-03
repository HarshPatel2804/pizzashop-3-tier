using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;
using Pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IModifierRepository
{
    Task<List<Modifiergroup>> GetAllModifierGroupAsync();

    Task<List<ModifierGroupModifierMapping>> GetModifierByGroupAsync(int ModifierGroupId);

    Task DeleteModifier(int modifierId);

    Task<List<SelectListItem>> GetAllmodifierGroups();

    Task<(List<Modifier> modifiers, int totalModifiers)> GetAllModifierAsync(int page, int pageSize, string search);

    Task<int> AddModifierGroup(Modifiergroup modifierGroup);

    bool UpdateModifierGroup(Modifiergroup modifierGroup);

    Task AddMappings(ModifierGroupModifierMapping mapping);

    IEnumerable<ModifierGroupModifierMapping> GetByModifierGroupId(int modifierGroupId);



}
