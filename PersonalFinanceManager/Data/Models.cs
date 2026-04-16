using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Data;

public interface ITenantEntity
{
    public int UserId { get; set; }
}

// User Model for Authentication
public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string PasswordHash { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? LastLoginAt { get; set; }
}

// Enums defined first to avoid conflicts
public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum AccountType
{
    Cash,
    Bank,
    CreditCard,
    Wallet
}

public enum TransactionType
{
    Income,
    Expense,
    Transfer,
    Lending,
    Borrowing,
    LoanRepayment,
    LoanReceived
}

public enum LoanStatus
{
    Active,
    PartiallyPaid,
    FullyPaid,
    Overdue
}

public class Expense : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? SubCategory { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [StringLength(100)]
    public string PaymentSource { get; set; } = "Cash"; // Cash, HDFC Bank, ICICI Bank, etc.
    
    public int? AccountId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// New Models for Income Management
public class Income : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Source { get; set; } = string.Empty; // Salary, Freelance, Business, etc.
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [StringLength(100)]
    public string ReceivedIn { get; set; } = "Cash"; // Cash, HDFC Bank, etc.
    
    public int? AccountId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class Account : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // Cash, HDFC Bank, ICICI Bank, etc.
    
    [Required]
    public AccountType Type { get; set; }
    
    [Required]
    public decimal Balance { get; set; }
    
    [StringLength(50)]
    public string? AccountNumber { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? LastUpdated { get; set; }
}

public class ExpenseCategory : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public string? Icon { get; set; }
    
    public string? Color { get; set; }
    
    public int? ParentCategoryId { get; set; }
    
    public List<ExpenseCategory> SubCategories { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class LoanTransaction : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string PersonName { get; set; } = string.Empty;
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public TransactionType Type { get; set; } // Lending or Borrowing
    
    [Required]
    public LoanStatus Status { get; set; } = LoanStatus.Active;
    
    public decimal AmountPaid { get; set; } = 0;
    
    public decimal RemainingAmount => Amount - AmountPaid;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [StringLength(100)]
    public string? PaymentSource { get; set; }
    
    public int? AccountId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public List<LoanPayment> Payments { get; set; } = new();
}

public class LoanPayment : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public int LoanTransactionId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime PaymentDate { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    [StringLength(100)]
    public string? PaymentSource { get; set; }
    
    public int? AccountId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// Investment Enums
public enum InvestmentType
{
    Stock,
    MutualFund,
    Bond,
    Gold,
    RealEstate,
    Crypto,
    FD,
    Other
}

public enum TransactionAction
{
    Buy,
    Sell,
    Dividend,
    Bonus,
    Split
}

// Demat Account
public class DematAccount : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // e.g., "Zerodha", "Upstox"
    
    [Required]
    [StringLength(50)]
    public string AccountNumber { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? BrokerName { get; set; }
    
    public int? LinkedBankAccountId { get; set; } // Link to Account table
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// Investment Holdings (Current Portfolio)
public class InvestmentHolding : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public int DematAccountId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Symbol { get; set; } = string.Empty; // Stock symbol or fund name
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public InvestmentType Type { get; set; }
    
    [Required]
    public decimal Quantity { get; set; }
    
    [Required]
    public decimal AveragePrice { get; set; } // Average buy price
    
    public decimal CurrentPrice { get; set; } // Latest market price
    
    public decimal TotalInvested => Quantity * AveragePrice;
    
    public decimal CurrentValue => Quantity * CurrentPrice;
    
    public decimal UnrealizedProfitLoss => CurrentValue - TotalInvested;
    
    public decimal UnrealizedProfitLossPercentage => TotalInvested > 0 
        ? (UnrealizedProfitLoss / TotalInvested) * 100 
        : 0;
    
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}

// Investment Transactions (Buy/Sell History)
public class InvestmentTransaction : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public int DematAccountId { get; set; }
    
    public int? HoldingId { get; set; } // Link to holding
    
    [Required]
    [StringLength(200)]
    public string Symbol { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public InvestmentType Type { get; set; }
    
    [Required]
    public TransactionAction Action { get; set; }
    
    [Required]
    public decimal Quantity { get; set; }
    
    [Required]
    public decimal Price { get; set; } // Price per unit
    
    public decimal TotalAmount => Quantity * Price;
    
    public decimal BrokerageCharges { get; set; }
    
    public decimal TaxCharges { get; set; }
    
    public decimal NetAmount => Action == TransactionAction.Buy 
        ? TotalAmount + BrokerageCharges + TaxCharges 
        : TotalAmount - BrokerageCharges - TaxCharges;
    
    [Required]
    public DateTime TransactionDate { get; set; }
    
    public int? FromAccountId { get; set; } // Bank account used for transaction
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    // For Sell transactions - track profit/loss
    public decimal? RealizedProfitLoss { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// Keep old Investment model for backward compatibility (can be deprecated later)
public class Investment : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public decimal InitialAmount { get; set; }
    
    public decimal? CurrentValue { get; set; }
    
    [Required]
    public DateTime PurchaseDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? LastUpdated { get; set; }
    
    public decimal ProfitLoss => (CurrentValue ?? InitialAmount) - InitialAmount;
    
    public decimal ROIPercentage => InitialAmount > 0 
        ? ((CurrentValue ?? InitialAmount) - InitialAmount) / InitialAmount * 100 
        : 0;
}

public class DailyTask : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Company { get; set; } = string.Empty;
    
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public Data.TaskStatus Status { get; set; } = Data.TaskStatus.Pending;
    
    [Required]
    public Data.TaskPriority Priority { get; set; } = Data.TaskPriority.Medium;
    
    public DateTime? DueDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? CompletedAt { get; set; }
    
    // Subtask support
    public int? ParentTaskId { get; set; }
    
    public List<DailyTask> SubTasks { get; set; } = new();
}

public class SubTask : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public int ParentTaskId { get; set; }
    
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;
    
    public bool IsCompleted { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? CompletedAt { get; set; }
}

public class Note : ITenantEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public bool IsImportant { get; set; } = false;
    
    public bool IsPinned { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? UpdatedAt { get; set; }
}
