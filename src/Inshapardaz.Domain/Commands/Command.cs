using System;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands
{
    public abstract class Command : IRequest
    {
        protected Command()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}
