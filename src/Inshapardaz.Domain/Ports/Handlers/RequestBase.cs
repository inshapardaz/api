using Paramore.Brighter;
using System;

namespace Inshapardaz.Domain.Models
{
    public abstract class RequestBase : IRequest
    {
        public Guid Id { get; set; }
    }
}
