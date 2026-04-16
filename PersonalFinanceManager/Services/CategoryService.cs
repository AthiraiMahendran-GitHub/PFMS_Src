using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class CategoryService : BaseService
{
    public CategoryService(AppDbContext context, TenantService tenantService) : base(context, tenantService)
    {
    }

    public async Task<List<ExpenseCategory>> GetAllAsync()
    {
        await EnsureTenantAsync();
        return await _context.ExpenseCategories
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<ExpenseCategory>> GetSubCategoriesAsync(int parentId)
    {
        await EnsureTenantAsync();
        return await _context.ExpenseCategories
            .Where(c => c.ParentCategoryId == parentId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<ExpenseCategory?> GetByIdAsync(int id)
    {
        await EnsureTenantAsync();
        return await _context.ExpenseCategories.FindAsync(id);
    }

    public async Task<ExpenseCategory> CreateAsync(ExpenseCategory category)
    {
        await EnsureTenantAsync();
        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateAsync(ExpenseCategory category)
    {
        await EnsureTenantAsync();
        _context.ExpenseCategories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await EnsureTenantAsync();
        var category = await _context.ExpenseCategories.FindAsync(id);
        if (category != null)
        {
            // Delete subcategories first
            var subCategories = await GetSubCategoriesAsync(id);
            _context.ExpenseCategories.RemoveRange(subCategories);
            
            _context.ExpenseCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task InitializeDefaultCategoriesAsync()
    {
        // For shared categories, we use UserId 0. 
        // We bypass the filter to check absolute count
        var existingCount = await _context.ExpenseCategories
            .IgnoreQueryFilters()
            .CountAsync(c => c.UserId == 0);

        if (existingCount > 0) return;

        var categories = new List<ExpenseCategory>
        {
            new() { Name = "Food & Dining", Icon = "restaurant", Color = "#FF6B6B", UserId = 0 },
            new() { Name = "Transportation", Icon = "directions_car", Color = "#4ECDC4", UserId = 0 },
            new() { Name = "Shopping", Icon = "shopping_cart", Color = "#45B7D1", UserId = 0 },
            new() { Name = "Entertainment", Icon = "movie", Color = "#FFA07A", UserId = 0 },
            new() { Name = "Bills & Utilities", Icon = "receipt", Color = "#98D8C8", UserId = 0 },
            new() { Name = "Healthcare", Icon = "local_hospital", Color = "#F7DC6F", UserId = 0 },
            new() { Name = "Education", Icon = "school", Color = "#BB8FCE", UserId = 0 },
            new() { Name = "Personal Care", Icon = "spa", Color = "#85C1E2", UserId = 0 },
            new() { Name = "Travel", Icon = "flight", Color = "#F8B739", UserId = 0 },
            new() { Name = "Others", Icon = "more_horiz", Color = "#95A5A6", UserId = 0 }
        };

        _context.ExpenseCategories.AddRange(categories);
        await _context.SaveChangesAsync();
    }

    // Additional methods for UI
    public async Task<List<ExpenseCategory>> GetAllCategoriesAsync()
    {
        await EnsureTenantAsync();
        return await _context.ExpenseCategories
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<ExpenseCategory>> GetMainCategoriesAsync()
    {
        return await GetAllAsync();
    }

    public async Task<ExpenseCategory?> GetCategoryByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<ExpenseCategory> CreateCategoryAsync(ExpenseCategory category)
    {
        return await CreateAsync(category);
    }

    public async Task UpdateCategoryAsync(ExpenseCategory category)
    {
        await UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await DeleteAsync(id);
    }
}
