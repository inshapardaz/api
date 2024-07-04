using System;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests;

[SetUpFixture]
public class GlobalFixture
{
    private readonly MigrationHelper _helper;
    
    private long LatestMigrationVersion { get; set; }

    private long InitialDatabaseVersion { get; set; }
    
    public GlobalFixture()
    {
        _helper = new MigrationHelper();
        var startingMigrationState = _helper.GetMigrationInfo();
        InitialDatabaseVersion = startingMigrationState.LatestDatabaseVersion;
        LatestMigrationVersion = startingMigrationState.LatestAssemblyVersion;
    }
    
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("RUN_MIGRATIONS")))
        {
            _helper.RunUpTo(LatestMigrationVersion);
        }
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("RUN_MIGRATIONS")))
        {
            _helper.RunDownTo(InitialDatabaseVersion);
        }
    }
}