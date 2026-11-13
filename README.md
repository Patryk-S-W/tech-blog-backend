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

### Private account provisioning

The local CLI creates an ordinary account with the existing registration policy;
it does not grant new roles, replace passwords or expose an HTTP endpoint.
Set `ConnectionStrings__Postgres_Db` through trusted deployment configuration.
The database must already have all migrations applied; the CLI never migrates it.
No JWT key is needed. Do not pass credentials as command arguments.

```sh
dotnet run --project src/TechBlog.OwnerCli -- --username owner
docker compose exec backend dotnet /app/tools/owner/TechBlog.OwnerCli.dll --username owner
```

Both commands prompt for a hidden password and confirmation. For automation,
pipe one password line from a trusted secret source (never a literal shell command
or password environment variable):

```sh
trusted-secret-source | docker compose exec -T backend dotnet /app/tools/owner/TechBlog.OwnerCli.dll --username owner --password-stdin
```

Only `--username` and `--password-stdin` are accepted. Exit codes: 0 created,
2 invalid arguments/input/configuration, 1 database, duplicate-account or cancellation
failure. Failures do not replace an existing account or revoke its tokens.

### Article drafts and publication

`POST /api/v1/announcement` creates a private draft. Article responses include
`isPublished` (the existing Active state) and nullable `publishedAt` (first
publication time). Owner lists include drafts. Authenticated owner-only
`POST /api/v1/announcement/{id}/publish` and `/unpublish` return the updated
article (200); repeated transitions return 400. Another author's, deleted or
missing article returns 404. Unpublishing retains the first publication time.
Public article ID, slug, category and list endpoints exclude drafts/deleted
articles and use no-store caching. Unique-value conflicts return sanitized 409.

The additive publication migration preserves existing Active values, ownership,
content and creation dates; active rows inherit their nullable creation date as
publication time. Only the announcement Active column default becomes false.
Back up and review existing migration preconditions before deployment: older
slug uniqueness and project-owner foreign-key migrations can reject legacy data.
This feature does not repair those historical records or change article slugs.

Publication integration tests use the dedicated test settings below. The upgrade
test creates and drops its own `techblog_test_upgrade_*` database and requires
CREATEDB on the disposable PostgreSQL role; it never downgrades the shared test DB.

### Authentication and configuration

Public HTTP registration is disabled. Account provisioning must use a controlled,
trusted process; the application/domain registration commands remain available.
Login and refresh endpoints are unchanged. Existing accounts and tokens are not
revoked by this restriction and should be reviewed separately if needed.

For production, supply `ConnectionStrings__Postgres_Db` and `AppSettings__Token`
through the deployment environment (or an equivalent configuration provider).
The connection string must specify a PostgreSQL host, and the JWT signing key
must contain at least 64 UTF-8 bytes. Startup rejects missing, malformed, or
unexpanded `${...}` settings without including their values in validation errors.
JSON configuration does not interpolate shell-style placeholders. Development
can use the user secrets shown above; never commit real credentials.

Integration tests require explicit `TECHBLOG_TEST_CONNECTION` and
`TECHBLOG_TEST_JWT_KEY`, never runtime connection or JWT settings. The connection
must target localhost or 127.0.0.1 and a disposable database named `techblog_test_*`.
The test key must contain at least 64 UTF-8 bytes. Missing or unsafe settings fail
before test-host startup. Tests apply real migrations to this isolated database.
Run `dotnet test TechBlog.sln --configuration Release` with these dedicated settings.

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
