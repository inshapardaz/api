using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library;

public class PageSummaryModel
{
    public int BookId { get; set; }

    public List<PageStatusSummaryModel> Statuses { get; set; }
}
