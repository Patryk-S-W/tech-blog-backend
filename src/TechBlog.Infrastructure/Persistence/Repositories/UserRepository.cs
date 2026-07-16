using Microsoft.EntityFrameworkCore;
using TechBlog.Domain.Users;

namespace TechBlog.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(DataContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<bool> ExistsAsync(Username username, CancellationToken ct = default) =>
        context.Users.AnyAsync(u => u.Username == username, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await context.Users.AddAsync(user, ct);
}
