using System;
using Paramore.Brighter;

namespace Inshapardaz.Api.Adapters
{
    public abstract class RequestBase : IRequest
    {
        public Guid Id { get; set; }
    }
}
