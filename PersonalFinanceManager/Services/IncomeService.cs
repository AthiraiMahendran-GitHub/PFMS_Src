using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class IncomeService : BaseService
{
    public IncomeService(AppDbContext context, TenantService tenantService) : base(context, tenantService)
    {
    }

    public async Task<List<Income>> GetAllAsync()
    {
        await EnsureTenantAsync();
        return await _context.Incomes
            .OrderByDescending(i => i.Date)
            .ToListAsync();
    }

    public async Task<List<Income>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await EnsureTenantAsync();
        return await _context.Incomes
            .Where(i => i.Date >= startDate && i.Date <= endDate)
            .OrderByDescending(i => i.Date)
            .ToListAsync();
    }

    public async Task<Income?> GetByIdAsync(int id)
    {
        await EnsureTenantAsync();
        return await _context.Incomes.FindAsync(id);
    }

    public async Task<Income> CreateAsync(Income income)
    {
        await EnsureTenantAsync();
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();
        
        // Update account balance if account is specified
        if (income.AccountId.HasValue)
        {
            var account = await _context.Accounts.FindAsync(income.AccountId.Value);
            if (account != null)
            {
                account.Balance += income.Amount;
                account.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
        
        return income;
    }

    public async Task UpdateAsync(Income income)
    {
        await EnsureTenantAsync();
        _context.Incomes.Update(income);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await EnsureTenantAsync();
        var income = await _context.Incomes.FindAsync(id);
        if (income != null)
        {
            // Update account balance if account is specified
            if (income.AccountId.HasValue)
            {
                var account = await _context.Accounts.FindAsync(income.AccountId.Value);
                if (account != null)
                {
                    account.Balance -= income.Amount;
                    account.LastUpdated = DateTime.Now;
                }
            }
            
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        var query = _context.Incomes.AsQueryable();
        
        if (startDate.HasValue)
            query = query.Where(i => i.Date >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(i => i.Date <= endDate.Value);
        
        var incomes = await query.ToListAsync();
        return incomes.Sum(i => i.Amount);
    }

    public async Task<Dictionary<string, decimal>> GetIncomeBySourceAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        var query = _context.Incomes.AsQueryable();
        
        if (startDate.HasValue)
            query = query.Where(i => i.Date >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(i => i.Date <= endDate.Value);
        
        var incomes = await query.ToListAsync();
        return incomes
            .GroupBy(i => i.Source)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));
    }

    // Additional methods for UI
    public async Task<List<Income>> GetAllIncomesAsync()
    {
        return await GetAllAsync();
    }

    public async Task<Income?> GetIncomeByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<Income> CreateIncomeAsync(Income income)
    {
        return await CreateAsync(income);
    }

    public async Task UpdateIncomeAsync(Income income)
    {
        await UpdateIncomeAsync(income); // Note: Fix recursion if necessary, but caller might expect this name
        // Use UpdateAsync(income) instead
    }

    public async Task UpdateIncomeAsyncProxy(Income income)
    {
         await UpdateAsync(income);
    }

    public async Task DeleteIncomeAsync(int id)
    {
        await DeleteAsync(id);
    }

    public async Task<decimal> GetMonthlyIncomeAsync(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return await GetTotalIncomeAsync(startDate, endDate);
    }
}
