using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface ITableSectionService
{
    Task<List<SectionViewModel>> GetAllSections();

    Task<(List<TableViewModel> tableModel, int totalTables, int totalPages)> GetTablesBySection(int sectionId , int page, int pageSize, string search);

    Task AddSection(SectionViewModel model);

    Task<SectionViewModel> GetSectionById(int sectionId);

    Task EditSection(SectionViewModel model);

    Task<(bool Success, string Message)> DeleteSection(int sectionId);

    Task<TableViewModel> GetTableviewModel(int sectionId);

    Task AddTable(TableViewModel model);

    Task<(string message , bool success)> DeleteTable(int tableId);

    Task<TableViewModel> GetTableById(int tableId);

    Task<(string message , bool success)> EditTable(TableViewModel tableViewModel);

    Task UpdateSectionSortOrder(List<int> sortOrder);

    Task<(bool success, string message)> DeleteMultipleTables(List<int> tableIds);

    Task<Table> GetTableByName(TableViewModel model);

    Task<Section> GetSectionByName(SectionViewModel model);

    Task<int> FirstSectionId();

    Task<List<OrderSectionViewModel>> GetAllSectionsWithTablesAsync();

    Task<WaitingtokenViewModel> GetSectionList();

    Task<string> AssignTable(AssignTableViewModel model);
}
