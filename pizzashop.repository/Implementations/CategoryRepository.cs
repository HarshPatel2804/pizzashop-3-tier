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
        var model = await _context.Categories.Where(u => u.Isdeleted != true).Select(u => new CategoryViewModel
        {
            Categoryid = u.Categoryid,
            Categoryname = u.Categoryname,
            Description = u.Description

        }).OrderBy(u => u.Categoryid).ToListAsync();

        return model;
    }

    public async Task AddCategoryAsync(Category model){
        await _context.Categories.AddAsync(model);
        await _context.SaveChangesAsync();

    }

     public async Task<List<SelectListItem>> GetCategoriesListAsync()
    {
        return _context.Categories.Where(u=>u.Isdeleted == false).OrderBy(u=>u.Categoryid).Select(c => new SelectListItem
            {
                Value = c.Categoryid.ToString(),
                Text = c.Categoryname
            }).ToList();
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
}
