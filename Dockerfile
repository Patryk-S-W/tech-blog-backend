FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/TechBlog.Domain/TechBlog.Domain.csproj src/TechBlog.Domain/
COPY src/TechBlog.Application/TechBlog.Application.csproj src/TechBlog.Application/
COPY src/TechBlog.Infrastructure/TechBlog.Infrastructure.csproj src/TechBlog.Infrastructure/
COPY src/TechBlog.WebApi/TechBlog.WebApi.csproj src/TechBlog.WebApi/
RUN dotnet restore src/TechBlog.WebApi/TechBlog.WebApi.csproj

COPY src/TechBlog.Domain/ src/TechBlog.Domain/
COPY src/TechBlog.Application/ src/TechBlog.Application/
COPY src/TechBlog.Infrastructure/ src/TechBlog.Infrastructure/
COPY src/TechBlog.WebApi/ src/TechBlog.WebApi/
RUN dotnet publish src/TechBlog.WebApi/TechBlog.WebApi.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechBlog.WebApi.dll"]
