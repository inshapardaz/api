namespace Inshapardaz.Domain
{
    public class Settings
    {
        public bool RunDBMigrations { get; set; }

        public int DefaultDictionaryId { get; set; }

        public string ElasticsearchUrl { get; set; }
    }
}
