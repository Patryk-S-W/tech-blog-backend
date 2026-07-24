namespace TechBlog.Application.Auth;

public sealed record LoginResponse(string AccessToken, string RefreshToken);
