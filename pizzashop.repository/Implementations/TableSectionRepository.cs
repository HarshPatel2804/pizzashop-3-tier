using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class TableSectionRepository : ITableSectionRepository
{
    private readonly PizzaShopContext _context;

    public TableSectionRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<List<SectionRawViewModel>> GetAllSetionsAsync()
    {
       var result = await _context.Set<SectionRawViewModel>()
        .FromSqlRaw("SELECT * FROM get_all_sections_with_tokens()")
        .ToListAsync();

        return result;
    }

    public async Task<(List<Table> tables, int totalTables)> GetTablesBySectionAsync(int Sectionid, int page, int pageSize, string search)
    {
        var query = _context.Tables
        .Where(u => u.Sectionid == Sectionid && u.Isdeleted != true)
        .Where(u => string.IsNullOrEmpty(search) ||
                        u.Tablename.ToLower().Contains(search.ToLower()));

        int totalTables = await query.CountAsync();

        if(pageSize == 0){
            var allTables = await query
            .ToListAsync();
            return (allTables, totalTables);
        }
        var tables = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (tables, totalTables);
    }

    public async Task AddSectionAsync(Section model)
    {
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

    public async Task<bool> AnyTableOccupied(int sectionId)
{
    return await _context.Tables
        .AnyAsync(t => t.Sectionid == sectionId && 
                      t.Isdeleted != true && 
                      (t.Tablestatus == tablestatus.Occupied || t.Tablestatus == tablestatus.Reserved));
}

    public async Task DeleteTablesBySectionAsync(int sectionId)
    {
        await _context.Tables.Where(u => u.Sectionid == sectionId).ForEachAsync(u => u.Isdeleted = true);
        await _context.SaveChangesAsync();
    }

    public async Task<List<SelectListItem>> GetSectionListAsync()
    {
        return _context.Sections.Where(u => u.Isdeleted != true).OrderBy(u => u.Sectionid).Select(c => new SelectListItem
        {
            Value = c.Sectionid.ToString(),
            Text = c.Sectionname
        })
        .OrderBy(c => c.Text)
        .ToList();
    }
    public async Task<List<SelectListItem>> GetTableListAsync(int sectionId)
    {
        return _context.Tables.Where(u => u.Isdeleted != true && u.Tablestatus == tablestatus.Available && u.Sectionid == sectionId).Select(c => new SelectListItem
        {
            Value = c.Tableid.ToString(),
            Text = c.Tablename
        })
        .OrderBy(c => c.Text)
        .ToList();
    }
    public async Task<List<SelectListItem>> GetMultiTableListAsync(List<int> sectionId)
    {
        return _context.Tables.Where(u => u.Isdeleted != true && u.Tablestatus == tablestatus.Available && sectionId.Contains((int)u.Sectionid)).Select(c => new SelectListItem
        {
            Value = c.Tableid.ToString(),
            Text = c.Tablename
        })
        .OrderBy(c => c.Text)
        .ToList();
    }

    public async Task AddTableAsync(Table model)
    {
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

    public async Task UpdateSortOrderOfSection(List<int> sortOrder)
    {

        for (int i = 0; i < sortOrder.Count; i++)
        {
            Section section = _context.Sections.FirstOrDefault(s => s.Sectionid == sortOrder[i]);

            if (section != null)
            {
                section.OrderField = i + 1;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AreTablesOccupied(List<int> tableIds)
{
    return await _context.Tables
        .AnyAsync(t => tableIds.Contains(t.Tableid) && t.Tablestatus == tablestatus.Occupied);
}

    public async Task MassDeleteTable(List<int> Tableid)
    {
        await _context.Tables.Where(u => Tableid.Contains(u.Tableid)).ForEachAsync(u => u.Isdeleted = true);
        await _context.SaveChangesAsync();
    }

    public async Task<Table> GetTableByName(TableViewModel model)
    {
        return await _context.Tables
            .FirstOrDefaultAsync(mg =>
                mg.Tablename.ToLower().Trim() == model.Tablename.ToLower().Trim() &&
                mg.Tableid != model.Tableid &&
                mg.Isdeleted != true);
    }

     public async Task<Section> GetSectionByName(SectionViewModel model)
    {
        return await _context.Sections
            .FirstOrDefaultAsync(mg =>
                mg.Sectionname.ToLower().Trim() == model.Sectionname.ToLower().Trim() &&
                mg.Sectionid != model.Sectionid &&
                mg.Isdeleted != true);
    }

    public async Task<int> GetSectionIdWithLeastOrderField()
    {
        var sectionId = await _context.Sections
            .Where(s => s.OrderField.HasValue && s.Isdeleted == false)
            .OrderBy(s => s.OrderField.Value)
            .Select(s => s.Sectionid)
            .FirstOrDefaultAsync();

        return sectionId;
    }

    public async Task<List<Section>> GetAllSectionsWithTablesAndOrdersAsync()
    {
        return await _context.Sections
            .Where(s => s.Isdeleted != true)
            .Include(s => s.Tables.Where(t => t.Isdeleted != true))
                .ThenInclude(t => t.Ordertables)
                    .ThenInclude(ot => ot.Order)
            .OrderBy(s => s.OrderField)
            .ToListAsync();
    }

    public async Task AddOrderTables(List<Ordertable> orderTables)
    {
        await _context.Ordertables.AddRangeAsync(orderTables);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTableStatusToOccupied(int tableId)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Tableid == tableId);
        if (table != null)
        {
            table.Tablestatus = tablestatus.Occupied; 
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
        }
    }

}
