using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class InvestmentTransactionService
{
    private readonly AppDbContext _context;
    private readonly AccountService _accountService;

    public InvestmentTransactionService(AppDbContext context, AccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<List<InvestmentTransaction>> GetAllAsync(int? dematAccountId = null)
    {
        var query = _context.InvestmentTransactions.AsQueryable();
        
        if (dematAccountId.HasValue)
        {
            query = query.Where(t => t.DematAccountId == dematAccountId.Value);
        }
        
        return await query
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<InvestmentTransaction?> GetByIdAsync(int id)
    {
        return await _context.InvestmentTransactions.FindAsync(id);
    }

    public async Task<InvestmentTransaction> CreateBuyTransactionAsync(InvestmentTransaction transaction)
    {
        transaction.Action = TransactionAction.Buy;
        
        // Deduct money from bank account
        if (transaction.FromAccountId.HasValue)
        {
            var account = await _context.Accounts.FindAsync(transaction.FromAccountId.Value);
            if (account != null)
            {
                account.Balance -= transaction.NetAmount;
                account.LastUpdated = DateTime.Now;
            }
        }
        
        // Update or create holding
        await UpdateHoldingForBuy(transaction);
        
        _context.InvestmentTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        
        return transaction;
    }

    // Alias for Portfolio page
    public async Task<InvestmentTransaction> BuyStockAsync(InvestmentTransaction transaction)
    {
        return await CreateBuyTransactionAsync(transaction);
    }

    public async Task<InvestmentTransaction> CreateSellTransactionAsync(InvestmentTransaction transaction)
    {
        transaction.Action = TransactionAction.Sell;
        
        // Calculate realized profit/loss
        var holding = await _context.InvestmentHoldings
            .FirstOrDefaultAsync(h => h.DematAccountId == transaction.DematAccountId 
                                   && h.Symbol == transaction.Symbol);
        
        if (holding != null)
        {
            // Realized P/L = (Sell Price - Average Buy Price) * Quantity - Charges
            transaction.RealizedProfitLoss = 
                ((transaction.Price - holding.AveragePrice) * transaction.Quantity) 
                - transaction.BrokerageCharges - transaction.TaxCharges;
            
            // Update holding
            await UpdateHoldingForSell(transaction, holding);
        }
        
        // Add money to bank account
        if (transaction.FromAccountId.HasValue)
        {
            var account = await _context.Accounts.FindAsync(transaction.FromAccountId.Value);
            if (account != null)
            {
                account.Balance += transaction.NetAmount;
                account.LastUpdated = DateTime.Now;
            }
        }
        
        _context.InvestmentTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        
        return transaction;
    }

    // Alias for Portfolio page
    public async Task<InvestmentTransaction> SellStockAsync(InvestmentTransaction transaction)
    {
        return await CreateSellTransactionAsync(transaction);
    }

    public async Task<List<InvestmentTransaction>> GetTransactionsByAccountAsync(int dematAccountId)
    {
        return await GetAllAsync(dematAccountId);
    }

    private async Task UpdateHoldingForBuy(InvestmentTransaction transaction)
    {
        var holding = await _context.InvestmentHoldings
            .FirstOrDefaultAsync(h => h.DematAccountId == transaction.DematAccountId 
                                   && h.Symbol == transaction.Symbol);
        
        if (holding == null)
        {
            // Create new holding
            holding = new InvestmentHolding
            {
                DematAccountId = transaction.DematAccountId,
                Symbol = transaction.Symbol,
                Name = transaction.Name,
                Type = transaction.Type,
                Quantity = transaction.Quantity,
                AveragePrice = transaction.Price,
                CurrentPrice = transaction.Price
            };
            _context.InvestmentHoldings.Add(holding);
        }
        else
        {
            // Update existing holding - calculate new average price
            var totalCost = (holding.Quantity * holding.AveragePrice) + (transaction.Quantity * transaction.Price);
            holding.Quantity += transaction.Quantity;
            holding.AveragePrice = totalCost / holding.Quantity;
            holding.LastUpdated = DateTime.Now;
        }
        
        transaction.HoldingId = holding.Id;
    }

    private async Task UpdateHoldingForSell(InvestmentTransaction transaction, InvestmentHolding holding)
    {
        holding.Quantity -= transaction.Quantity;
        holding.LastUpdated = DateTime.Now;
        
        // If all sold, remove holding
        if (holding.Quantity <= 0)
        {
            _context.InvestmentHoldings.Remove(holding);
        }
        
        transaction.HoldingId = holding.Id;
    }

    public async Task<decimal> GetTotalRealizedProfitLossAsync(int dematAccountId)
    {
        var transactions = await _context.InvestmentTransactions
            .Where(t => t.DematAccountId == dematAccountId 
                     && t.Action == TransactionAction.Sell 
                     && t.RealizedProfitLoss.HasValue)
            .ToListAsync();
        
        return transactions.Sum(t => t.RealizedProfitLoss ?? 0);
    }

    public async Task<List<InvestmentTransaction>> GetTransactionsBySymbolAsync(int dematAccountId, string symbol)
    {
        return await _context.InvestmentTransactions
            .Where(t => t.DematAccountId == dematAccountId && t.Symbol == symbol)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task UpdateCurrentPriceAsync(int holdingId, decimal currentPrice)
    {
        var holding = await _context.InvestmentHoldings.FindAsync(holdingId);
        if (holding != null)
        {
            holding.CurrentPrice = currentPrice;
            holding.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var transaction = await _context.InvestmentTransactions.FindAsync(id);
        if (transaction != null)
        {
            // Note: This is a simple delete. In production, you'd want to reverse the holding updates
            _context.InvestmentTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
