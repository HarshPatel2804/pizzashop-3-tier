using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface ICategoryRepository
{
    Task<List<CategoryViewModel>> GetAllCategoryAsync();
    Task AddCategoryAsync(Category model);

    Task<Category> GetCategoryByIdAsync(int categoryid);

    Task EditCategoryAsync(Category model);

    Task DeleteCategoryAsync(int Categoryid);
}
