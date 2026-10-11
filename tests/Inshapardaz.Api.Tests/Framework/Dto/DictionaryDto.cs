namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class DictionaryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public int EntryCount { get; set; }
        public int? OwnerId { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class DictionaryArticleDto
    {
        public long Id { get; set; }
        public long DictionaryId { get; set; }
        public string Headword { get; set; }
        public string HeadwordNormalised { get; set; }
        public string Pronunciation { get; set; }
        public string Senses { get; set; }
        public string Links { get; set; }
        public string RawDefinition { get; set; }
        public string Alternates { get; set; }
    }
}
