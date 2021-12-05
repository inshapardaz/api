namespace Inshapardaz.Domain.Models.Library
{
    public class LibraryUserModel
    {
        public int LibraryId { get; set; }
        public int AccountId { get; set; }

        public Role Role { get; set; }
    }
}
