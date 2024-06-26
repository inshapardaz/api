﻿using Paramore.Brighter;
using System;
using System.Diagnostics;

namespace Inshapardaz.Domain.Ports.Command;

public abstract class RequestBase : IRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Activity Span { get; set; }
}
