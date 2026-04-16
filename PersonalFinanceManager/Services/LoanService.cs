using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class LoanService
{
    private readonly AppDbContext _context;

    public LoanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LoanTransaction>> GetAllAsync()
    {
        return await _context.LoanTransactions
            .OrderByDescending(l => l.Date)
            .ToListAsync();
    }

    public async Task<List<LoanTransaction>> GetByTypeAsync(TransactionType type)
    {
        return await _context.LoanTransactions
            .Where(l => l.Type == type)
            .OrderByDescending(l => l.Date)
            .ToListAsync();
    }

    public async Task<LoanTransaction?> GetByIdAsync(int id)
    {
        return await _context.LoanTransactions.FindAsync(id);
    }

    public async Task<LoanTransaction> CreateAsync(LoanTransaction loan)
    {
        _context.LoanTransactions.Add(loan);
        await _context.SaveChangesAsync();
        
        // Update account balance if account is specified
        if (loan.AccountId.HasValue)
        {
            var account = await _context.Accounts.FindAsync(loan.AccountId.Value);
            if (account != null)
            {
                if (loan.Type == TransactionType.Lending)
                    account.Balance -= loan.Amount; // Money given out
                else if (loan.Type == TransactionType.Borrowing)
                    account.Balance += loan.Amount; // Money received
                
                account.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
        
        return loan;
    }

    public async Task UpdateAsync(LoanTransaction loan)
    {
        _context.LoanTransactions.Update(loan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var loan = await _context.LoanTransactions.FindAsync(id);
        if (loan != null)
        {
            _context.LoanTransactions.Remove(loan);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<LoanPayment> AddPaymentAsync(LoanPayment payment)
    {
        _context.LoanPayments.Add(payment);
        
        // Update loan transaction
        var loan = await _context.LoanTransactions.FindAsync(payment.LoanTransactionId);
        if (loan != null)
        {
            loan.AmountPaid += payment.Amount;
            
            if (loan.AmountPaid >= loan.Amount)
                loan.Status = LoanStatus.FullyPaid;
            else if (loan.AmountPaid > 0)
                loan.Status = LoanStatus.PartiallyPaid;
            
            // Update account balance
            if (payment.AccountId.HasValue)
            {
                var account = await _context.Accounts.FindAsync(payment.AccountId.Value);
                if (account != null)
                {
                    if (loan.Type == TransactionType.Lending)
                        account.Balance += payment.Amount; // Money received back
                    else if (loan.Type == TransactionType.Borrowing)
                        account.Balance -= payment.Amount; // Money paid back
                    
                    account.LastUpdated = DateTime.Now;
                }
            }
        }
        
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<List<LoanPayment>> GetPaymentsAsync(int loanId)
    {
        return await _context.LoanPayments
            .Where(p => p.LoanTransactionId == loanId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalLendingAsync()
    {
        var loans = await _context.LoanTransactions
            .Where(l => l.Type == TransactionType.Lending)
            .ToListAsync();
        return loans.Sum(l => l.RemainingAmount);
    }

    public async Task<decimal> GetTotalBorrowingAsync()
    {
        var loans = await _context.LoanTransactions
            .Where(l => l.Type == TransactionType.Borrowing)
            .ToListAsync();
        return loans.Sum(l => l.RemainingAmount);
    }

    // Additional methods for UI
    public async Task<List<LoanTransaction>> GetLendingLoansAsync()
    {
        return await GetByTypeAsync(TransactionType.Lending);
    }

    public async Task<List<LoanTransaction>> GetBorrowingLoansAsync()
    {
        return await GetByTypeAsync(TransactionType.Borrowing);
    }

    public async Task<LoanTransaction?> GetLoanByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<LoanTransaction> CreateLoanAsync(LoanTransaction loan)
    {
        return await CreateAsync(loan);
    }

    public async Task DeleteLoanAsync(int id)
    {
        await DeleteAsync(id);
    }

    public async Task<decimal> GetTotalLentAsync()
    {
        return await GetTotalLendingAsync();
    }

    public async Task<decimal> GetTotalBorrowedAsync()
    {
        return await GetTotalBorrowingAsync();
    }
}
