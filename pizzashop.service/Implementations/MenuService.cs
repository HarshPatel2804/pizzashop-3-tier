using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using Pizzashop.repository.Models;

namespace pizzashop.service.Implementations;

public class MenuService : IMenuService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IItemRepository _itemRepository;

    private readonly IModifierRepository _modifierRepository;

    private readonly IUnitRepository _unitRepository;

    private readonly IImageService _imageService;

    public MenuService(ICategoryRepository categoryRepository, IItemRepository itemRepository, IModifierRepository modifierRepository, IUnitRepository unitRepository, IImageService imageService)
    {
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;
        _modifierRepository = modifierRepository;
        _unitRepository = unitRepository;
        _imageService = imageService;
    }

    public async Task AddCategory(CategoryViewModel model)
    {
        var Category = new Category
        {
            Categoryname = model.Categoryname,
            Description = model.Description,
        };
        await _categoryRepository.AddCategoryAsync(Category);
    }

    public async Task DeleteCategory(int Categoryid)
    {
        await _categoryRepository.DeleteCategoryAsync(Categoryid);

        await _itemRepository.DeleteItemsByCategory(Categoryid);
    }

    public async Task DeleteItem(int Itemid)
    {
        await _itemRepository.DeleteItem(Itemid);
    }

    public async Task DeleteModifier(int modifierId, int modifierGroupId)
    {
        await _modifierRepository.DeleteModifier(modifierId, modifierGroupId);
    }

    public async Task EditCategory(CategoryViewModel model)
    {
        // Console.WriteLine(model.Description + "Description");
        var category = await _categoryRepository.GetCategoryByIdAsync(model.Categoryid);
        category.Categoryname = model.Categoryname;
        category.Description = model.Description;
        await _categoryRepository.EditCategoryAsync(category);
    }

    public async Task<List<CategoryViewModel>> GetAllCategories()
    {
        return await _categoryRepository.GetAllCategoryAsync();
    }

    public async Task<List<ModifierGroupViewModel>> GetAllmodifiergroups()
    {
        var model = await _modifierRepository.GetAllModifierGroupAsync(); ;
        var viewModel = model.Select(u => new ModifierGroupViewModel
        {
            Modifiergroupid = u.Modifiergroupid,
            Modifiergroupname = u.Modifiergroupname,
            Description = u.Description,

        }).ToList();

        return viewModel;
    }


    public async Task<CategoryViewModel> GetCategoryById(int categoryId)
    {
        var model = await _categoryRepository.GetCategoryByIdAsync(categoryId);

        var viewModel = new CategoryViewModel
        {
            Categoryid = model.Categoryid,
            Categoryname = model.Categoryname,
            Description = model.Description
        };

        return viewModel;
    }

    public async Task<AddEditItemViewModel> GetItemDetails()
    {
        var model = new AddEditItemViewModel
        {
            Category = await _categoryRepository.GetCategoriesListAsync(),
            Units = await _unitRepository.GetUnitsListAsync(),
            ModifierGroups = await _modifierRepository.GetAllmodifierGroups()
        };
        return model;
    }
    public async Task<ModifierViewModel> GetModifierGroups()
    {
        var model = new ModifierViewModel
        {
            ModifierGroups = await _modifierRepository.GetAllmodifierGroups(),
            Units = await _unitRepository.GetUnitsListAsync(),
        };
        return model;
    }

    public async Task<AddEditItemViewModel> GetEditItemDetails(int itemId)
    {
        var item = await _itemRepository.GetItemById(itemId);
        var model = new AddEditItemViewModel
        {
            Category = await _categoryRepository.GetCategoriesListAsync(),
            Units = await _unitRepository.GetUnitsListAsync(),
            ModifierGroups = await _modifierRepository.GetAllmodifierGroups(),
            Itemid = item.Itemid,
            Itemname = item.Itemname,
            Categoryid = item.Categoryid,
            Rate = item.Rate,
            Quantity = item.Quantity,
            Unitid = item.Unitid,
            Isavailable = (bool)item.Isavailable,
            Taxpercentage = item.Taxpercentage,
            Shortcode = item.Shortcode,
            Isdefaulttax = (bool)item.Isdefaulttax,
            Itemimg = item.Itemimg,
            Description = item.Description,
            ItemType = item.Itemtype
        };
        return model;
    }

    public async Task<(List<ItemViewModel> itemModel, int totalItems, int totalPages)> GetItemsByCategory(int CategoryId, int page, int pageSize, string search)
    {
        var (model, totalItems) = await _itemRepository.GetItemsByCategoryAsync(CategoryId, page, pageSize, search);

        int totalPages = (int)System.Math.Ceiling((double)totalItems / pageSize);

        var itemModel = model.Select(u => new ItemViewModel
        {
            Itemimg = u.Itemimg,
            Categoryid = u.Categoryid,
            Itemid = u.Itemid,
            Itemname = u.Itemname,
            Rate = u.Rate,
            Isavailable = u.Isavailable,
            Quantity = u.Quantity,
            Itemtype = u.Itemtype
        }).ToList();

        return (itemModel, totalItems, totalPages);
    }

    public async Task AddItemAsync(AddEditItemViewModel addEditItemViewModel, IFormFile Itemimg)
    {
        var items = new Item
        {
            Categoryid = addEditItemViewModel.Categoryid,
            Itemname = addEditItemViewModel.Itemname,
            Itemtype = addEditItemViewModel.ItemType,
            Rate = addEditItemViewModel.Rate,
            Quantity = addEditItemViewModel.Quantity,
            Unitid = addEditItemViewModel.Unitid,
            Taxpercentage = addEditItemViewModel.Taxpercentage,
            Shortcode = addEditItemViewModel.Shortcode,
            Description = addEditItemViewModel.Description,
            Isavailable = addEditItemViewModel.Isavailable,
            Isdefaulttax = addEditItemViewModel.Isdefaulttax
        };
        if (Itemimg != null)
        {
            var Itemimage = await _imageService.GiveImagePath(Itemimg);
            items.Itemimg = Itemimage;
        }
        var itemId = await _itemRepository.AddItemsAsync(items);

        if (addEditItemViewModel.SelectedModifierGroups != null)
        {
            var itemModifierGroupMappings = new List<Itemmodifiergroupmap>();

            foreach (var modifierGroup in addEditItemViewModel.SelectedModifierGroups)
            {
                var itemModifierGroupMapping = new Itemmodifiergroupmap
                {
                    Itemid = itemId,
                    Modifiergroupid = modifierGroup.Modifiergroupid,
                    Minselectionrequired = modifierGroup.Minselectionrequired,
                    Maxselectionallowed = modifierGroup.Maxselectionallowed
                };

                itemModifierGroupMappings.Add(itemModifierGroupMapping);
            }
            await _itemRepository.AddItemModifierGroupMappingsAsync(itemModifierGroupMappings);
        }
    }

    public async Task EditItemAsync(AddEditItemViewModel addEditItemViewModel, IFormFile Itemimg)
    {
        var item = await _itemRepository.GetItemById(addEditItemViewModel.Itemid);

        item.Categoryid = addEditItemViewModel.Categoryid;
        item.Itemname = addEditItemViewModel.Itemname;
        item.Itemtype = addEditItemViewModel.ItemType;
        item.Rate = addEditItemViewModel.Rate;
        item.Quantity = addEditItemViewModel.Quantity;
        item.Unitid = addEditItemViewModel.Unitid;
        item.Taxpercentage = addEditItemViewModel.Taxpercentage;
        item.Shortcode = addEditItemViewModel.Shortcode;
        item.Description = addEditItemViewModel.Description;
        item.Isavailable = addEditItemViewModel.Isavailable;
        item.Isdefaulttax = addEditItemViewModel.Isdefaulttax;

        if (Itemimg != null)
        {
            var Itemimage = await _imageService.GiveImagePath(Itemimg);
            item.Itemimg = Itemimage;
        }
        await _itemRepository.EditItemAsync(item);

        await _itemRepository.RemoveItemModifierGroupMappingsAsync(addEditItemViewModel.Itemid);

        // Add new modifier group mappings
        if (addEditItemViewModel.SelectedModifierGroups != null)
        {
            var itemModifierGroupMappings = new List<Itemmodifiergroupmap>();

            foreach (var modifierGroup in addEditItemViewModel.SelectedModifierGroups)
            {
                var itemModifierGroupMapping = new Itemmodifiergroupmap
                {
                    Itemid = addEditItemViewModel.Itemid,
                    Modifiergroupid = modifierGroup.Modifiergroupid,
                    Minselectionrequired = modifierGroup.Minselectionrequired,
                    Maxselectionallowed = modifierGroup.Maxselectionallowed
                };

                itemModifierGroupMappings.Add(itemModifierGroupMapping);
            }
            await _itemRepository.AddItemModifierGroupMappingsAsync(itemModifierGroupMappings);
        }
    }

    public async Task<(List<ModifierViewModel> modifiers, int totalModifiers, int totalPages)> GetModifiersByGroup(int ModifierGroupId, int page, int pageSize, string search)
    {
        var (modifiers, totalModifiers) = await _modifierRepository.GetModifierByGroupAsync(ModifierGroupId, page, pageSize, search);

        var modifierModel = new List<ModifierViewModel>();

        foreach (var u in modifiers)
        {
            var unitName = await _unitRepository.GetUnit(u.Modifier.Unitid);

            modifierModel.Add(new ModifierViewModel
            {
                Modifiergroupid = u.Modifier.Modifiergroupid,
                Modifierid = u.Modifier.Modifierid,
                Modifiername = u.Modifier.Modifiername,
                Rate = u.Modifier.Rate,
                Quantity = u.Modifier.Quantity,
                UnitName = unitName
            });
        }

        int totalPages = (int)System.Math.Ceiling((double)totalModifiers / pageSize);

        return (modifierModel, totalModifiers, totalPages);
    }

    public async Task<List<ItemModifierGroupMapping>> GetItemModifierGroupsAsync(int itemId)
    {
        // Fetch modifier groups
        return await _itemRepository.GetItemModifierGroupsAsync(itemId);
    }

    public async Task<(List<Modifier> modifiers, int totalModifiers, int totalPages)> GetModifierList(int page, int pageSize, string search)
    {
        var (modifiers, totalModifiers) = await _modifierRepository.GetAllModifierAsync(page, pageSize, search);

        int totalPages = (int)System.Math.Ceiling((double)totalModifiers / pageSize);

        return (modifiers, totalModifiers, totalPages);
    }

    public async Task<int> AddModifierGroup(ModifierGroupViewModel model)
    {
        // Create new modifier group
        var modifierGroup = new Modifiergroup
        {
            Modifiergroupname = model.Modifiergroupname,
            Description = model.Description,
            Isdeleted = false,
            Createdat = DateTime.Now,
            Modifiedat = DateTime.Now,
        };

        int modifierGroupId = await _modifierRepository.AddModifierGroup(modifierGroup);

        // Add mappings for each selected modifier
        if (model.SelectedModifierIds != null && model.SelectedModifierIds.Count > 0)
        {
            foreach (var modifierId in model.SelectedModifierIds)
            {
                var mapping = new ModifierGroupModifierMapping
                {
                    ModifierGroupId = modifierGroupId,
                    ModifierId = modifierId
                };

                await _modifierRepository.AddMappings(mapping);
            }
        }

        return modifierGroupId;
    }
    public async Task<int> EditModifierGroup(ModifierGroupViewModel model)
    {
        // Create new modifier group
        var modifierGroup = new Modifiergroup
        {
            Modifiergroupid = model.Modifiergroupid,
            Modifiergroupname = model.Modifiergroupname,
            Description = model.Description,
            Modifiedat = DateTime.Now,
        };

        _modifierRepository.UpdateModifierGroup(modifierGroup);
        await _modifierRepository.DeleteMappings(model.Modifiergroupid);

        // Add mappings for each selected modifier
        if (model.SelectedModifierIds != null && model.SelectedModifierIds.Count > 0)
        {
            foreach (var modifierId in model.SelectedModifierIds)
            {
                var mapping = new ModifierGroupModifierMapping
                {
                    ModifierGroupId = model.Modifiergroupid,
                    ModifierId = modifierId
                };

                await _modifierRepository.AddMappings(mapping);
            }
        }

        return model.Modifiergroupid;
    }

    public async Task<ModifierGroupViewModel> GetSelectedModifiers(int modifierGroupId)
    {
        var Groupdata = await _modifierRepository.GetModifierGroupByIdAsync(modifierGroupId);
        var modifiers = _modifierRepository.GetByModifierGroupId(modifierGroupId);
        var modifierIds = new List<Modifier>();

        foreach (var id in modifiers)
        {
            var modifier = await _modifierRepository.GetModifierByIdAsync(id.ModifierId);
            modifierIds.Add(modifier);
        }

        var model = new ModifierGroupViewModel
        {
            Modifiergroupname = Groupdata.Modifiergroupname,
            Modifiergroupid = modifierGroupId,
            Description = Groupdata.Description,
            SelectedModifiers = modifierIds
        };

        return model;
    }

    public async Task<int> SaveModifier(ModifierViewModel model)
    {
        // Create new modifier group
        var modifier = new Modifier
        {
            Modifiergroupid = model.SelectedModifierGroups[0],
            Modifiername = model.Modifiername,
            Description = model.Description,
            Isdeleted = false,
            Createdat = DateTime.Now,
            Modifiedat = DateTime.Now,
            Unitid = model.Unitid,
            Rate = model.Rate,
            Quantity = model.Quantity
        };

        int modifierId = await _modifierRepository.AddModifier(modifier);

        if (model.SelectedModifierGroups != null && model.SelectedModifierGroups.Count > 0)
        {
            foreach (var modifierGroupId in model.SelectedModifierGroups)
            {
                var mapping = new ModifierGroupModifierMapping
                {
                    ModifierGroupId = modifierGroupId,
                    ModifierId = modifierId
                };

                await _modifierRepository.AddMappings(mapping);
            }
        }

        return modifier.Modifiergroupid;
    }

    public async Task<ModifierViewModel> GetModifierDetails(int modifierId)
    {
        Modifier modifier = await _modifierRepository.GetModifierByIdAsync(modifierId);

        var mappings = _modifierRepository.GetMappingsByModifierId(modifierId);

        ModifierViewModel model = new ModifierViewModel
        {
            Modifierid = modifier.Modifierid,
            Modifiername = modifier.Modifiername,
            Rate = modifier.Rate,
            Quantity = modifier.Quantity,
            Description = modifier.Description,
            Unitid = modifier.Unitid,
            ModifierGroups = await _modifierRepository.GetAllmodifierGroups(),
            Units = await _unitRepository.GetUnitsListAsync()
        };

        var modifierGroupIds = new List<Modifiergroup>();
        var Ids = new List<int>();

        foreach (var mapping in mappings)
        {
            Ids.Add(mapping.ModifierGroupId);
            var modifierGroup = await _modifierRepository.GetModifierGroupByIdAsync(mapping.ModifierGroupId);
            modifierGroupIds.Add(modifierGroup);
        }

        model.Groups = modifierGroupIds;
        model.SelectedModifierGroups = Ids;

        return model;
    }

    public async Task EditModifier(ModifierViewModel model)
    {
        // Create new modifier group
        var modifier = await _modifierRepository.GetModifierByIdAsync(model.Modifierid);

        modifier.Modifiername = model.Modifiername;
        modifier.Rate = model.Rate;
        modifier.Quantity = model.Quantity;
        modifier.Description = model.Description;
        modifier.Unitid = model.Unitid;
        modifier.Modifiergroupid = model.SelectedModifierGroups[0];

        await _modifierRepository.UpdateModifier(modifier);

        await _modifierRepository.RemoveMappings(model.Modifierid);

        if (model.SelectedModifierGroups != null && model.SelectedModifierGroups.Count > 0)
        {
            foreach (var modifierGroupId in model.SelectedModifierGroups)
            {
                var mapping = new ModifierGroupModifierMapping
                {
                    ModifierGroupId = modifierGroupId,
                    ModifierId = model.Modifierid
                };

                await _modifierRepository.AddMappings(mapping);
            }
        }
    }

    public async Task DeleteMultipleItems(List<int> itemIds)
    {
        await _itemRepository.MassDeleteItem(itemIds);
    }

    public async Task DeleteMultipleModifiers(List<int> itemIds, int modifierGroupId)
    {
        foreach (var id in itemIds)
        {
            await _modifierRepository.DeleteModifier(id, modifierGroupId);
        }
    }

    public async Task<Category> GetCategoryByName(CategoryViewModel model)
    {
        return await _categoryRepository.GetCategoryByName(model);
    }

    public async Task<Modifiergroup> GetModifierGroupByName(string name, int id)
    {
        return await _modifierRepository.GetModifierGroupByName(name, id);
    }
    public async Task<Modifier> GetModifierByName(ModifierViewModel model)
    {
        return await _modifierRepository.GetModifierByName(model);
    }
    public async Task<Item> GetItemByName(AddEditItemViewModel model)
    {
        return await _itemRepository.GetItemByName(model);
    }

    public async Task<bool> UpdateItemAvailabilityAsync(int itemId, bool isAvailable)
    {
        return await _itemRepository.UpdateItemAvailabilityAsync(itemId, isAvailable);
    }

    public async Task<List<ModifierViewModel>> GetModifiersBymodifierGroup(int id)
    {
        var data = await _modifierRepository.GetModifiersBymodifierGroup(id);

        var Modifiers = data.Select(m => new ModifierViewModel
        {
            Modifierid = m.Modifierid,
            Modifiername = m.Modifiername,
            Rate = m.Rate,
            Quantity = m.Quantity,
        }).ToList();

        return Modifiers;

    }

    public async Task UpdateCategorySortOrder(List<int> sortOrder)
    {
        await _categoryRepository.UpdateSortOrderOfCategory(sortOrder);
    }

    public async Task UpdateModifierGroupSortOrder(List<int> sortOrder)
    {
        await _modifierRepository.UpdateSortOrderOfModifierGroup(sortOrder);
    }

    public async Task<int> FirstCategoryId()
    {
        return await _categoryRepository.GetFirstCategoryId();
    }

    public async Task DeleteModifiergroup(int modifierGroupId)
    {
        var mappings = _modifierRepository.GetByModifierGroupId(modifierGroupId);

        foreach (var mapping in mappings)
        {
            await _modifierRepository.DeleteModifier(mapping.ModifierId, modifierGroupId);
        }

        await _modifierRepository.DeleteModifierGroupAsync(modifierGroupId);
    }

    public async Task<List<ItemViewModel>> GetMenuItemsAsync(string categoryId, string searchText)
    {
        return await _itemRepository.GetMenuItemsbyCategoryAsync(categoryId, searchText);
    }

    public async Task<bool> ToggleFavoriteAsync(int itemId, bool isFavorite)
    {
        return await _itemRepository.ToggleFavoriteAsync(itemId, isFavorite);
    }
}
