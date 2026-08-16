# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Nawishta — an electronic books cataloging and publishing platform. This repo is the backend REST API (`Inshapardaz.Api`).

## Commands

```bash
# Restore, build, run
dotnet restore
dotnet build
dotnet run --project src/Inshapardaz.Api/Inshapardaz.Api.csproj   # http://localhost:4000

# Test (whole solution)
dotnet test

# Test a single project
dotnet test tests/Inshapardaz.Api.Tests/Inshapardaz.Api.Tests.csproj

# Test a single fixture/class or method (NUnit filter syntax)
dotnet test --filter "FullyQualifiedName~WhenAddingBookWithPermissions"
dotnet test --filter "FullyQualifiedName~WhenAddingBookWithPermissions.ShouldReturnCreated"

# Migration tests require this env var, and run against a real MySQL/SQL Server instance
RUN_MIGRATIONS=1 dotnet test tests/Inshapardaz.Database.Migrations.MySql.Tests/Inshapardaz.Database.Migrations.MySql.Tests.csproj
RUN_MIGRATIONS=1 dotnet test tests/Inshapardaz.Database.Migrations.SqlServer.Tests/Inshapardaz.Database.Migrations.SqlServer.Tests.csproj

# Full local stack (API + SQL Server + MailHog)
docker-compose up
```

Swagger UI is served at `/swagger`; spec at `/swagger/v1/swagger.json`.

## Technology Stack

- .NET 9.0 / ASP.NET Core Web API, minimal hosting model (top-level statements in `Program.cs`, no `Startup.cs`)
- CQRS via Paramore **Brighter** (commands) and **Darker** (queries)
- **Dapper** for data access — no Entity Framework, raw SQL in repository implementations
- **FluentMigrator** for schema migrations, run on startup by `MigrationService` (`IHostedService`)
- **Serilog** for structured logging
- ASP.NET Core **JWT Bearer** authentication (`AddJwtBearer`), with cookie fallback (`OnMessageReceived` reads the `token` cookie when no `Authorization` header is present)
- **BCrypt** for password hashing
- **Newtonsoft.Json**/`System.Text.Json` for serialization
- NUnit 4 + FluentAssertions + AutoFixture + Bogus for testing, `WebApplicationFactory<Program>` for integration tests

## Solution Structure

```
src/
  Inshapardaz.Api/            # ASP.NET Core Web API (entry point)
    Controllers/               # API controllers, thin, dispatch to Brighter/Darker
    Converters/                # Renderers (HATEOAS link generation)
    Mappings/                  # Static extension method mappers (Model <-> View)
    Views/                     # DTOs, ViewWithLinks base class
    Infrastructure/            # Middleware, DI config, factories
    Extensions/ Helpers/
  Inshapardaz.Domain/          # Core domain, no framework dependencies
    Ports/Command/              # CQRS command requests + handlers (same file)
    Ports/Query/                 # CQRS query requests + handlers (same file)
    Adapters/                    # Repository interfaces & service contracts (ports)
    Models/ Exception/ Helpers/ Common/
  Adapters/
    Database/MySql/  Database/SqlServer/   # Dapper repository implementations
    Storage/Azure/ Storage/FileSystem/ Storage/S3/ Storage/SqlServer/  # IFileStorage backends
    Ocr.Google/                 # Google Cloud Vision OCR adapter
  Inshapardaz.LibraryMigrator/ # console app for data migration
db/Inshapardaz.Database.Migrations/   # FluentMigrator schema migrations, numbered MigrationNNNNNN files
tests/
  Inshapardaz.Api.Tests/                          # integration tests
  Inshapardaz.Database.Migrations.MySql.Tests/
  Inshapardaz.Database.Migrations.SqlServer.Tests/
docs/                          # API documentation
```

## Architecture

### Ports & Adapters (Hexagonal) with CQRS

```
HTTP Request
  -> Controller (thin, dispatches to Brighter/Darker)
    -> Command/Query Handler (business logic + authorization)
      -> Repository Interface (domain port)
        -> Dapper Implementation (MySQL or SQL Server adapter)
  -> Renderer (domain model -> view model with HATEOAS links)
  -> HTTP Response
```

The domain layer defines port interfaces (`Inshapardaz.Domain/Adapters`); adapter projects implement them per technology. Every domain repository interface has both a MySQL and a SQL Server implementation.

### Multi-Tenancy

Each Library is a tenant with its own database connection string, database type (MySQL or SQL Server), and file storage type. `LibraryConfigurationMiddleware` reads `libraryId` from the route on each request and configures a scoped `LibraryConfiguration`. `DatabaseFactory` and `FileStorageFactory` resolve the correct repository/storage implementation at runtime based on that config. All routes are scoped under `libraries/{libraryId}/...`.

### Middleware Pipeline

Configured in `Program.cs`, in order:
1. Swagger / SwaggerUI
2. CORS (open, credentials allowed)
3. HTTPS redirection
4. `UseAuthentication` (JWT Bearer, cookie fallback)
5. `UseAuthorization`
6. Request logging (custom)
7. `ErrorHandlerMiddleware` — maps domain exceptions to HTTP status (`BadRequestException`->400, `NotFoundException`->404, `UnauthorizedException`->401, `ForbiddenException`->403)
8. `LibraryConfigurationMiddleware` — loads per-request library config from the route
9. `StatusCodeMiddleware`

### Authentication & Authorization

- ASP.NET Core JWT Bearer auth (`Microsoft.AspNetCore.Authentication.JwtBearer`), symmetric key signing (`TokenGenerator`)
- `IUserHelper`/`UserHelper` reads the current account from `ClaimsPrincipal` claims (`id`, `ClaimTypes.Name`, `ClaimTypes.Email`, `isSuperAdmin`, `lib:{libraryId}:role`)
- Handler-level authorization via Brighter pipeline attribute `[LibraryAuthorize(step, Role...)]` -> `LibraryAuthorizeHandler<TRequest>`, which throws `UnauthorizedException`/`ForbiddenException`
- Roles: `Admin`, `LibraryAdmin`, `Writer`, `Reader`

### Database Strategy

Dapper for all queries. FluentMigrator migrations run automatically on API startup against the connection string in `appsettings.json`. When adding a migration: create a new numbered file starting at the next integer after the latest in `db/Inshapardaz.Database.Migrations`, then run both migration test projects (MySQL and SQL Server) to confirm up/down correctness before committing.

The very first migration creates the root user; its credentials come from `NAWISHTA_ROOT_USER`/`NAWISHTA_ROOT_PASSWORD` env vars (must be a valid email), falling back to the defaults in `Migration000001_Initial_Database.cs`.

### File Storage

Pluggable via `IFileStorage`, 4 backends selectable per library: FileSystem (local disk, `data/{libraryId}`), Azure Blob Storage, AWS S3, Database (MySQL/SQL Server).

## Coding Conventions

| Element | Convention | Example |
|---|---|---|
| Domain models | `{Entity}Model` | `BookModel` |
| View/DTO classes | `{Entity}View` | `BookView` |
| Renderers | `IRender{Entity}` / `{Entity}Renderer` | `IRenderBook` / `BookRenderer` |
| Mappers | `{Entity}Mapper` with `Map()` extension | `BookMapper.Map()` |
| Repository interfaces | `I{Entity}Repository` | `IBookRepository` |
| Commands | `{Verb}{Entity}Request` | `AddBookRequest` |
| Command handlers | `{Verb}{Entity}RequestHandler` | `AddBookRequestHandler` |
| Queries | `Get{Entity}Query` | `GetBookByIdQuery` |
| Query handlers | `Get{Entity}QueryHandler` | `GetBookByIdQueryHandler` |
| Test classes | `When{Action}{Condition}` | `WhenAddingBookWithPermissions` |

The codebase intentionally spells it `Extentions` (e.g. `MiscExtentions`, `StringExtentions`), not `Extensions`. Match this in existing files.

### Command/Query Handler Pattern

The request class and its handler live in the **same file**.

- **Commands** (`Ports/Command/`): request extends `LibraryBaseCommand` (-> `RequestBase`); handler extends `RequestHandlerAsync<TRequest>`; authorization via `[LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]`; results written to `command.Result`; call `await base.HandleAsync(command, cancellationToken)` to continue the pipeline.
- **Queries** (`Ports/Query/`): query extends `LibraryBaseQuery<TResult>`; handler extends `QueryHandlerAsync<TQuery, TResult>`; override `ExecuteAsync()` and return the result directly.

### Controller Pattern

- Inherit from `Controller` (not `ControllerBase`); no `[ApiController]` attribute except on `AccountsController`
- Constructor-injected `IAmACommandProcessor`, `IQueryProcessor`, a renderer, `IUserHelper`
- Attribute routing, e.g. `[HttpGet("libraries/{libraryId}/books", Name = nameof(GetBooks))]`
- Thin: build command/query -> dispatch -> render -> return `IActionResult` (`OkObjectResult` for GET, `CreatedResult` for POST, `NoContentResult` for DELETE)

### Renderer / HATEOAS Pattern

`IRender{Entity}`/`{Entity}Renderer` maps the domain model via `source.Map()`, then builds links conditionally on user role using type-safe route references (`nameof(BookController.GetBookById)`) and `RelTypes` constants.

### View Models

Base class `ViewWithLinks` adds a `Links` collection. `PageView<T>` for paginated responses (`PageSize`, `PageCount`, `CurrentPageIndex`, `TotalCount`, `Data`). `[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]` suppresses nulls; `[Required]` for validation.

## Testing

NUnit 4 with `WebApplicationFactory<Program>` (`tests/Inshapardaz.Api.Tests/TestBase.cs`) for integration testing.

- One test class per scenario, organized by entity/operation: `Library/Book/AddBook/`, `Library/Book/GetBooks/`, etc. Class names: `When{Action}{Condition}`.
- `TestBase` sets up the `WebApplicationFactory`, an authenticated `HttpClient` (bearer token built via `TokenBuilder`), and lazy data builders/repositories. Constructor takes `Role? role` for auth level.
- `[TestFixture(Role.Admin)]`-style parameterization for role-based variants.
- `[OneTimeSetUp]` does arrange + act; individual `[Test]` methods assert; `[OneTimeTearDown]` calls `Cleanup()`.
- Fluent data builders (`Framework/DataBuilders/`), e.g. `AuthorBuilder.WithLibrary(LibraryId).Build(3)`.
- Custom fluent assertions (`Framework/Asserts/`), e.g. `BookAssert.ShouldHaveSelfLink().ShouldHaveUpdateLink()`, and response extensions like `_response.ShouldBeCreated()` / `ShouldBeForbidden()`.
- `TestBase.DatabaseType` is currently hardcoded to `MySql` — both MySQL and SQL Server test-repository implementations exist in `Framework/DataHelpers/`, but only one is wired up at a time.
- `Framework/Fakes/` provides `FakeFileStorage` and `FakeSmtpClient`, swapped in via `ConfigureTestServices`.

## Configuration

`appsettings.json`, all settings under `AppSettings`:
- **Email** — SMTP host/port/credentials/TLS/SSL
- **Security** — JWT secret key, token TTLs (`AccessTokenTTL` 10 min, `RefreshTokenTTL` 2 days, `ResetTokenTTL` 1 day), reset/register page paths
- **Database** — `DatabaseConnectionType` (MySql/SqlServer), `ConnectionString`
- **Storage** — `FileStoreType` (FileSystem/AzureBlobStorage/S3Storage/Database)
- `FrontEndUrl`, `DefaultLibraryId`, `Domain`, `Allowed_Origins`
- `BASE_PATH` env var sets a reverse-proxy path prefix (`app.UsePathBase`)

## Domain Concepts

### Libraries
A Library is a tenant — a walled garden of books, authors, categories, periodicals, poetry and prose, with its own administrator and member users (writers, readers, restricted readers).

### Book
Has title, one or more authors, description, images, taxonomy, publisher, publish date, language. Contains chapters (text only) and pages (image + text, associated with a chapter). Can have multiple document files (PDF scans, text, Word, e-book) uploaded or system-generated on publish.

Digitizing workflow: upload a PDF -> split two-page scans into single pages -> define chapters and associate pages -> type text (manual or OCR, side-by-side editor) -> mark for proof reading -> proof read pages -> join page text into chapter text -> proof read chapters -> publish (e-pub, text, Word, or embedded reader).

Page/chapter statuses: Incomplete -> Being Typed -> Typed -> Being Proof Read -> Completed.
Book statuses: Available, Being Typed, Typed, Proof Read, Published.

### Periodicals
Published at a frequency (Daily/Weekly/BiWeekly/Monthly/Quarterly/Yearly/Occasionally). Each periodical has issues identified by volume + issue number and a publish date. Issues are like books but have an editor instead of an author and articles instead of chapters (articles can have their own authors); issues can be digitized the same way as books.

### Authors
Name, image, type (Poet or Prose writer). Associated with books, articles, issue articles, poetry.

### Poetry / Articles
Independent text (poetry or prose) with title, content, taxonomy, and poet/writer name; optionally associated with a book and chapter. Statuses: Incomplete -> Typing -> Typed -> Proof Reading -> Published.

### Users & Roles
| Role | Permissions |
|---|---|
| Reader | Read published library content |
| Writer | Reader + create/edit books, periodicals |
| LibraryAdmin | Writer + delete books, manage categories/authors/periodicals/members within their library |
| Admin | Everything + create/edit/delete libraries, manage all memberships |

### Permission Model
Books, periodical issues, poetry and articles have visibility **Public** (everyone, including anonymous), **Protected** (library members only), or **Private** (admins, librarians, writers only). Permissions cascade to child resources (e.g. a book's permission applies to its chapters, pages and files).
