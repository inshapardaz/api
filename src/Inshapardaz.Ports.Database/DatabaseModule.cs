using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inshapardaz.Ports.Database
{

    public static class DatabaseModule
    {
        public static void MigrateToDatabase(IConfiguration configuration, IDatabaseContext context)
        {
            bool.TryParse(configuration["Application:RunDBMigrations"], out bool migrationEnabled);
            if (!migrationEnabled)
                return;
            
            context.Database.SetCommandTimeout(5 * 60);
            context.Database.Migrate();
            Console.WriteLine("Database migrations completed.");
        }
    }
}
