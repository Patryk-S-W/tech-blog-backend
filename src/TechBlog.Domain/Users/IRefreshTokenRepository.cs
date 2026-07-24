namespace TechBlog.Domain.Users;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task RevokeAllForUserAsync(int userId, CancellationToken ct = default);
}
