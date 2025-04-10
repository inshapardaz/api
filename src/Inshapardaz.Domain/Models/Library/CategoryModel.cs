﻿namespace Inshapardaz.Domain.Models.Library;

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int BookCount { get; internal set; }
    public int PeriodicalCount { get; internal set; }
    public int ArticleCount { get; internal set; }
    public int PoetryCount { get; internal set; }
}
