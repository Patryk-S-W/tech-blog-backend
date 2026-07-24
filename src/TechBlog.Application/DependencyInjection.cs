using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TechBlog.Application.Announcements;
using TechBlog.Application.Projects;

namespace TechBlog.Application;

public static class DependencyInjection
{
    /// <summary>Registers everything Application owns EXCEPT Mediator
    /// itself - the generated AddMediator() extension only exists in
    /// whichever project references Mediator.SourceGenerator (WebApi, the
    /// "edge" project per the library's own docs), so that call has to
    /// happen there, not here. This method still needs to run first/also,
    /// for FluentValidation and the Mapperly mapper.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Mapperly mappers are stateless - safe (and cheaper) as singletons
        services.AddSingleton<AnnouncementMapper>();
        services.AddSingleton<ProjectMapper>();

        return services;
    }
}
