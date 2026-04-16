using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace PersonalFinanceManager.Services;

public class TenantService
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public TenantService(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    public async Task<int?> GetUserIdAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
        }

        return null;
    }
}
