using Microsoft.EntityFrameworkCore;
using TechBlog.Domain.Users;

namespace TechBlog.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(DataContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default) =>
        await context.Set<RefreshToken>().AddAsync(refreshToken, ct);

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default) =>
        await context.Set<RefreshToken>()
            .FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task RevokeAllForUserAsync(int userId, CancellationToken ct = default)
    {
        var tokens = await context.Set<RefreshToken>()
            .Where(r => r.UserId == userId)
            .ToListAsync(ct);
        context.Set<RefreshToken>().RemoveRange(tokens);
    }
}
