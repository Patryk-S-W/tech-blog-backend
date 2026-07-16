using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Common;
using TechBlog.Domain.Users;
using TechBlog.Infrastructure.Persistence;
using TechBlog.Infrastructure.Persistence.Repositories;
using TechBlog.Infrastructure.Services;

namespace TechBlog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres_Db")
            ?? throw new InvalidOperationException("Connection string 'Postgres_Db' is not configured.");

        services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DataContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
