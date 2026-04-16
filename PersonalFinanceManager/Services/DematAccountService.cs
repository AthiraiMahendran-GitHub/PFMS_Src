using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class DematAccountService
{
    private readonly AppDbContext _context;
    private readonly AccountService _accountService;

    public DematAccountService(AppDbContext context, AccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<List<DematAccount>> GetAllAsync()
    {
        return await _context.DematAccounts
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<DematAccount?> GetByIdAsync(int id)
    {
        return await _context.DematAccounts.FindAsync(id);
    }

    public async Task<DematAccount> CreateAsync(DematAccount dematAccount)
    {
        _context.DematAccounts.Add(dematAccount);
        await _context.SaveChangesAsync();
        return dematAccount;
    }

    public async Task UpdateAsync(DematAccount dematAccount)
    {
        _context.DematAccounts.Update(dematAccount);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var dematAccount = await _context.DematAccounts.FindAsync(id);
        if (dematAccount != null)
        {
            dematAccount.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<InvestmentHolding>> GetHoldingsAsync(int dematAccountId)
    {
        return await _context.InvestmentHoldings
            .Where(h => h.DematAccountId == dematAccountId && h.Quantity > 0)
            .OrderBy(h => h.Symbol)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalInvestedAsync(int dematAccountId)
    {
        var holdings = await GetHoldingsAsync(dematAccountId);
        return holdings.Sum(h => h.TotalInvested);
    }

    public async Task<decimal> GetCurrentValueAsync(int dematAccountId)
    {
        var holdings = await GetHoldingsAsync(dematAccountId);
        return holdings.Sum(h => h.CurrentValue);
    }

    public async Task<decimal> GetTotalProfitLossAsync(int dematAccountId)
    {
        var holdings = await GetHoldingsAsync(dematAccountId);
        return holdings.Sum(h => h.UnrealizedProfitLoss);
    }

    public async Task UpdateHoldingAsync(InvestmentHolding holding)
    {
        _context.InvestmentHoldings.Update(holding);
        await _context.SaveChangesAsync();
    }
}
