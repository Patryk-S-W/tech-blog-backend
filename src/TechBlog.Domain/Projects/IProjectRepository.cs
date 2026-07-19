namespace TechBlog.Domain.Projects;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(CancellationToken ct = default);

    Task<Project?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<List<Project>> GetAllPublishedAsync(CancellationToken ct = default);

    Task<Project?> GetPublishedByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Project project, CancellationToken ct = default);

    void Remove(Project project);
}