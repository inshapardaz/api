using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateArticleContentRequest : LibraryBaseCommand
    {
        public UpdateArticleContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string contents, string language, string mimetype)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            SequenceNumber = sequenceNumber;
            Contents = contents;
            MimeType = mimetype;
            Language = language;
        }

        public string MimeType { get; set; }
        public string Language { get; set; }
        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public int SequenceNumber { get; }
        public string Contents { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public ArticleContentModel Content { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateArticleContentRequestHandler : RequestHandlerAsync<UpdateArticleContentRequest>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IFileRepository _fileRepository;
        private readonly ILibraryRepository _libraryRepository;

        public UpdateArticleContentRequestHandler(IArticleRepository articleRepository, IIssueRepository issueRepository, IFileStorage fileStorage,
                                                  IFileRepository fileRepository, ILibraryRepository libraryRepository)
        {
            _articleRepository = articleRepository;
            _issueRepository = issueRepository;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
            _libraryRepository = libraryRepository;
        }

        public override async Task<UpdateArticleContentRequest> HandleAsync(UpdateArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
            var article = await _articleRepository.GetArticle(command.LibraryId, command.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, command.SequenceNumber, cancellationToken);

            if (issue == null || article == null)
            {
                throw new BadRequestException();
            }

            if (string.IsNullOrWhiteSpace(command.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Language = library.Language;
            }

            var contentUrl = await _articleRepository.GetArticleContentUrl(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Language, command.MimeType, cancellationToken);

            if (contentUrl == null)
            {
                var name = GenerateArticleContentUrl(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Language, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(name, command.Contents, cancellationToken);

                var fileModel = new Models.FileModel { MimeType = command.MimeType, FilePath = actualUrl, IsPublic = issue.IsPublic, FileName = name };
                var file = await _fileRepository.AddFile(fileModel, cancellationToken);
                var articleContent = new ArticleContentModel
                {
                    PeriodicalId = command.PeriodicalId,
                    IssueId = command.IssueNumber,
                    SequenceNumber = command.SequenceNumber,
                    Language = command.Language,
                    MimeType = command.MimeType,
                    FileId = file.Id
                };

                command.Result.Content = await _articleRepository.AddArticleContent(command.LibraryId,
                                                                                           articleContent,
                                                                                           cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                string url = contentUrl ?? GenerateArticleContentUrl(command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, command.Language, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(url, command.Contents, cancellationToken);

                await _issueRepository.UpdateIssueContent(command.LibraryId,
                                                        command.PeriodicalId,
                                                        command.VolumeNumber,
                                                        command.IssueNumber,
                                                        command.SequenceNumber,
                                                        command.Language,
                                                        command.MimeType,
                                                        actualUrl,
                                                        cancellationToken);
                command.Result.Content = await _articleRepository.GetArticleContent(command.LibraryId,
                                                        command.PeriodicalId,
                                                        command.VolumeNumber,
                                                        command.IssueNumber,
                                                        command.SequenceNumber,
                                                        command.Language,
                                                        command.MimeType,
                                                        cancellationToken);
            }

            command.Result.Content.ContentUrl = await ImageHelper.TryConvertToPublicFile(command.Result.Content.FileId, _fileRepository, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }

        private string GenerateArticleContentUrl(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, string mimeType)
        {
            var extension = MimetypeToExtension(mimeType);
            return $"periodicals/{periodicalId}/volumes/{volumeNumber}/issues/{issueNumber}/{sequenceNumber}_{language}.{extension}";
        }

        private string MimetypeToExtension(string mimeType)
        {
            switch (mimeType.ToLower())
            {
                case "text/plain": return "txt";
                case "text/markdown": return "md";
                case "text/html": return "md";
                case "application/msword": return "doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return "doc";
                case "application/pdf": return "pdf";
                case "application/epub+zip": return "epub";
                default: throw new BadRequestException();
            }
        }
    }
}
