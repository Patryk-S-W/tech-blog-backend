global using tech_blog_backend.Models;
global using tech_blog_backend.Data;
global using tech_blog_backend.Services.AnnouncementService;
global using tech_blog_backend.Services.ProjectService;
global using tech_blog_backend.Dtos.Announcement;
global using Microsoft.EntityFrameworkCore;
using tech_blog_backend.Mappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

{
    var services = builder.Services;

    services.AddDbContext<DataContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres_Db")));
    services.AddControllers();

    services.AddOpenApi();

    // Mapperly-generated mappers have no state, safe as a singleton
    services.AddSingleton<AnnouncementMapper>();
    services.AddScoped<IAnnouncementService, AnnouncementService>();
    services.AddScoped<IProjectService, ProjectService>();
    services.AddScoped<IAuthRepository, AuthRepository>();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    services.AddHttpContextAccessor();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

// For integration tests (WebApplicationFactory)
public partial class Program;
