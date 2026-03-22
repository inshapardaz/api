using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Library;

public abstract class LibraryBaseQuery<T>(int libraryId) : IQuery<T>
{
    public int LibraryId { get; private set; } = libraryId;
}
