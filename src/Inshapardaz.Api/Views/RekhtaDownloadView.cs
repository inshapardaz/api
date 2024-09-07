﻿using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views;

public class RekhtaDownloadView
{
    [Required]
    public string BookUrl { get; set; }
    public bool ConvertToPdf { get; set; }
}
