using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.File;

public class GetTextFileQuery(long fileId) : IQuery<string>
{
    public long FileId { get; private set; } = fileId;
}

public class GetTextFileQueryHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    : QueryHandlerAsync<GetTextFileQuery, string>
{
    public override async Task<string> ExecuteAsync(GetTextFileQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var file = await fileRepository.GetFileById(query.FileId, cancellationToken);

        if (string.IsNullOrWhiteSpace(file.FilePath))
        {
            throw new NotFoundException();
        }

        var contents = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
        if (contents == null)
        {
            throw new NotFoundException();
        }

        return contents;
    }
}
