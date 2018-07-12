namespace Inshapardaz.Domain
{
    public class Settings
    {
        public bool RunDBMigrations { get; set; }

        public int DefaultDictionaryId { get; set; }

        public string ElasticsearchUrl { get; set; }

        public string Audience { get; set; }
        public string Authority { get; set; }
    }
}
