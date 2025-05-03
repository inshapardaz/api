using System;

namespace Inshapardaz.Domain.Models.Library;

public class ReadProgressModel
{
    public ProgressType ProgressType { get; set; }

    public long ProgressId { get; set; }

    public double ProgressValue { get; set; }

    public DateTime DateRead { get; set; }
}
