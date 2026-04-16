using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class InvestmentService
{
    private readonly AppDbContext _context;

    public InvestmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Investment>> GetAllAsync()
    {
        return await _context.Investments
            .OrderByDescending(i => i.PurchaseDate)
            .ToListAsync();
    }

    public async Task<Investment?> GetByIdAsync(int id)
    {
        return await _context.Investments.FindAsync(id);
    }

    public async Task<Investment> CreateAsync(Investment investment)
    {
        _context.Investments.Add(investment);
        await _context.SaveChangesAsync();
        return investment;
    }

    public async Task UpdateAsync(Investment investment)
    {
        investment.LastUpdated = DateTime.Now;
        _context.Investments.Update(investment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var investment = await _context.Investments.FindAsync(id);
        if (investment != null)
        {
            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalInvestedAsync()
    {
        var investments = await _context.Investments.ToListAsync();
        return investments.Sum(i => i.InitialAmount);
    }

    public async Task<decimal> GetTotalCurrentValueAsync()
    {
        var investments = await _context.Investments.ToListAsync();
        return investments.Sum(i => i.CurrentValue ?? i.InitialAmount);
    }

    public async Task<decimal> GetTotalProfitLossAsync()
    {
        var investments = await _context.Investments.ToListAsync();
        return investments.Sum(i => i.ProfitLoss);
    }

    public async Task<Dictionary<string, decimal>> GetInvestmentsByTypeAsync()
    {
        var investments = await _context.Investments.ToListAsync();
        return investments
            .GroupBy(i => i.Type)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.CurrentValue ?? i.InitialAmount));
    }
}
