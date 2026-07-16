namespace TechBlog.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default);
    Task<bool> ExistsAsync(Username username, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
}
