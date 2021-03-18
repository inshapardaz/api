using System.ComponentModel;

namespace Inshapardaz.Domain.Models
{
    public enum PageStatuses
    {
        [Description("All")]
        All = -1,

        [Description("Available")]
        Available = 0,

        [Description("Typing")]
        Typing = 1,

        [Description("Typed")]
        Typed = 2,

        [Description("InReview")]
        InReview = 3,

        [Description("Completed")]
        Completed = 4
    }
}
