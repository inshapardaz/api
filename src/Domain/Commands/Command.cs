using System;
using paramore.brighter.commandprocessor;

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
