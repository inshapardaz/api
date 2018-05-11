using System;
using AutoMapper;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Ports.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Ports.Database
{

    public static class DatabaseModule
    {
        public static Profile GetMappingProfile()
        {
            return new MappingProfile();
        }

        public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDictionaryRepository, DictionaryRepository>();
            services.AddTransient<IWordRepository, WordRepository>();
            services.AddTransient<IMeaningRepository, MeaningRepository>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<IRelationshipRepository, RelationshipRepository>();

            var connectionString = configuration.GetConnectionString("DefaultDatabase");

            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<DatabaseContext>(
                        options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));
            services.AddTransient<IDatabaseContext, DatabaseContext>();

            bool.TryParse(configuration["Application:RunDBMigrations"], out bool migrationEnabled);
            if (migrationEnabled)
            {
                Console.WriteLine("Running database migrations...");
                var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
                optionsBuilder.UseSqlServer(connectionString);

                var database = new DatabaseContext(optionsBuilder.Options).Database;
                database.SetCommandTimeout(5 * 60);
                database.Migrate();
                Console.WriteLine("Database migrations completed.");
            }
        }
    }
}
