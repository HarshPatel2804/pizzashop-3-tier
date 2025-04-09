using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface ITableSectionRepository
{
    Task<List<Section>> GetAllSetionsAsync();

    Task<(List<Table> tables, int totalTables)> GetTablesBySectionAsync(int Sectionid , int page, int pageSize, string search);

    Task AddSectionAsync(Section model);

    Task<Section> GetSectionById(int sectionId);

    Task EditSectionAsync(Section model);

    Task DeleteTablesBySectionAsync(int sectionId);

    Task<List<SelectListItem>> GetSectionListAsync();

    Task AddTableAsync(Table model);

    Task DeleteTableAsync(int tableId);

    Task<Table> GetTableById(int tableId);

    Task EditTableAsync(Table table);

    Task UpdateSortOrderOfSection(List<int> sortOrder);

    Task MassDeleteTable(List<int> Tableid);

    Task<Table> GetTableByName(TableViewModel model);

    Task<int> GetSectionIdWithLeastOrderField();
}
