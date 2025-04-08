using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;
using Pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IModifierRepository
{
    Task<List<Modifiergroup>> GetAllModifierGroupAsync();

    Task<Modifiergroup> GetModifierGroupByIdAsync(int modifierGroupId);

    Task<List<ModifierGroupModifierMapping>> GetModifierByGroupAsync(int ModifierGroupId);

    Task DeleteModifier(int modifierId , int modifierGroupId);

    Task<List<SelectListItem>> GetAllmodifierGroups();

    Task<(List<Modifier> modifiers, int totalModifiers)> GetAllModifierAsync(int page, int pageSize, string search);

    Task<int> AddModifierGroup(Modifiergroup modifierGroup);

    bool UpdateModifierGroup(Modifiergroup modifierGroup);

    Task AddMappings(ModifierGroupModifierMapping mapping);

    IEnumerable<ModifierGroupModifierMapping> GetByModifierGroupId(int modifierGroupId);

    Task<Modifier> GetModifierByIdAsync(int modifierId);

    Task DeleteMappings(int modifierGroupId);

    Task<int> AddModifier(Modifier modifier);

    IEnumerable<ModifierGroupModifierMapping> GetMappingsByModifierId(int modifierId);

    Task UpdateModifier(Modifier modifier);

    Task RemoveMappings(int modifierId);

}
