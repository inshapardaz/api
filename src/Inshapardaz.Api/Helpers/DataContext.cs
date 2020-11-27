using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Inshapardaz.Api.Entities;

namespace Inshapardaz.Api.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultDatabase"));
        }
    }
}