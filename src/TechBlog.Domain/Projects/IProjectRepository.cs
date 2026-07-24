namespace TechBlog.Domain.Projects;

public interface IProjectRepository
{
    Task<List<Project>> GetAllForUserAsync(int userId, CancellationToken ct = default);

    Task<Project?> GetByIdForUserAsync(int id, int userId, CancellationToken ct = default);

    Task<List<Project>> GetAllPublishedAsync(CancellationToken ct = default);

    Task<Project?> GetPublishedByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Project project, CancellationToken ct = default);

    void Remove(Project project);
}
