# inshapardaz

API for Inshapardaz

# Build Status

[![Docker Image CI](https://github.com/inshapardaz/api/actions/workflows/docker-image.yml/badge.svg)](https://github.com/inshapardaz/api/actions/workflows/docker-image.yml)

# Components

1. Rest API

## Tools and Frameworks requires

- Dotnet Core (https://www.microsoft.com/net/core)
- SQL Server 2016

### Rest Service and WebSite

Checkout code and

- Restore packages
  `dotnet restore`

- Build the app
  `dotnet build`
  
- Start app locally
  `dotnet run`

- Run tests
  `dotnet test`

#### Running locally

Use `dotnet run` to start local server at port 4000

#### Swagger

Swagger specifications can be found at /swagger/v1/swagger.json and /swagger for UI.


## Database migration

Database migrations are defined in the `db/Inshapardaz.Database.Migrations` folder as FluentMigrations project. 

### Adding a new migration

- Create a new migration using the numeric incrementing migration starting at `000001`
- Run tests in `tests/Inshapardaz/Database.Migration.MySql.Tests` and `tests/Inshapardaz/Database.Migration.SqlServer.Tests` and ensure they pass.
- Commit the migrations code

### Running migrations

Migrations run on startup of api. It will only run migration on the database defined in the `appsettings.json` file connection string and ensure the database is same as latest migration.
 
### Initial migration

First migration would create a whole lot of tables. It will also create the root user for the database that can be used to access website and setup rest of the system. 
The user name and password can be defined by setting environment variables `NAWISHTA_ROOT_USER` and `NAWISHTA_ROOT_PASSWORD`. If these environment variables are not set, the default values for username and password would be used as defined in `db\Inshapardaz.Database.Migrations\Migration000001_Initial_Database.cs`.  Username must be a valid email, else user created would not be able to login and manage account.
