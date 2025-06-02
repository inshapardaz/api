using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class TagDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LibraryId { get; set; }

        public TagView ToView()
        {
            return new TagView
            {
                Id =  Id,
                Name = Name,
            };
        }
    }
}
