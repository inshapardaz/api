﻿namespace Inshapardaz.Domain.Models.Library;

public class AuthorModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long? ImageId { get; set; }

    public string ImageUrl { get; set; }

    public int BookCount { get; set; }
    public int ArticleCount { get; set; }
    public int PoetryCount { get; set; }

    public AuthorTypes AuthorType { get; set; }
}
