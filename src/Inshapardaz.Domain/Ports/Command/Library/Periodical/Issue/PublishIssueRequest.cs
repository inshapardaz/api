using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class PublishIssueRequest : LibraryBaseCommand
{
    public PublishIssueRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber) : base(libraryId)
    {
        PeriodicalId = periodicalId;
        VolumeNumber = volumeNumber;
        IssueNumber = issueNumber;
    }

    public int IssueNumber { get; set; }

    public int VolumeNumber { get; set; }

    public int PeriodicalId { get; set; }

    public string Result { get; set; }

}

public class PublishBookRequestHandler : RequestHandlerAsync<PublishIssueRequest>
{
    private readonly IPeriodicalRepository _periodicalRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IIssuePageRepository _issuePageRepository;
    private readonly IIssueArticleRepository _issueArticleRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IFileRepository _fileRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public PublishBookRequestHandler(IPeriodicalRepository periodicalRepository,
        IIssueRepository issueRepository,
        IIssuePageRepository issuePageRepository,
        IIssueArticleRepository issueArticleRepository,
        IFileStorage fileStorage, 
        IFileRepository fileRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _issueRepository = issueRepository;
        _issuePageRepository = issuePageRepository;
        _issueArticleRepository = issueArticleRepository;
        _fileStorage = fileStorage;
        _fileRepository = fileRepository;
        _commandProcessor = commandProcessor;
        _periodicalRepository = periodicalRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin)]
    public override async Task<PublishIssueRequest> HandleAsync(PublishIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
        var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, cancellationToken);
        var articles = await _issueArticleRepository.GetIssueArticlesByIssue(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, cancellationToken);
        var articleText = new List<string>();
        foreach (var article in articles)
        {
            var pages = await _issuePageRepository.GetPagesByIssueArticle(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, article.Id, cancellationToken);
            var finalText = await CombinePages(pages, cancellationToken);
            articleText.Add(finalText);
            if (article.Contents.Any(cc => cc.Language == periodical.Language))
            {
                var cmd = new UpdateIssueArticleContentRequest(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, article.SequenceNumber, finalText, periodical.Language);               
                await _commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
            }
            else
            {
                var cmd = new AddIssueArticleContentRequest(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, article.SequenceNumber, finalText, periodical.Language);
                await _commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
            }
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private async Task<FileModel> SaveFileToStorage(BookModel book, byte[] wordDocument, CancellationToken cancellationToken)
    {
        var fileName = $"{book.Title.ToSafeFilename()}.docx";
        var url = await _fileStorage.StoreFile($"books/{book.Id}/{fileName}", wordDocument, MimeTypes.MsWord, cancellationToken);
        var file = await _fileRepository.AddFile(new FileModel
        {
            FilePath = url,
            MimeType = MimeTypes.MsWord,
            FileName = fileName,
            IsPublic = false
        }, cancellationToken);
        return file;
    }

    private async Task UpdateFileInStorage(BookModel book, long fileId, byte[] file, CancellationToken cancellationToken)
    {
        var fileName = $"{book.Title.ToSafeFilename()}.docx";
        var existingDocx = await _fileRepository.GetFileById(fileId, cancellationToken);
        if (existingDocx != null && !string.IsNullOrWhiteSpace(existingDocx.FilePath))
        {
            await _fileStorage.DeleteFile(existingDocx.FilePath, cancellationToken);
        }

        existingDocx.FilePath = await _fileStorage.StoreFile($"books/{book.Id}/{fileName}", file, MimeTypes.MsWord, cancellationToken);

        await _fileRepository.UpdateFile(existingDocx, cancellationToken);
    }

    private char[] pageBreakSymbols = new char[] { '۔', ':', '“', '"', '\'', '!' };

    private async Task<string> CombinePages(IEnumerable<IssuePageModel> pages, CancellationToken cancellationToken)
    {
        StringBuilder builder = new StringBuilder();

        var tasks = pages.Select(GetPageText).ToArray();
        
        Task.WaitAll(tasks); 
        
        foreach (var task in tasks)
        {
            var (separator, finalText) = task.Result;

            builder.Append(separator);
            builder.Append(finalText);
        }

        return builder.ToString().TrimStart();

        async Task<(string separator, string finalText)> GetPageText(IssuePageModel page)
        {
            var separator = " ";
            if (page.FileId.HasValue)
            {
                var file = await _fileRepository.GetFileById(page.FileId.Value, cancellationToken);
                if (file != null)
                {
                    page.Text = await _fileStorage.GetTextFile(file.FilePath, cancellationToken);
                }
            }
            var finalText = page.Text.Trim();
            if (string.IsNullOrWhiteSpace(finalText))
            {
                return (separator, finalText);
            }

            var lastCharacter = finalText.Last();

            if (pageBreakSymbols.Contains(lastCharacter))
            {
                separator = Environment.NewLine;
            }

            return (separator, finalText);
        }
    }
}
