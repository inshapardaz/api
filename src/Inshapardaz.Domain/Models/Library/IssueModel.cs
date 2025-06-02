using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library;

public class IssueModel
{
    public int Id { get; set; }

    public int IssueNumber { get; set; }

    public int VolumeNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public long? ImageId { get; set; }

    public int PeriodicalId { get; set; }

    public virtual PeriodicalModel Periodical { get; set; }

    public string ImageUrl { get; set; }

    public bool IsPublic { get; set; }

    public int ArticleCount { get; set; }

    public int PageCount { get; set; }

    public List<IssueContentModel> Contents { get; set; } = new();
    public List<TagModel> Tags { get; set; } = new List<TagModel>();
    public StatusType Status { get; set; }
    public IEnumerable<PageStatusSummaryModel> PageStatus { get; set; }
    public decimal Progress { get; set; }
}
