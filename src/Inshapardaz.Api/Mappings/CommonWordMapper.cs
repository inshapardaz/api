using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Mappings;

public static class CommonWordMapper
{
    public static CommonWordView Map(this CommonWordModel source)
        => new CommonWordView
        {
            Id = source.Id,
            Word = source.Word,
            Language = source.Language
        };

    public static CommonWordModel Map(this CommonWordView source)
        => new CommonWordModel
        {
            Id = source.Id,
            Word = source.Word,
            Language = source.Language
        };
}
