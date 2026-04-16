using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceManager.Data;

public class AppDbContext : DbContext
{
    public int CurrentUserId { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<LoanTransaction> LoanTransactions { get; set; }
    public DbSet<LoanPayment> LoanPayments { get; set; }
    public DbSet<Investment> Investments { get; set; }
    public DbSet<DematAccount> DematAccounts { get; set; }
    public DbSet<InvestmentHolding> InvestmentHoldings { get; set; }
    public DbSet<InvestmentTransaction> InvestmentTransactions { get; set; }
    public DbSet<DailyTask> Tasks { get; set; }
    public DbSet<SubTask> SubTasks { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<User> Users { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.UserId = CurrentUserId;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Query Filters for Multi-Tenancy
        modelBuilder.Entity<Expense>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<Income>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<Account>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<ExpenseCategory>().HasQueryFilter(e => e.UserId == CurrentUserId || e.UserId == 0);
        modelBuilder.Entity<LoanTransaction>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<LoanPayment>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<DematAccount>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<InvestmentHolding>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<InvestmentTransaction>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<Investment>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<DailyTask>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<SubTask>().HasQueryFilter(e => e.UserId == CurrentUserId);
        modelBuilder.Entity<Note>().HasQueryFilter(e => e.UserId == CurrentUserId);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });
        
        // ... (rest of indexes)
        // Note: I will only keep the indexes that don't conflict with filters. 
        // Actually, I'll keep all existing indexes as they are still relevant.

        // Expense configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.AccountId);
            entity.HasIndex(e => e.UserId); // Important for performance
        });

        // Income configuration
        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Source);
            entity.HasIndex(e => e.AccountId);
            entity.HasIndex(e => e.UserId);
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.UserId);
        });

        // ExpenseCategory configuration
        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ParentCategoryId);
            entity.HasIndex(e => e.UserId);
        });

        // LoanTransaction configuration
        modelBuilder.Entity<LoanTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.UserId);
        });

        // LoanPayment configuration
        modelBuilder.Entity<LoanPayment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.LoanTransactionId);
            entity.HasIndex(e => e.PaymentDate);
            entity.HasIndex(e => e.UserId);
        });

        // Investment configuration
        modelBuilder.Entity<Investment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InitialAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentValue).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.PurchaseDate);
            entity.HasIndex(e => e.UserId);
        });

        // DematAccount configuration
        modelBuilder.Entity<DematAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LinkedBankAccountId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.UserId);
        });

        // InvestmentHolding configuration
        modelBuilder.Entity<InvestmentHolding>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,4)");
            entity.Property(e => e.AveragePrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentPrice).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.DematAccountId);
            entity.HasIndex(e => e.Symbol);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.UserId);
        });

        // InvestmentTransaction configuration
        modelBuilder.Entity<InvestmentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,4)");
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.BrokerageCharges).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxCharges).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RealizedProfitLoss).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.DematAccountId);
            entity.HasIndex(e => e.HoldingId);
            entity.HasIndex(e => e.Symbol);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.UserId);
        });

        // Task configuration
        modelBuilder.Entity<DailyTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Company);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DueDate);
            entity.HasIndex(e => e.ParentTaskId);
            entity.HasIndex(e => e.UserId);
        });

        // SubTask configuration
        modelBuilder.Entity<SubTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ParentTaskId);
            entity.HasIndex(e => e.UserId);
        });

        // Note configuration
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsImportant);
            entity.HasIndex(e => e.IsPinned);
            entity.HasIndex(e => e.UserId);
        });
    }
}
