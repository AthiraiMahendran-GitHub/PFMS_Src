using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class ReportService : BaseService
{
    public ReportService(AppDbContext context, TenantService tenantService) : base(context, tenantService)
    {
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync()
    {
        await EnsureTenantAsync();
        var today = DateTime.Today;
        var thisMonth = new DateTime(today.Year, today.Month, 1);
        var thisYear = new DateTime(today.Year, 1, 1);

        // Get all data first, then aggregate in memory
        var allExpenses = await _context.Expenses.ToListAsync();
        var allInvestments = await _context.Investments.ToListAsync();
        var allTasks = await _context.Tasks.ToListAsync();
        var allNotes = await _context.Notes.ToListAsync();

        var summary = new DashboardSummary
        {
            // Expenses
            TotalExpensesToday = allExpenses
                .Where(e => e.Date.Date == today)
                .Sum(e => e.Amount),
            
            TotalExpensesThisMonth = allExpenses
                .Where(e => e.Date >= thisMonth)
                .Sum(e => e.Amount),
            
            TotalExpensesThisYear = allExpenses
                .Where(e => e.Date >= thisYear)
                .Sum(e => e.Amount),

            // Investments
            TotalInvested = allInvestments.Sum(i => i.InitialAmount),
            TotalCurrentValue = allInvestments.Sum(i => i.CurrentValue ?? i.InitialAmount),

            // Tasks
            PendingTasks = allTasks.Count(t => t.Status == Data.TaskStatus.Pending),
            CompletedTasksToday = allTasks
                .Count(t => t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today),
            OverdueTasks = allTasks
                .Count(t => t.Status != Data.TaskStatus.Completed && t.DueDate.HasValue && t.DueDate.Value.Date < today),

            // Notes
            TotalNotes = allNotes.Count,
            NotesToday = allNotes.Count(n => n.CreatedAt.Date == today)
        };

        summary.TotalProfitLoss = summary.TotalCurrentValue - summary.TotalInvested;
        summary.ROIPercentage = summary.TotalInvested > 0 
            ? (summary.TotalProfitLoss / summary.TotalInvested * 100) 
            : 0;

        return summary;
    }

    public async Task<List<MonthlyExpenseTrend>> GetMonthlyExpenseTrendsAsync(int months = 12)
    {
        await EnsureTenantAsync();
        var startDate = DateTime.Today.AddMonths(-months);
        
        var expenses = await _context.Expenses
            .Where(e => e.Date >= startDate)
            .ToListAsync();

        return expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new MonthlyExpenseTrend
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalAmount = g.Sum(e => e.Amount),
                TransactionCount = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();
    }

    public async Task<List<CategoryExpenseReport>> GetCategoryExpenseReportAsync(DateTime? startDate = null, DateTime? endDate = null)
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
            .Select(g => new CategoryExpenseReport
            {
                Category = g.Key,
                TotalAmount = g.Sum(e => e.Amount),
                TransactionCount = g.Count(),
                AverageAmount = g.Average(e => e.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }

    public async Task<List<InvestmentPerformanceReport>> GetInvestmentPerformanceReportAsync()
    {
        await EnsureTenantAsync();
        var investments = await _context.Investments.ToListAsync();
        
        return investments.Select(i => new InvestmentPerformanceReport
        {
            Name = i.Name,
            Type = i.Type,
            InitialAmount = i.InitialAmount,
            CurrentValue = i.CurrentValue ?? i.InitialAmount,
            ProfitLoss = i.ProfitLoss,
            ROIPercentage = i.ROIPercentage,
            DaysHeld = (DateTime.Now - i.PurchaseDate).Days
        })
        .OrderByDescending(x => x.ROIPercentage)
        .ToList();
    }

    public async Task<TaskCompletionReport> GetTaskCompletionReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        var query = _context.Tasks.AsQueryable();
        
        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        var tasks = await query.ToListAsync();

        return new TaskCompletionReport
        {
            TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.Status == Data.TaskStatus.Completed),
            PendingTasks = tasks.Count(t => t.Status == Data.TaskStatus.Pending),
            InProgressTasks = tasks.Count(t => t.Status == Data.TaskStatus.InProgress),
            CancelledTasks = tasks.Count(t => t.Status == Data.TaskStatus.Cancelled),
            CompletionRate = tasks.Count > 0 
                ? (decimal)tasks.Count(t => t.Status == Data.TaskStatus.Completed) / tasks.Count * 100 
                : 0,
            TasksByCompany = tasks.GroupBy(t => t.Company)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public async Task<List<IncomeSourceReport>> GetIncomeSourceReportAsync(DateTime? startDate = null, DateTime? endDate = null)
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
            .Select(g => new IncomeSourceReport
            {
                Source = g.Key,
                TotalAmount = g.Sum(i => i.Amount),
                TransactionCount = g.Count(),
                AverageAmount = g.Average(i => i.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }

    public async Task<List<MonthlyIncomeTrend>> GetMonthlyIncomeTrendsAsync(int months = 12)
    {
        await EnsureTenantAsync();
        var startDate = DateTime.Today.AddMonths(-months);
        
        var incomes = await _context.Incomes
            .Where(i => i.Date >= startDate)
            .ToListAsync();

        return incomes
            .GroupBy(i => new { i.Date.Year, i.Date.Month })
            .Select(g => new MonthlyIncomeTrend
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalAmount = g.Sum(i => i.Amount),
                TransactionCount = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();
    }

    public async Task<IncomeVsExpenseReport> GetIncomeVsExpenseReportAsync(int months = 12)
    {
        await EnsureTenantAsync();
        var startDate = DateTime.Today.AddMonths(-months);

        var incomes = await _context.Incomes
            .Where(i => i.Date >= startDate)
            .ToListAsync();

        var expenses = await _context.Expenses
            .Where(e => e.Date >= startDate)
            .ToListAsync();

        var monthlyData = new List<MonthlyIncomeExpenseComparison>();

        for (int i = 0; i < months; i++)
        {
            var monthStart = DateTime.Today.AddMonths(-i);
            var year = monthStart.Year;
            var month = monthStart.Month;

            var monthIncome = incomes
                .Where(inc => inc.Date.Year == year && inc.Date.Month == month)
                .Sum(inc => inc.Amount);

            var monthExpense = expenses
                .Where(exp => exp.Date.Year == year && exp.Date.Month == month)
                .Sum(exp => exp.Amount);

            monthlyData.Add(new MonthlyIncomeExpenseComparison
            {
                Year = year,
                Month = month,
                Income = monthIncome,
                Expense = monthExpense,
                NetSavings = monthIncome - monthExpense
            });
        }

        return new IncomeVsExpenseReport
        {
            TotalIncome = incomes.Sum(i => i.Amount),
            TotalExpense = expenses.Sum(e => e.Amount),
            NetSavings = incomes.Sum(i => i.Amount) - expenses.Sum(e => e.Amount),
            MonthlyComparison = monthlyData.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList()
        };
    }
    public async Task<FinancialInsightsReport> GetFinancialInsightsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        
        var incomeQuery = _context.Incomes.AsQueryable();
        var expenseQuery = _context.Expenses.AsQueryable();
        var loanQuery = _context.LoanTransactions.AsQueryable();
        
        if (startDate.HasValue)
        {
            incomeQuery = incomeQuery.Where(i => i.Date >= startDate.Value);
            expenseQuery = expenseQuery.Where(e => e.Date >= startDate.Value);
            loanQuery = loanQuery.Where(l => l.Date >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            incomeQuery = incomeQuery.Where(i => i.Date <= endDate.Value);
            expenseQuery = expenseQuery.Where(e => e.Date <= endDate.Value);
            loanQuery = loanQuery.Where(l => l.Date <= endDate.Value);
        }
        
        var incomes = await incomeQuery.ToListAsync();
        var expenses = await expenseQuery.ToListAsync();
        var loans = await loanQuery.ToListAsync();
        
        var totalIncome = incomes.Sum(i => i.Amount);
        var totalExpense = expenses.Sum(e => e.Amount);
        var totalLent = loans.Where(l => l.Type == TransactionType.Lending).Sum(l => l.Amount);
        var totalBorrowed = loans.Where(l => l.Type == TransactionType.Borrowing).Sum(l => l.Amount);
        
        return new FinancialInsightsReport
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TotalLent = totalLent,
            TotalBorrowed = totalBorrowed,
            IncomeBySource = incomes.GroupBy(i => i.Source)
                .Select(g => new ChartDataPoint { Category = g.Key, Value = g.Sum(i => i.Amount) })
                .OrderByDescending(x => x.Value)
                .ToList(),
            ExpenseByCategory = expenses.GroupBy(e => e.Category)
                .Select(g => new ChartDataPoint { Category = g.Key, Value = g.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Value)
                .ToList()
        };
    }

    public async Task<List<TransactionViewModel>> GetFullTransactionReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        var allTransactions = new List<TransactionViewModel>();
        
        var incomeQuery = _context.Incomes.AsQueryable();
        if (startDate.HasValue) incomeQuery = incomeQuery.Where(i => i.Date >= startDate.Value);
        if (endDate.HasValue) incomeQuery = incomeQuery.Where(i => i.Date <= endDate.Value);
        var incomes = await incomeQuery.ToListAsync();
        allTransactions.AddRange(incomes.Select(i => new TransactionViewModel {
            Date = i.Date, Type = "Income", Category = i.Source, Description = i.Description ?? "-", Amount = i.Amount
        }));

        var expenseQuery = _context.Expenses.AsQueryable();
        if (startDate.HasValue) expenseQuery = expenseQuery.Where(e => e.Date >= startDate.Value);
        if (endDate.HasValue) expenseQuery = expenseQuery.Where(e => e.Date <= endDate.Value);
        var expenses = await expenseQuery.ToListAsync();
        allTransactions.AddRange(expenses.Select(e => new TransactionViewModel {
            Date = e.Date, Type = "Expense", Category = e.Category, Description = e.Description ?? "-", Amount = e.Amount
        }));

        var loanQuery = _context.LoanTransactions.AsQueryable();
        if (startDate.HasValue) loanQuery = loanQuery.Where(l => l.Date >= startDate.Value);
        if (endDate.HasValue) loanQuery = loanQuery.Where(l => l.Date <= endDate.Value);
        var loans = await loanQuery.ToListAsync();
        allTransactions.AddRange(loans.Select(l => new TransactionViewModel {
            Date = l.Date, Type = l.Type.ToString(), Category = "Loan", Description = l.PersonName, Amount = l.Amount
        }));

        return allTransactions.OrderByDescending(t => t.Date).ToList();
    }

    public async Task<LoanSummaryReport> GetLoanSummaryReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await EnsureTenantAsync();
        var query = _context.LoanTransactions.AsQueryable();
        if (startDate.HasValue) query = query.Where(l => l.Date >= startDate.Value);
        if (endDate.HasValue) query = query.Where(l => l.Date <= endDate.Value);
        
        var loans = await query.ToListAsync();
        
        return new LoanSummaryReport {
            TotalLent = loans.Where(l => l.Type == TransactionType.Lending).Sum(l => l.Amount),
            TotalBorrowed = loans.Where(l => l.Type == TransactionType.Borrowing).Sum(l => l.Amount),
            Details = loans.Select(l => new LoanReportDetail {
                PersonName = l.PersonName,
                Type = l.Type.ToString(),
                InitialAmount = l.Amount,
                Balance = l.RemainingAmount,
                Status = l.Status.ToString()
            }).ToList()
        };
    }
}

public class DashboardSummary
{
    public decimal TotalExpensesToday { get; set; }
    public decimal TotalExpensesThisMonth { get; set; }
    public decimal TotalExpensesThisYear { get; set; }
    public decimal TotalInvested { get; set; }
    public decimal TotalCurrentValue { get; set; }
    public decimal TotalProfitLoss { get; set; }
    public decimal ROIPercentage { get; set; }
    public int PendingTasks { get; set; }
    public int CompletedTasksToday { get; set; }
    public int OverdueTasks { get; set; }
    public int TotalNotes { get; set; }
    public int NotesToday { get; set; }
}

public class MonthlyExpenseTrend
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
}

public class CategoryExpenseReport
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageAmount { get; set; }
}

public class InvestmentPerformanceReport
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal InitialAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ROIPercentage { get; set; }
    public int DaysHeld { get; set; }
}

public class TaskCompletionReport
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CancelledTasks { get; set; }
    public decimal CompletionRate { get; set; }
    public Dictionary<string, int> TasksByCompany { get; set; } = new();
}

public class IncomeSourceReport
{
    public string Source { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageAmount { get; set; }
}

public class MonthlyIncomeTrend
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
}

public class IncomeVsExpenseReport
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetSavings { get; set; }
    public List<MonthlyIncomeExpenseComparison> MonthlyComparison { get; set; } = new();
}

public class MonthlyIncomeExpenseComparison
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal NetSavings { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
}
public class FinancialInsightsReport
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TotalLent { get; set; }
    public decimal TotalBorrowed { get; set; }
    public List<ChartDataPoint> IncomeBySource { get; set; } = new();
    public List<ChartDataPoint> ExpenseByCategory { get; set; } = new();
}

public class ChartDataPoint
{
    public string Category { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
public class TransactionViewModel
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class LoanSummaryReport
{
    public decimal TotalLent { get; set; }
    public decimal TotalBorrowed { get; set; }
    public List<LoanReportDetail> Details { get; set; } = new();
}

public class LoanReportDetail
{
    public string PersonName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal InitialAmount { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
}
