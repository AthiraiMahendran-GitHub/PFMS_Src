using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class AccountService : BaseService
{
    public AccountService(AppDbContext context, TenantService tenantService) : base(context, tenantService)
    {
    }

    public async Task<List<Account>> GetAllAsync()
    {
        await EnsureTenantAsync();
        return await _context.Accounts
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        await EnsureTenantAsync();
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> CreateAsync(Account account)
    {
        await EnsureTenantAsync();
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task UpdateAsync(Account account)
    {
        await EnsureTenantAsync();
        account.LastUpdated = DateTime.Now;
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await EnsureTenantAsync();
        var account = await _context.Accounts.FindAsync(id);
        if (account != null)
        {
            account.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalBalanceAsync()
    {
        await EnsureTenantAsync();
        var accounts = await _context.Accounts
            .Where(a => a.IsActive)
            .ToListAsync();
        return accounts.Sum(a => a.Balance);
    }

    public async Task<Dictionary<string, decimal>> GetBalanceByTypeAsync()
    {
        await EnsureTenantAsync();
        var accounts = await _context.Accounts
            .Where(a => a.IsActive)
            .ToListAsync();
        
        return accounts
            .GroupBy(a => a.Type.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(a => a.Balance));
    }

    public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount, string description)
    {
        await EnsureTenantAsync();
        var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
        var toAccount = await _context.Accounts.FindAsync(toAccountId);

        if (fromAccount == null || toAccount == null)
            throw new Exception("Account not found");

        if (fromAccount.Balance < amount)
            throw new Exception("Insufficient balance");

        fromAccount.Balance -= amount;
        fromAccount.LastUpdated = DateTime.Now;

        toAccount.Balance += amount;
        toAccount.LastUpdated = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    // Additional methods for UI
    public async Task<List<Account>> GetAllAccountsAsync()
    {
        return await GetAllAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<Account> CreateAccountAsync(Account account)
    {
        return await CreateAsync(account);
    }

    public async Task UpdateAccountAsync(Account account)
    {
        await UpdateAsync(account);
    }

    public async Task DeleteAccountAsync(int id)
    {
        await DeleteAsync(id);
    }
}
