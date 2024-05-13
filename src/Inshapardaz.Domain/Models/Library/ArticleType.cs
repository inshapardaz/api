using System.ComponentModel;

namespace Inshapardaz.Domain.Models.Library;

public enum ArticleType
{
    [Description("Unknown")]
    Unknown,
    [Description("Writing")]
    Writing,
    [Description("Poetry")]
    Poetry
}
