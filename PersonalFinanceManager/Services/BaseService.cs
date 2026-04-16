using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public abstract class BaseService
{
    protected readonly AppDbContext _context;
    protected readonly TenantService _tenantService;

    protected BaseService(AppDbContext context, TenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    protected async Task EnsureTenantAsync()
    {
        if (_context.CurrentUserId == 0)
        {
            var userId = await _tenantService.GetUserIdAsync();
            if (userId.HasValue)
            {
                _context.CurrentUserId = userId.Value;
            }
        }
    }
}
