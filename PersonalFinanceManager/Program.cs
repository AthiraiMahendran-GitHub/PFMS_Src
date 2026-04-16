using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Services;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<TenantService>();

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<IncomeService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<InvestmentService>();
builder.Services.AddScoped<DematAccountService>();
builder.Services.AddScoped<InvestmentTransactionService>();
builder.Services.AddScoped<LoanService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ExportService>();

// Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Initialize database
try 
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        
        // Initialize default categories
        var categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();
        await categoryService.InitializeDefaultCategoriesAsync();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Startup Error: {ex.Message}");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
