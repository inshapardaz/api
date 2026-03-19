# Nawishta

Nawishta is an electronic books cataloging and publishing platform. This is the backend REST API.

## Technology Stack

- **.NET 8.0** / ASP.NET Core Web API
- **Minimal hosting model** (top-level statements in `Program.cs`, no `Startup.cs`)
- **CQRS** via [Paramore Brighter](https://github.com/BrighterCommand/Brighter) (commands) and [Paramore Darker](https://github.com/BrighterCommand/Darker) (queries)
- **Dapper** micro-ORM for data access (not Entity Framework)
- **FluentMigrator** for database schema migrations
- **Serilog** for structured logging (console + Elasticsearch sinks)
- **Swashbuckle** for Swagger/OpenAPI documentation
- **BCrypt** for password hashing, custom JWT authentication
- **Newtonsoft.Json** for JSON serialization
- **NUnit 4** + FluentAssertions + AutoFixture + Bogus for testing
- **Docker** multi-stage build targeting `mcr.microsoft.com/dotnet/aspnet:8.0`

## Solution Structure

```
Inshapardaz.Api.sln (14 projects)
├── src/
│   ├── Inshapardaz.Api/                          # ASP.NET Core Web API (entry point)
│   │   ├── Controllers/                           # 19 API controllers
│   │   ├── Converters/                            # Renderers (HATEOAS link generation)
│   │   ├── Mappings/                              # Static extension method mappers (Model ↔ View)
│   │   ├── Views/                                 # DTOs with ViewWithLinks base class
│   │   ├── Infrastructure/                        # Middleware, DI config, factories
│   │   ├── Extensions/                            # Utility extension methods
│   │   └── Helpers/                               # Helper classes
│   ├── Inshapardaz.Domain/                        # Core domain (no framework dependencies)
│   │   ├── Ports/Command/                         # CQRS command requests + handlers
│   │   ├── Ports/Query/                           # CQRS query requests + handlers
│   │   ├── Adapters/                              # Repository interfaces & service contracts
│   │   ├── Models/                                # Domain model POCOs
│   │   ├── Exception/                             # Domain exceptions
│   │   ├── Helpers/                               # String, file path, image helpers
│   │   └── Common/                                # Shared utilities
│   ├── Adapters/
│   │   ├── Database/MySql/                        # MySQL Dapper repository implementations
│   │   ├── Database/SqlServer/                    # SQL Server Dapper repository implementations
│   │   ├── Storage/Azure/                         # Azure Blob Storage adapter
│   │   ├── Storage/FileSystem/                    # Local file system storage adapter
│   │   ├── Storage/S3/                            # AWS S3 storage adapter
│   │   ├── Storage/SqlServer/                     # Database file storage adapter
│   │   └── Ocr.Google/                            # Google Cloud Vision OCR adapter
│   └── Inshapardaz.LibraryMigrator/              # Console app for data migration
├── db/
│   └── Inshapardaz.Database.Migrations/           # FluentMigrator schema migrations
├── tests/
│   ├── Inshapardaz.Api.Tests/                     # Integration tests (WebApplicationFactory)
│   ├── Inshapardaz.Database.Migrations.MySql.Tests/
│   └── Inshapardaz.Database.Migrations.SqlServer.Tests/
└── docs/                                          # API documentation
```

## Architecture

### Ports & Adapters (Hexagonal) with CQRS

The domain layer defines port interfaces. Adapter projects implement them for specific technologies.

**Request flow:**
```
HTTP Request
  → Controller (thin, dispatches to Brighter/Darker)
    → Command/Query Handler (business logic + authorization)
      → Repository Interface (domain port)
        → Dapper Implementation (MySQL or SQL Server adapter)
  → Renderer (domain model → view model with HATEOAS links)
  → HTTP Response
```

### Multi-Tenancy

Each Library is a tenant with its own database connection string, database type (MySQL or SQL Server), and file storage type. `LibraryConfigurationMiddleware` reads `libraryId` from the route on each request and configures a scoped `LibraryConfiguration` object. `DatabaseFactory` and `FileStorageFactory` resolve the correct repository and storage implementations at runtime.

### Middleware Pipeline

Configured in `Program.cs` in this order:
1. Swagger UI
2. CORS (open with credentials)
3. HTTPS Redirection
4. Authorization
5. Request Logging (custom)
6. `ErrorHandlerMiddleware` — maps domain exceptions to HTTP status codes (`BadRequestException`→400, `NotFoundException`→404, `UnauthorizedException`→401, `ForbiddenException`→403)
7. `LibraryConfigurationMiddleware` — loads per-request library config from route
8. `StatusCodeMiddleware`
9. `JwtMiddleware` — custom JWT validation, attaches account to `HttpContext.Items`
10. `CookieAuthenticationMiddleware`

### Authentication & Security

- Custom JWT implementation (not ASP.NET Identity): symmetric key signing
- Token settings: AccessTokenTTL (10 min), RefreshTokenTTL (2 days), ResetTokenTTL (1 day)
- Dual auth: Bearer token + Cookie
- Authorization enforced at handler level via Brighter pipeline attributes: `[LibraryAuthorize(step, Role...)]`
- Roles: `Admin`, `LibraryAdmin`, `Writer`, `Reader`

### Database Strategy

- Multi-database support: MySQL and SQL Server, switchable per library
- Dapper for all queries (raw SQL in repository implementations)
- FluentMigrator for migrations, run on startup by `MigrationService` (IHostedService)
- Every domain repository interface has both MySQL and SQL Server implementations

### File Storage

Pluggable via `IFileStorage` interface, 4 backends:
- **FileSystem** — local disk (`data/{libraryId}`)
- **Azure Blob Storage**
- **AWS S3**
- **Database** (MySQL or SQL Server)

Storage type is configurable per library.

## Coding Conventions

### Naming

| Element | Convention | Example |
|---|---|---|
| Domain models | `{Entity}Model` | `BookModel`, `AuthorModel` |
| View/DTO classes | `{Entity}View` | `BookView`, `AuthorView` |
| Renderers | `IRender{Entity}` / `{Entity}Renderer` | `IRenderBook` / `BookRenderer` |
| Mappers | `{Entity}Mapper` with `Map()` extension | `BookMapper.Map()` |
| Repository interfaces | `I{Entity}Repository` | `IBookRepository` |
| Commands | `{Verb}{Entity}Request` | `AddBookRequest`, `DeleteBookRequest` |
| Command handlers | `{Verb}{Entity}RequestHandler` | `AddBookRequestHandler` |
| Queries | `Get{Entity}Query` | `GetBookByIdQuery`, `GetBooksQuery` |
| Query handlers | `Get{Entity}QueryHandler` | `GetBookByIdQueryHandler` |
| Test classes | `When{Action}{Condition}` | `WhenAddingBookWithPermissions` |
| Private fields | `_camelCase` | `_bookRepository`, `_userHelper` |
| Properties | `PascalCase` | `LibraryId`, `BookId` |

**Note:** The codebase uses the spelling `Extentions` (e.g. `MiscExtentions`, `StringExtentions`) rather than `Extensions`. Follow this spelling in existing files for consistency.

### Command/Query Handler Pattern

Both the request class and its handler live in the **same file**.

**Commands** (`Ports/Command/`):
- Request extends `LibraryBaseCommand` (which extends `RequestBase`)
- Handler extends `RequestHandlerAsync<TRequest>`
- Authorization via `[LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]` attribute
- Results written to `command.Result`
- Returns `await base.HandleAsync(command, cancellationToken)` to continue pipeline

**Queries** (`Ports/Query/`):
- Query extends `LibraryBaseQuery<TResult>`
- Handler extends `QueryHandlerAsync<TQuery, TResult>`
- Override `ExecuteAsync()`, directly returns result

### Controller Pattern

- Inherit from `Controller` (not `ControllerBase`)
- No `[ApiController]` attribute (except `AccountsController`)
- Constructor-injected: `IAmACommandProcessor`, `IQueryProcessor`, renderer, `IUserHelper`
- Attribute routing: `[HttpGet("libraries/{libraryId}/books", Name = nameof(GetBooks))]`
- All routes scoped to `libraries/{libraryId}/...` (multi-tenant)
- Thin controllers: create command/query → dispatch → render result → return `IActionResult`
- Response patterns: `OkObjectResult` for GET, `CreatedResult` for POST, `NoContentResult` for DELETE

### Renderer / HATEOAS Pattern

- Interface + implementation: `IRenderBook` / `BookRenderer`
- Map domain model to view via `source.Map()`
- Build links conditionally based on user role
- Type-safe route references: `nameof(BookController.GetBookById)`
- Relation types from `RelTypes` constants

### Mappers

- Static extension method classes in `Mappings/`
- Manual two-way mapping (no AutoMapper): `BookModel.Map()` → `BookView` and vice versa

### View Models

- Base class: `ViewWithLinks` (adds `Links` collection)
- `PageView<T>` for paginated responses (PageSize, PageCount, CurrentPageIndex, TotalCount, Data)
- `[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]` to suppress nulls
- `[Required]` for validation

## Testing

### Framework
NUnit 4 with `WebApplicationFactory<Program>` for integration testing.

### Test Structure
- One test class per scenario, one file per class
- Organized by entity and operation: `Library/Book/AddBook/`, `Library/Book/GetBooks/`, etc.
- Classes named `When{Action}{Condition}`: `WhenAddingBookWithPermissions`

### Test Patterns
- `TestBase` provides `WebApplicationFactory<Program>`, authenticated `HttpClient`, and lazy data builders
- Constructor accepts `Role?` for auth level
- `[TestFixture(Role.Admin)]` parameterization for role-based test variants
- `[OneTimeSetUp]` for arrange + act, individual `[Test]` methods for assertions
- `[OneTimeTearDown]` calls `Cleanup()` for data teardown
- Fluent data builders: `AuthorBuilder.WithLibrary(LibraryId).Build(3)`
- Custom assert classes: `BookAssert.ShouldHaveSelfLink().ShouldHaveUpdateLink()`
- Response extensions: `_response.ShouldBeCreated()`, `_response.ShouldBeForbidden()`

### Test Support (`Framework/`)
- `Asserts/` — Custom fluent assertion classes per entity
- `DataBuilders/` — Builder classes for test data setup
- `DataHelpers/` — Database helpers for direct data verification
- `Dto/` — Test DTOs
- `Fakes/` — `FakeFileStorage`, `FakeSmtpClient`
- `Helpers/` — HTTP client extensions, test utilities

## Running the Project

### Prerequisites
- .NET 8.0 SDK
- MySQL or SQL Server instance
- (Optional) Docker

### Build & Run
```bash
dotnet build
dotnet run --project src/Inshapardaz.Api/Inshapardaz.Api.csproj
```

### Docker Compose
```bash
docker-compose up
```
Starts 3 services: API (port 5000), SQL Server (port 1433), MailHog (ports 8025/1025).

### Tests
```bash
dotnet test
```
Migration tests require `RUN_MIGRATIONS` environment variable.

### VS Code Tasks
- `build`, `clean`, `rebuild`, `restore` — standard dotnet build
- `test`, `test-with-coverage` — test execution
- `run-api`, `watch-api` — development server
- `publish-debug`, `publish-release` — publish

## Configuration (`appsettings.json`)

All settings under `AppSettings`:
- **Email** — SMTP host, port, credentials, TLS/SSL
- **Security** — JWT secret key, token TTLs, reset/register page paths
- **Database** — `DatabaseConnectionType` (MySql/SqlServer), `ConnectionString`
- **Storage** — `FileStoreType` (FileSystem/AzureBlobStorage/S3Storage/Database)
- **FrontEndUrl**, **DefaultLibraryId**, **Domain**, **Allowed_Origins**
- Supports `BASE_PATH` env variable for reverse proxy path prefix

## Domain Concepts

### Libraries

Platform is organized into Libraries. Library acts like a tenant. It is a walled garden which acts as an individual entity for consumers. Library can have books, authors, categories, periodicals, poetry and prose. Each library has an administrator. There can be other users added to library. These users can be writers or readers or restricted readers.

### Book

Book represents a book in the real world. It has title, one or more authors, description, title images, taxonomy, publisher, date published, language, etc. Book contains one or more chapters and also contains pages. Chapters contain text only. Pages are associated with chapters. A page can have an image (typically scanned from a real book) and text that can be typed by a person or automatically extracted by OCR.

Each book can have one or more documents (files) associated with it. These files can be PDF scanned copies, text content files, word documents or e-book format files. These files can be uploaded by a user or generated by the system when published.

When a book is added to the library, it is empty. User can take two paths: create a book manually or digitize.

**Digitizing workflow:**
1. User uploads a book, typically a PDF
2. Process the PDF to split two-page scans into single pages and create pages from the images
3. Define chapters and associate pages with chapters
4. Create text using OCR or manual typing in a side-by-side editor
5. Once all pages are typed, the book is marked for proof reading
6. All pages are proof read and marked completed
7. Pages text is joined into chapter text
8. Chapters are proof read
9. Book is published (e-pub, text, word document, or embedded reader)

**Page/Chapter statuses:** Incomplete → Being Typed → Typed → Being Proof Read → Completed

**Book statuses:** Available, Being Typed, Typed, Proof Read, Published

### Periodicals

Periodicals are published at regular intervals: Daily, Weekly, BiWeekly, Monthly, Quarterly, Yearly, or Occasionally.

#### Issues
Each periodical has issues identified by Volume Number and Issue Number with a publish date. Issues are like books but have an editor instead of an author, articles instead of chapters, and articles can have their own authors. Issues have files and can be digitized just like books.

### Authors

Authors have name, image, and type (Poet or Prose writer). Authors can be associated with books, articles, issue articles, poetry, etc.

### Poetry

Independent poetry text created by poets with title, content, taxonomy and poet name. Can optionally be associated with a book and chapter.

**Statuses:** Incomplete → Typing → Typed → Proof Reading → Published

### Articles

Independent prose text created by writers with title, content, taxonomy and writer name. Can optionally be associated with a book and chapter.

Same statuses as poetry.

### Users & Roles

| Role | Permissions |
|---|---|
| **Reader** | Read published library content |
| **Writer** | Reader + create/edit books, periodicals |
| **LibraryAdmin** | Writer + delete books, manage categories/authors/periodicals/members within their library |
| **Admin** | Everything + create/edit/delete libraries, manage all memberships |

### Permission Model

Books, periodical issues, poetry and articles have visibility: **Public**, **Protected**, or **Private**.

- **Public** — visible to everyone including anonymous users
- **Protected** — visible only to library members
- **Private** — visible only to admins, librarians and writers

Permissions cascade to child resources (e.g. book permissions apply to its chapters, pages and files).
