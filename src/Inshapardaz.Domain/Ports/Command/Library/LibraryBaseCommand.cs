namespace Inshapardaz.Domain.Ports.Command.Library;

public abstract class LibraryBaseCommand(int libraryId) : RequestBase
{
    public int LibraryId { get; private set; } = libraryId;
}
