using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserManagementApp.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System;

public class UserStatusCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserStatusCheckMiddleware> _logger;

    public UserStatusCheckMiddleware(RequestDelegate next, ILogger<UserStatusCheckMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (path.StartsWith("/account/login") || path.StartsWith("/account/register") || path.StartsWith("/account/logout") || path.StartsWith("/favicon.ico") || path.StartsWith("/lib/") || path.StartsWith("/css/") || path.StartsWith("/js/"))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity.IsAuthenticated)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = await userManager.FindByIdAsync(userId);

                _logger.LogInformation("Checking status for authenticated user with ID: {UserId}", userId);

                if (user == null || user.IsBlocked)
                {
                    _logger.LogWarning("User with ID {UserId} is blocked or not found. Signing out and redirecting to login.", userId);
                    await signInManager.SignOutAsync();

                    context.Response.Cookies.Append("BlockedStatus", "Your account has been blocked or deleted. Please log in again.", new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(1)
                    });

                    context.Response.Redirect("/Account/Login");
                    return;
                }
                _logger.LogInformation("User with ID {UserId} is active.", userId);
            }
        }

        await _next(context);
    }
}