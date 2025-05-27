using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using Dapper;
using System.Data;


namespace pizzashop.repository.Implementations;

public class WaitingTokenRepository : IWaitingTokenRepository
{
    private readonly PizzaShopContext _context;

    public WaitingTokenRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<int> SaveWaitingToken(Waitingtoken model)
    {
        await _context.AddAsync(model);
        await _context.SaveChangesAsync();
        return model.Waitingtokenid;
    }

    public async Task<SaveWaitingTokenRawViewModel> AddWaitingTokenAsync(WaitingtokenViewModel model)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        const string query = "SELECT add_waiting_token(@p_email, @p_customername, @p_phoneno, @p_noofpeople, @p_sectionid)";

        var parameters = new
        {
            p_email = model.Email,
            p_customername = model.Customername,
            p_phoneno = model.Phoneno,
            p_noofpeople = model.Noofpeople,
            p_sectionid = model.Sectionid
        };

        var jsonResult = await connection.QuerySingleAsync<string>(query, parameters);

        var result = System.Text.Json.JsonSerializer.Deserialize<SaveWaitingTokenRawViewModel>(jsonResult);

        return result;
    }

    public async Task<IEnumerable<WaitingTokenWithCustomerViewModel>> GetAllWaitingTokensWithCustomer(int section)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        const string query = "SELECT * FROM get_waiting_token_with_customer(@sectionId)";

        var parameters = new { sectionId = section };

        return await connection.QueryAsync<WaitingTokenWithCustomerViewModel>(query, parameters);
    }
    public async Task<IEnumerable<Waitingtoken>> GetAllWaitingTokens(List<int> sectionIds)
    {
        var startDate = DateTime.Today;
        var endDate = startDate.Date.AddDays(1).AddTicks(-1);
        return await _context.Waitingtokens
            .Where(u => sectionIds.Contains(u.Sectionid) && u.Isassigned == false && u.Createdat >= startDate && u.Modifiedat <= endDate)
            .Include(w => w.Customer)
            .ToListAsync();
    }

    public async Task<bool> IsCustomerInWaitingList(int customerId)
    {
        var startDate = DateTime.Today;
        var endDate = startDate.Date.AddDays(1).AddTicks(-1);
        return await _context.Waitingtokens
            .AnyAsync(w => w.Customerid == customerId && w.Isassigned == false && w.Createdat >= startDate && w.Modifiedat <= endDate);
    }

    public async Task WaitingToAssign(int tokenId)
    {
        var token = await _context.Waitingtokens.FirstOrDefaultAsync(w => w.Waitingtokenid == tokenId);
        token.Isassigned = true;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int tokenId)
    {
        var connection = _context.Database.GetDbConnection();

    if (connection.State != ConnectionState.Open)
    {
        await connection.OpenAsync();
    }

    const string query = "SELECT delete_waiting_token(@p_token_id)";

    var parameters = new { p_token_id = tokenId };

    // Call the PostgreSQL function and get the result
    var result = await connection.QuerySingleAsync<bool>(query, parameters);

    return result;
    }

    public async Task<Waitingtoken?> GetTokenByIdWithCustomerAsync(int tokenId)
    {
        // return await _context.Waitingtokens
        //                      .Include(wt => wt.Customer)
        //                      .FirstOrDefaultAsync(wt => wt.Waitingtokenid == tokenId);

        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        const string query = "SELECT * FROM get_token_by_id_with_customer(@p_token_id)";

        var parameters = new { p_token_id = tokenId };

        var result = await connection.QuerySingleOrDefaultAsync(query, parameters);

        if (result == null)
        {
            return null;
        }

        return new Waitingtoken
        {
            Waitingtokenid = result.waitingtokenid,
            Noofpeople = result.noofpeople,
            Sectionid = result.sectionid,
            Isassigned = result.isassigned,
            Createdat = result.createdat,
            Modifiedat = result.modifiedat,
            Customer = new Customer
            {
                Customerid = result.customerid,
                Customername = result.customername,
                Email = result.email,
                Phoneno = result.phoneno
            }
        };
    }

    public async Task<Waitingtoken?> GetByIdAsync(int tokenId)
    {
        return await _context.Waitingtokens.FindAsync(tokenId);
    }

    public async Task<SaveWaitingTokenRawViewModel> UpdateWaitingTokenAsync(WaitingtokenViewModel token)
    {
        // _context.Waitingtokens.Update(token);
        // await _context.SaveChangesAsync();
        // return true;
        var connection = _context.Database.GetDbConnection();

    if (connection.State != ConnectionState.Open)
    {
        await connection.OpenAsync();
    }

    const string query = "SELECT * FROM update_waiting_token_details(@p_waitingtokenid, @p_noofpeople, @p_sectionid, @p_customername, @p_email, @p_phoneno)";

    var parameters = new
    {
        p_waitingtokenid = token.Waitingtokenid,
        p_noofpeople = token.Noofpeople,
        p_sectionid = token.Sectionid,
        p_customername = token.Customername,
        p_email = token.Email,
        p_phoneno = token.Phoneno
    };

    var resultJson = await connection.QuerySingleAsync<string>(query, parameters);

    var result = System.Text.Json.JsonSerializer.Deserialize<SaveWaitingTokenRawViewModel>(resultJson);

    return result;
    }

    public async Task<IEnumerable<Waitingtoken>> GetActiveWaitingTokensByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var inclusiveEndDate = endDate.Date.AddDays(1).AddTicks(-1);
        startDate = DateTime.Today;
        endDate = startDate.Date.AddDays(1).AddTicks(-1);

        return await _context.Waitingtokens
            .Where(wt => wt.Createdat >= startDate && wt.Createdat <= endDate &&
                         wt.Isassigned == false)
            .ToListAsync();
    }
}
