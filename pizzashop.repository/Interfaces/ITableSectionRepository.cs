using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface ITableSectionRepository
{
    Task<List<SectionRawViewModel>> GetAllSetionsAsync();

    Task<(List<Table> tables, int totalTables)> GetTablesBySectionAsync(int Sectionid , int page, int pageSize, string search);

    Task AddSectionAsync(Section model);

    Task<Section> GetSectionById(int sectionId);

    Task EditSectionAsync(Section model);

    Task DeleteTablesBySectionAsync(int sectionId);

    Task<List<SelectListItem>> GetSectionListAsync();

    Task<List<SelectListItem>> GetTableListAsync(int sectionId);

    Task AddTableAsync(Table model);

    Task DeleteTableAsync(int tableId);

    Task<Table> GetTableById(int tableId);

    Task EditTableAsync(Table table);

    Task UpdateSortOrderOfSection(List<int> sortOrder);

    Task MassDeleteTable(List<int> Tableid);

    Task<Table> GetTableByName(TableViewModel model);

    Task<Section> GetSectionByName(SectionViewModel model);

    Task<int> GetSectionIdWithLeastOrderField();

     Task<List<Section>> GetAllSectionsWithTablesAndOrdersAsync();

     Task AddOrderTables(List<Ordertable> orderTables);

     Task UpdateTableStatusToOccupied(int tableId);

     Task<bool> AnyTableOccupied(int sectionId);

     Task<bool> AreTablesOccupied(List<int> tableIds);

     Task<List<SelectListItem>> GetMultiTableListAsync(List<int> sectionId);
}
