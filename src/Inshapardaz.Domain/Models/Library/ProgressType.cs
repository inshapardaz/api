using System.ComponentModel;

namespace Inshapardaz.Domain.Models.Library;

public enum ProgressType
{ 
    [Description("Unknown")]
    Unknown,
    [Description("Chapter")]
    Chapter,
    [Description("File")]
    File,
    [Description("Pages")]
    Pages,
}
