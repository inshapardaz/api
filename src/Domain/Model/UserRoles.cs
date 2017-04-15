using System.ComponentModel;

namespace Inshapardaz.Domain.Model
{
    public enum UserRoles
    {
        [Description("Guest")]
        Guest,
        [Description("Reader")]
        Reader,
        [Description("Contributor")]
        Contributor,
        [Description("Developer")]
        Developer,
        [Description("Admin")]
        Admin
    }
}
