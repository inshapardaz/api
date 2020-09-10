using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddArticleContentRequest : LibraryAuthorisedCommand
    {
        public AddArticleContentRequest(ClaimsPrincipal claims, int libraryId, int periodicalId, int issueId, int articleId, string contents, string language, string mimeType, Guid? userId)
            : base(claims, libraryId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
            ArticleId = articleId;
            Contents = contents;
            MimeType = mimeType;
            Language = language;
        }

        public int PeriodicalId { get; }
        public int IssueId { get; }
        public int ArticleId { get; }
        public string Contents { get; }

        public string MimeType { get; set; }

        public string Language { get; set; }

        public ArticleContentModel Result { get; set; }
    }

    public class AddArticleContentRequestHandler : RequestHandlerAsync<AddArticleContentRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IFileStorage _fileStorage;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IFileRepository _fileRepository;

        public AddArticleContentRequestHandler(IIssueRepository issueRepository, IArticleRepository chapterRepository, IFileStorage fileStorage, ILibraryRepository libraryRepository, IFileRepository fileRepository)
        {
            _issueRepository = issueRepository;
            _articleRepository = chapterRepository;
            _fileStorage = fileStorage;
            _libraryRepository = libraryRepository;
            _fileRepository = fileRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddArticleContentRequest> HandleAsync(AddArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Language = library.Language;
            }

            var issue = await _issueRepository.GetIssueById(command.LibraryId, command.PeriodicalId, command.IssueId, cancellationToken);
            if (issue == null)
            {
                throw new BadRequestException();
            }

            var article = await _articleRepository.GetArticleById(command.LibraryId, command.PeriodicalId, command.IssueId, command.ArticleId, cancellationToken);
            if (article != null)
            {
                var name = GenerateChapterContentUrl(command.PeriodicalId, command.IssueId, command.ArticleId, command.Language, command.MimeType);
                var actualUrl = await _fileStorage.StoreTextFile(name, command.Contents, cancellationToken);

                var fileModel = new Models.FileModel { MimeType = command.MimeType, FilePath = actualUrl, IsPublic = issue.IsPublic, FileName = name };
                var file = await _fileRepository.AddFile(fileModel, cancellationToken);
                var issueContent = new ArticleContentModel
                {
                    PeriodicalId = command.PeriodicalId,
                    IssueId = command.IssueId,
                    ArticleId = command.ArticleId,
                    Language = command.Language,
                    MimeType = command.MimeType,
                    FileId = file.Id
                };

                command.Result = await _articleRepository.AddArticleContent(command.LibraryId, issueContent, cancellationToken);

                if (file.IsPublic)
                {
                    var url = await ImageHelper.TryConvertToPublicFile(file.Id, _fileRepository, cancellationToken);
                    command.Result.ContentUrl = url;
                }
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private string GenerateChapterContentUrl(int periodicalId, int issueId, int articleId, string language, string mimeType)
        {
            var extension = MimetypeToExtension(mimeType);
            return $"periodicals/{periodicalId}/issues/{issueId}/{articleId}_{language}.{extension}";
        }

        private string MimetypeToExtension(string mimeType)
        {
            switch (mimeType.ToLower())
            {
                case "text/plain": return "txt";
                case "text/markdown": return "md";
                case "text/html": return "md";
                case "application/msword": return "docx";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return "docx";
                case "application/pdf": return "pdf";
                case "application/epub+zip": return "epub";
                default: throw new BadRequestException();
            }
        }
    }
}
