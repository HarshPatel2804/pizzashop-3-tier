using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

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

    public async Task DeleteModifier(int modifierId)
    {
        await _modifierRepository.DeleteModifier(modifierId);
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

    public async Task<List<ModifierViewModel>> GetModifiersByGroup(int ModifierGroupId)
    {
        var model = await _modifierRepository.GetModifierByGroupAsync(ModifierGroupId);

        var modifierModel = new List<ModifierViewModel>();

        foreach (var u in model)
        {
            var unitName = await _unitRepository.GetUnit(u.Unitid);

            modifierModel.Add(new ModifierViewModel
            {
                Modifiergroupid = u.Modifiergroupid,
                Modifierid = u.Modifierid,
                Modifiername = u.Modifiername,
                Rate = u.Rate,
                Quantity = u.Quantity,
                UnitName = unitName
            });
        }

        return modifierModel;
    }

    public async Task<List<ItemModifierGroupMapping>> GetItemModifierGroupsAsync(int itemId)
    {
        // Fetch modifier groups
        return await _itemRepository.GetItemModifierGroupsAsync(itemId);
    }

}
