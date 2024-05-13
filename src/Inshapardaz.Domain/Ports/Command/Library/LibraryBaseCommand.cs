namespace Inshapardaz.Domain.Models.Handlers.Library
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
