using Microsoft.EntityFrameworkCore;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Common;
using TechBlog.Domain.Users;

namespace TechBlog.Infrastructure.Persistence;

public sealed class DataContext(DbContextOptions<DataContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        // Domain events are raised (see Announcement.Create) but not
        // dispatched anywhere yet - there's no handler that needs to react
        // to one currently, and wiring a publish pipeline for zero
        // consumers is the same kind of dead weight the old, unused
        // Common/DomainEvent.cs already was. Clearing them here so they
        // don't pile up on long-lived tracked entities; swap this for an
        // actual IPublisher.Publish(...) call (wrapped per Jason Taylor's
        // Clean Architecture template pattern, since Domain can't reference
        // Mediator directly) once something needs to react to one.
        var entitiesWithEvents = ChangeTracker.Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        foreach (var entity in entitiesWithEvents)
            entity.ClearDomainEvents();

        return result;
    }
}
