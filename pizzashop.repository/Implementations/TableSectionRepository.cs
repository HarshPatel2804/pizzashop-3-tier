using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class TableSectionRepository : ITableSectionRepository
{
    private readonly PizzaShopContext _context;

    public TableSectionRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<List<Section>> GetAllSetionsAsync()
    {
        return await _context.Sections.Where(u => u.Isdeleted != true).OrderBy(u=> u.OrderField).ToListAsync();
    }

    public async Task<(List<Table> tables, int totalTables)> GetTablesBySectionAsync(int Sectionid , int page, int pageSize, string search)
    {
        var query = _context.Tables
        .Where(u => u.Sectionid == Sectionid && u.Isdeleted != true)
        .Where(u => string.IsNullOrEmpty(search) ||
                        u.Tablename.ToLower().Contains(search.ToLower()));

        int totalTables = await query.CountAsync();

        var tables = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (tables, totalTables);
    }

    public async Task AddSectionAsync(Section model){
        await _context.Sections.AddAsync(model);
        await _context.SaveChangesAsync();

    }

     public async Task<Section> GetSectionById(int sectionId)
    {
        return await _context.Sections.FirstOrDefaultAsync(u => u.Sectionid == sectionId);
    }

    public async Task EditSectionAsync(Section model)
    {
        _context.Sections.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTablesBySectionAsync(int sectionId)
    {
        await _context.Tables.Where(u => u.Sectionid == sectionId).ForEachAsync(u => u.Isdeleted = true);
        await _context.SaveChangesAsync();
    }

    public async Task<List<SelectListItem>> GetSectionListAsync()
    {
        return _context.Sections.Where(u=> u.Isdeleted != true).OrderBy(u=>u.Sectionid).Select(c => new SelectListItem
            {
                Value = c.Sectionid.ToString(),
                Text = c.Sectionname
            }).ToList();
    }

    public async Task AddTableAsync(Table model){
        await _context.Tables.AddAsync(model);
        await _context.SaveChangesAsync();

    }

     public async Task DeleteTableAsync(int tableId)
    {
        await _context.Tables.Where(u => u.Tableid == tableId).ForEachAsync(u => u.Isdeleted = true);
        await _context.SaveChangesAsync();
    }

    public async Task<Table> GetTableById(int tableId)
    {
        return await _context.Tables.FirstOrDefaultAsync(u => u.Tableid == tableId);
    }

    public async Task EditTableAsync(Table table)
    {
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSortOrderOfSection(List<int> sortOrder){

        for(int i = 0 ; i < sortOrder.Count ; i++){
            Section section = _context.Sections.FirstOrDefault(s => s.Sectionid == sortOrder[i]);

            if(section != null){
                section.OrderField = i+1;
            }
        }
        await _context.SaveChangesAsync();
    }

}
