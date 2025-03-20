using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface ITableSectionService
{
    Task<List<SectionViewModel>> GetAllSections();

    Task<(List<TableViewModel> tableModel, int totalTables, int totalPages)> GetTablesBySection(int sectionId , int page, int pageSize, string search);

    Task AddSection(SectionViewModel model);

    Task<SectionViewModel> GetSectionById(int sectionId);

    Task EditSection(SectionViewModel model);

    Task DeleteSection(int sectionId);

    Task<TableViewModel> GetSections();

    Task AddTable(TableViewModel model);

    Task DeleteTable(int tableId);
}
