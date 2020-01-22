using System;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports
{
    public abstract class RequestBase : IRequest
    {
        public Guid Id { get; set; }
    }
}
