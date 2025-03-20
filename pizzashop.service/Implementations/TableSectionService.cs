using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class TableSectionService : ITableSectionService
{
    private readonly ITableSectionRepository _tableSectionRepository;

    public TableSectionService(ITableSectionRepository tableSectionRepository)
    {
        _tableSectionRepository = tableSectionRepository;
    }

    public async Task<List<SectionViewModel>> GetAllSections()
    {
        var model = await _tableSectionRepository.GetAllSetionsAsync();

        var viewModel = model.Select(u => new SectionViewModel
        {
            Sectionid = u.Sectionid ,
            Sectionname = u.Sectionname ,
            Description = u.Description

        }).ToList();

        return viewModel;
       
    }

     public async Task<(List<TableViewModel> tableModel, int totalTables, int totalPages)> GetTablesBySection(int sectionId , int page, int pageSize, string search)
    {
        var (model ,  totalTables) = await _tableSectionRepository.GetTablesBySectionAsync(sectionId,page, pageSize, search);

        int totalPages = (int)System.Math.Ceiling((double)totalTables / pageSize);

        var tableModel = model.Select(u => new TableViewModel
        {
            Tableid = u.Tableid , 
            Tablename = u.Tablename ,
            Tablestatus = u.Tablestatus ,
            Capacity = u.Capacity ,
            Sectionid = u.Sectionid
        }).ToList();

        return (tableModel, totalTables, totalPages);
    }

    public async Task AddSection(SectionViewModel model)
    {
        var section = new Section
        {
            Sectionname = model.Sectionname,
            Description = model.Description,
        };
        await _tableSectionRepository.AddSectionAsync(section);
    }

    public async Task<SectionViewModel> GetSectionById(int sectionId)
    {
        var model = await _tableSectionRepository.GetSectionById(sectionId);

        var viewModel = new SectionViewModel
        {
            Sectionid = model.Sectionid,
            Sectionname = model.Sectionname,
            Description = model.Description
        };

        return viewModel;
    }

    public async Task EditSection(SectionViewModel model)
    {
        // Console.WriteLine(model.Description + "Description");
        var section = await _tableSectionRepository.GetSectionById(model.Sectionid);
        section.Sectionname = model.Sectionname;
        section.Description = model.Description;
        await _tableSectionRepository.EditSectionAsync(section);
    }

     public async Task DeleteSection(int sectionId)
    {
        var section = await _tableSectionRepository.GetSectionById(sectionId);
        section.Isdeleted = true;
        await _tableSectionRepository.EditSectionAsync(section);

        await _tableSectionRepository.DeleteTablesBySectionAsync(sectionId);
    }

    public async Task<TableViewModel> GetSections()
    {   
        var model = new TableViewModel
        {
            Sections = await _tableSectionRepository.GetSectionListAsync()
        };
        return model;
    }

     public async Task AddTable(TableViewModel model)
    {
        var Table = new Table
        {
            Tablename = model.Tablename ,
            Tablestatus = model.Tablestatus ,
            Sectionid = model.Sectionid ,
            Capacity = model.Capacity
        };
        await _tableSectionRepository.AddTableAsync(Table);
    }

    public async Task DeleteTable(int tableId){
        await _tableSectionRepository.DeleteTableAsync(tableId);
    }

}
