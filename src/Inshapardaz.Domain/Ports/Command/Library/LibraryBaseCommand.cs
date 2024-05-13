namespace Inshapardaz.Domain.Ports.Command.Library;

public abstract class LibraryBaseCommand : RequestBase
{
    public LibraryBaseCommand(int libraryId)
    {
        LibraryId = libraryId;
    }

    public int LibraryId { get; private set; }
}
