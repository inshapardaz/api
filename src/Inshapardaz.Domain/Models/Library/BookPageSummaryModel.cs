using System;
using System.Collections.Generic;
using System.Text;

namespace Inshapardaz.Domain.Models.Library
{
    public class BookPageSummaryModel
    {
        public int BookId { get; set; }

        public List<PageSummaryModel> Statuses { get; set; }
    }

    public class PageSummaryModel
    {
        public PageStatuses Status { get; set; }
        public int Count { get; set; }
    }
}
