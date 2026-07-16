namespace TechBlog.Application.Common;

/// <summary>Replaces the old AnnouncementService's direct
/// IHttpContextAccessor dependency - Application shouldn't know HTTP
/// exists. WebApi provides the implementation reading the JWT claims.</summary>
public interface ICurrentUserService
{
    /// <summary>Throws if called with no authenticated user - every command/
    /// query that needs this is already behind [Authorize].</summary>
    int UserId { get; }

    string? Username { get; }
}
