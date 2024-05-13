using System.ComponentModel;

namespace Inshapardaz.Domain.Models;

public enum EditingStatus
{
    [Description("All")]
    All = 0,

    [Description("Available")]
    Available,

    [Description("Typing")]
    Typing,

    [Description("Typed")]
    Typed,

    [Description("InReview")]
    InReview,

    [Description("Completed")]
    Completed
}
