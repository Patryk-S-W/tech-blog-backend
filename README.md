# TechBlog

The TechBlog project is a fullstack application written in Angular and .NET that is used to publish articles related to technology, programming and topics related to artificial intelligence.

## Backend technologies stack

- .NET 10
- Entity Framework Core 10 (Npgsql / PostgreSQL)
- Mediator (CQRS - commands/queries, source-generator based, MIT)
- FluentValidation
- Riok.Mapperly (source-generator mapping, MIT)
- BCrypt.Net-Next (password hashing)
- Scalar (OpenAPI documentation UI)

## Architecture

Clean Architecture + DDD, split into four projects under `src/`:

```
src/
  TechBlog.Domain/         Entities, value objects, domain events - zero
                           external dependencies
  TechBlog.Application/    CQRS commands/queries + handlers, validators,
                           mapping - depends only on Domain
  TechBlog.Infrastructure/ EF Core, repositories, BCrypt, JWT - implements
                           the interfaces Domain/Application define
  TechBlog.WebApi/         Controllers, Program.cs, the actual ASP.NET
                           Core host
```

Dependencies point inward: WebApi depends on Application and
Infrastructure; Infrastructure depends on Application; Application
depends on Domain; Domain depends on nothing.

## System requirements

- .NET 10.0+
- PostgreSQL

## Getting started

```bash
cd src/TechBlog.WebApi
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Postgres_Db" "Host=localhost;Port=5432;Database=techblog;Username=...;Password=..."
dotnet user-secrets set "AppSettings:Token" "<random string, 64+ characters>"

dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Development server

From `src/TechBlog.WebApi`, run `dotnet watch run` for a dev server.
Navigate to `http://localhost:5284/scalar` for interactive API
documentation. The application will automatically reload if you change
any of the source files.

## License

MIT License

Copyright (c) 2024 Patryk Sadowski

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
