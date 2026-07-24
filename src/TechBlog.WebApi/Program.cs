using Asp.Versioning;
using System.Text;
using System.Threading.RateLimiting;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TechBlog.Application;
using TechBlog.Application.Common;
using TechBlog.Application.Common.Behaviors;
using TechBlog.Infrastructure;
using TechBlog.Infrastructure.Persistence;
using TechBlog.WebApi.Middleware;
using TechBlog.WebApi.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

{
    var services = builder.Services;

    services.AddControllers();
    services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
    services.AddOpenApi();
    services.AddHttpContextAccessor();

    services.AddApplication();
    services.AddInfrastructure(builder.Configuration);

    services.AddMediator(options =>
    {
        options.ServiceLifetime = ServiceLifetime.Scoped;
        options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
    });

    services.AddScoped<ICurrentUserService, CurrentUserService>();

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? ["http://localhost:4200"];
    services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
            policy.WithOrigins(allowedOrigins)
                .WithHeaders("Content-Type", "Authorization")
                .WithMethods("GET", "POST", "PUT", "DELETE"));
    });

    var jwtSettings = builder.Configuration.GetSection("AppSettings");
    var issuer = jwtSettings["Issuer"] ?? "TechBlog";
    var audience = jwtSettings["Audience"] ?? "TechBlog";

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
            };
        });
    services.AddAuthorization();

    services.AddResponseCaching();

    services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.AddFixedWindowLimiter("api", limiterOptions =>
        {
            limiterOptions.PermitLimit = 100;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueLimit = 0;
        });
        options.AddFixedWindowLimiter("auth", limiterOptions =>
        {
            limiterOptions.PermitLimit = 10;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueLimit = 0;
        });
    });
}


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseResponseCaching();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

public partial class Program;
