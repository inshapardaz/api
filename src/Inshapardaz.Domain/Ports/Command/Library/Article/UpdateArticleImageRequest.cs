using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class UpdateArticleImageRequest : LibraryBaseCommand
{
    public UpdateArticleImageRequest(int libraryId, long articleId, int? accountId)
        : base(libraryId)
    {
        ArticleId = articleId;
        AccountId = accountId;
    }

    public long ArticleId { get; }
    public int? AccountId { get; }
    public FileModel Image { get; set; }

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public FileModel File { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateArticleImageRequestHandler : RequestHandlerAsync<UpdateArticleImageRequest>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;

    public UpdateArticleImageRequestHandler(IArticleRepository articleRepository, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _articleRepository = articleRepository;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateArticleImageRequest> HandleAsync(UpdateArticleImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var article = await _articleRepository.GetArticle(command.LibraryId, command.ArticleId, cancellationToken);

        if (article == null)
        {
            throw new NotFoundException();
        }

        if (article.ImageId.HasValue)
        {
            command.Image.Id = article.ImageId.Value;
            var existingImage = await _fileRepository.GetFileById(article.ImageId.Value, cancellationToken);
            if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
            {
                await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
            }

            var url = await AddImageToFileStore(article.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);

            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            await _fileRepository.UpdateFile(command.Image, cancellationToken);
            command.Result.File = command.Image;
            command.Result.File.Id = article.ImageId.Value;
        }
        else
        {
            command.Image.Id = default;
            var url = await AddImageToFileStore(article.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
            command.Image.FilePath = url;
            command.Image.IsPublic = true;
            command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
            command.Result.HasAddedNew = true;

            await _articleRepository.UpdateArticleImage(command.LibraryId, command.ArticleId, command.Result.File.Id, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<string> AddImageToFileStore(long articleId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
    {
        var filePath = GetUniqueFileName(articleId, fileName);
        return await _fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
    }

    private static string GetUniqueFileName(long articleId, string fileName)
    {
        var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
        return $"articles/{articleId}/title.{fileNameWithourExtension}";
    }
}
