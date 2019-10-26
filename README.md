# inshapardaz
Language Services for Urdu

# Build Status

Master 

[![Build status](https://ci.appveyor.com/api/projects/status/xoq9t6wau07b6hmq/branch/master?svg=true)](https://ci.appveyor.com/project/umerfaruk/api/branch/master)

# Components

1. Rest API
2. Database
3. Front-End

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

- Run with code coverage
`dotnet test --results-directory:Coverage --collect:"Code Coverage"`

#### Alternate npm commands

- Start app locally
`npm run`

- Build the app
`npm run build`

- Run tests
`npm run test`

- Run with code coverage
`npm run test:cover`


### Running migrations

Project now uses Fluent migrator tool to create database migrations. You need to install the tool manually on local machine to work with it.

`dotnet tool install -g FluentMigrator.DotNet.Cli`
