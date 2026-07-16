using System.Text;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TechBlog.Application;
using TechBlog.Application.Common;
using TechBlog.Application.Common.Behaviors;
using TechBlog.Infrastructure;
using TechBlog.WebApi.Middleware;
using TechBlog.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

{
    var services = builder.Services;

    services.AddControllers();
    services.AddOpenApi();
    services.AddHttpContextAccessor();

    services.AddApplication();
    services.AddInfrastructure(builder.Configuration);

    // AddMediator() only exists because Mediator.SourceGenerator is
    // referenced in THIS project - the generated implementation still
    // picks up every IRequestHandler defined in TechBlog.Application
    // transitively (that's specifically supported, not something relying
    // on undocumented behavior).
    services.AddMediator(options =>
    {
        options.ServiceLifetime = ServiceLifetime.Scoped;
        options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
    });

    services.AddScoped<ICurrentUserService, CurrentUserService>();

    // Frontend runs on a different origin (localhost:4200 in dev) -
    // without this, the browser blocks every fetch() call before it even
    // reaches a controller. Configurable so prod can point at the real
    // deployed frontend URL instead of hardcoding localhost.
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? ["http://localhost:4200"];
    services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    services.AddAuthorization();
}

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// For integration tests (WebApplicationFactory) - not used yet, this
// branch doesn't add tests, but costs nothing to have ready.
public partial class Program;
