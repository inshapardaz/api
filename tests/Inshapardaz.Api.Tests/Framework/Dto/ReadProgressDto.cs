using System;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Tests.Framework.Dto;

public class ReadProgressDto
{
    public int LibraryId { get; set; }

    public int BookId { get; set; }
    
    public int Account { get; set; }
    
    public ProgressType ProgressType { get; set; }

    public long ProgressId { get; set; }

    public double ProgressValue { get; set; }

    public DateTime DateRead { get; set; }
}
