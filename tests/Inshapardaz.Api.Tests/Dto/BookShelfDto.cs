namespace Inshapardaz.Api.Tests.Dto
{
    public class BookShelfDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public int? ImageId { get; set; }

        public int AccountId { get; set; }
        public int LibraryId { get; set; }
    }
}
