using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly PizzaShopContext _context;

    public CategoryRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
    {
        var model = await _context.Categories
       .FromSqlRaw("SELECT * FROM GetAllCategories()")
       .Select(u => new CategoryViewModel
       {
           Categoryid = u.Categoryid,
           Categoryname = u.Categoryname,
           Description = u.Description
       })
       .ToListAsync();

        return model;
    }

    public async Task AddCategoryAsync(Category model)
    {
        await _context.Categories.AddAsync(model);
        await _context.SaveChangesAsync();

    }

    public async Task<List<SelectListItem>> GetCategoriesListAsync()
    {
        return _context.Categories.Where(u => u.Isdeleted == false).OrderBy(u => u.Categoryid).Select(c => new SelectListItem
        {
            Value = c.Categoryid.ToString(),
            Text = c.Categoryname
        })
            .OrderBy(c => c.Text)
            .ToList();
    }


    public async Task<Category> GetCategoryByIdAsync(int categoryid)
    {
        return await _context.Categories.FirstOrDefaultAsync(u => u.Categoryid == categoryid);
    }

    public async Task EditCategoryAsync(Category model)
    {
        _context.Categories.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int Categoryid)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(u => u.Categoryid == Categoryid);
        category.Isdeleted = true;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<Category> GetCategoryByName(CategoryViewModel model)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Categoryname.ToLower() == model.Categoryname.ToLower() && c.Isdeleted != true && c.Categoryid != model.Categoryid);

        return category; 
    }

    public async Task UpdateSortOrderOfCategory(List<int> sortOrder)
    {

        for (int i = 0; i < sortOrder.Count; i++)
        {
            Category category = _context.Categories.FirstOrDefault(s => s.Categoryid == sortOrder[i]);

            if (category != null)
            {
                category.SortOrder = i + 1;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetFirstCategoryId()
    {
        var categortId = await _context.Categories
            .Where(s => s.SortOrder.HasValue && s.Isdeleted == false)
            .OrderBy(s => s.SortOrder.Value)
            .Select(s => s.Categoryid)
            .FirstOrDefaultAsync();

        return categortId;
    }
}
