using System.ComponentModel;

namespace Inshapardaz.Domain.Models
{
    public enum CopyrightStatuses
    {
        [Description("Copyright")]
        Copyright = 0,

        [Description("PublicDomain")]
        PublicDomain,

        [Description("Open")]
        Open,

        [Description("CreativeCommons")]
        CreativeCommons
    }
}
