global using tech_blog_backend.Models;
global using tech_blog_backend.Data;
global using tech_blog_backend.Services.AnnouncementService;
global using tech_blog_backend.Dtos.Announcement;
global using Microsoft.EntityFrameworkCore;
global using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

{
    var services = builder.Services;


    services.AddDbContext<DataContext>(options =>
      //   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
      options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres_Db")));
    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechBlog", Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = """Standard Authorization header using the Bearer scheme. Example: "bearer {token}" """,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        c.OperationFilter<SecurityRequirementsOperationFilter>();
    });
    services.AddAutoMapper(typeof(Program).Assembly);
    services.AddScoped<IAnnouncementService, AnnouncementService>();
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();