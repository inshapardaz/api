namespace Inshapardaz.Domain
{
    public class DatabaseFactory
    {
        public DatabaseFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }
}
