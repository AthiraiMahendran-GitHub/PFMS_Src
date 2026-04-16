using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class ExpenseService : BaseService
{
    public ExpenseService(AppDbContext context, TenantService tenantService) : base(context, tenantService)
    {
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        await EnsureTenantAsync();
        return await _context.Expenses
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await EnsureTenantAsync();
        return await _context.Expenses
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        await EnsureTenantAsync();
        return await _context.Expenses.FindAsync(id);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        await EnsureTenantAsync();
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        
        // Update account balance if account is specified
        if (expense.AccountId.HasValue)
        {
            var account = await _context.Accounts.FindAsync(expense.AccountId.Value);
            if (account != null)
            {
                account.Balance -= expense.Amount;
                account.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
        
        return expense;
    }

    public async Task UpdateAsync(Expense expense)
    {
        await EnsureTenantAsync();
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await EnsureTenantAsync();
        var expense = await _context.Expenses.FindAsync(id);
        if (expense != null)
        {
            // Update account balance if account is specified
            if (expense.AccountId.HasValue)
            {
                var account = await _context.Accounts.FindAsync(expense.AccountId.Value);
                if (account != null)
                {
                    account.Balance += expense.Amount; // Refund the amount
                    account.LastUpdated = DateTime.Now;
                }
            }
            
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalExpensesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        var query = _context.Expenses.AsQueryable();
        
        if (startDate.HasValue)
            query = query.Where(e => e.Date >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(e => e.Date <= endDate.Value);
        
        var expenses = await query.ToListAsync();
        return expenses.Sum(e => e.Amount);
    }

    public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        var query = _context.Expenses.AsQueryable();
        
        if (startDate.HasValue)
            query = query.Where(e => e.Date >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(e => e.Date <= endDate.Value);
        
        var expenses = await query.ToListAsync();
        return expenses
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
    }
}
