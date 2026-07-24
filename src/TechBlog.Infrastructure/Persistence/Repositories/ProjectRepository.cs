using Microsoft.EntityFrameworkCore;
using TechBlog.Domain.Projects;

namespace TechBlog.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(DataContext context) : IProjectRepository
{
    public Task<List<Project>> GetAllForUserAsync(int userId, CancellationToken ct = default) =>
        context.Projects
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.DateCreated)
            .ToListAsync(ct);

    public Task<Project?> GetByIdForUserAsync(int id, int userId, CancellationToken ct = default) =>
        context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId, ct);

    public Task<List<Project>> GetAllPublishedAsync(CancellationToken ct = default) =>
        context.Projects
            .Where(p => p.Active)
            .OrderByDescending(p => p.DateCreated)
            .ToListAsync(ct);

    public Task<Project?> GetPublishedByIdAsync(int id, CancellationToken ct = default) =>
        context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.Active, ct);

    public async Task AddAsync(Project project, CancellationToken ct = default) =>
        await context.Projects.AddAsync(project, ct);

    public void Remove(Project project) =>
        context.Projects.Remove(project);
}
