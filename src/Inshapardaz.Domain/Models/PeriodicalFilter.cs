using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Models;

public class PeriodicalFilter
{
    public int? CategoryId { get; set; }
    public PeriodicalFrequency? Frequency { get; set; }
    public int? TagId { get; set; }
}
