﻿using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Models;

public class ArticleFilter
{
    public int? AuthorId { get; set; }

    public int? CategoryId { get; set; }

    public bool? Favorite { get; set; }

    public bool? Read { get; set; }

    public EditingStatus Status { get; set; }

    public AssignmentStatus AssignmentStatus { get; set; }
    public ArticleType Type { get; set; }
    public int? TagId { get; set; }
}
