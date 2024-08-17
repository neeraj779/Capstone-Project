using Swar.API.Interfaces.Services;
using System.Security.Claims;

public class UserMiddleware
{
    private readonly RequestDelegate _next;

    public UserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        var sub = context.User.FindFirst(ClaimTypes.Email)?.Value;
        if (sub != null)
            context.Items["UserId"] = await userService.GetUserIdByEmail(sub);

        await _next(context);
    }
}
