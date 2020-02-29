using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Handlers.Library
{
    public abstract class LibraryBaseCommand : RequestBase
    {
        public LibraryBaseCommand(int libraryId)
        {
            LibraryId = libraryId;
        }

        public int LibraryId { get; private set; }
    }
}
