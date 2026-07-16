using System.Security.Claims;
using TechBlog.Application.Common;

namespace TechBlog.WebApi.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("No authenticated user.");
            return int.Parse(value);
        }
    }

    public string? Username => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
}
