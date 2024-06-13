using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.File;

public class GetTextFileQuery : IQuery<string>
{
    public GetTextFileQuery(long fileId)
    {
        FileId = fileId;
    }

    public long FileId { get; private set; }
}

public class GetTextFileQueryHandler : QueryHandlerAsync<GetTextFileQuery, string>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public GetTextFileQueryHandler(IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    public override async Task<string> ExecuteAsync(GetTextFileQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var file = await _fileRepository.GetFileById(query.FileId, cancellationToken);

        if (string.IsNullOrWhiteSpace(file.FilePath))
        {
            throw new NotFoundException();
        }

        var contents = await _fileStorage.GetTextFile(file.FilePath, cancellationToken);
        if (contents == null)
        {
            throw new NotFoundException();
        }

        return contents;
    }
}
