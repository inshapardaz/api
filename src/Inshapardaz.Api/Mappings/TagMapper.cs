using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Mappings;

public static class TagMapper
{
    public static TagView Map(this TagModel source)
        => new TagView
        {
            Id = source.Id,
            Name = source.Name
        };

    public static TagModel Map(this TagView source)
        => new TagModel
        {
            Id = source.Id,
            Name = source?.Name
        };
}
