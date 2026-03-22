using System.Text;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue.Article;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical.Issue;

public class PublishIssueRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber)
    : LibraryBaseCommand(libraryId)
{
    public int IssueNumber { get; set; } = issueNumber;

    public int VolumeNumber { get; set; } = volumeNumber;

    public int PeriodicalId { get; set; } = periodicalId;

    public string Result { get; set; }

}

public class PublishIssueRequestHandler(
    IPeriodicalRepository periodicalRepository,
    IIssueRepository issueRepository,
    IIssuePageRepository issuePageRepository,
    IIssueArticleRepository issueArticleRepository,
    IFileStorage fileStorage,
    IFileRepository fileRepository,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<PublishIssueRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin)]
    public override async Task<PublishIssueRequest> HandleAsync(PublishIssueRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var periodical = await periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
        var issue = await issueRepository.GetIssue(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, cancellationToken);
        var articles = await issueArticleRepository.GetIssueArticlesByIssue(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, cancellationToken);
        var articleText = new List<string>();
        foreach (var article in articles)
        {
            var pages = await issuePageRepository.GetPagesByIssueArticle(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, article.Id, cancellationToken);
            var finalText = await CombinePages(pages, cancellationToken);
            articleText.Add(finalText);
            if (article.Contents.Any(cc => cc.Language == periodical.Language))
            {
                var cmd = new UpdateIssueArticleContentRequest(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, article.SequenceNumber, finalText, periodical.Language);               
                await commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
            }
            else
            {
                var cmd = new AddIssueArticleContentRequest(command.LibraryId, command.PeriodicalId,command.VolumeNumber, command.IssueNumber, article.SequenceNumber, finalText, periodical.Language);
                await commandProcessor.SendAsync(cmd, cancellationToken: cancellationToken);
            }
        }

        return await base.HandleAsync(command, cancellationToken);
    }

    private char[] pageBreakSymbols = new char[] { '۔', ':', '“', '"', '\'', '!' };

    private async Task<string> CombinePages(IEnumerable<IssuePageModel> pages, CancellationToken cancellationToken)
    {
        StringBuilder builder = new StringBuilder();

        var tasks = pages.Select(GetPageText).ToArray();
        
        await Task.WhenAll(tasks); 
        
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
                var file = await fileRepository.GetFileById(page.FileId.Value, cancellationToken);
                if (file != null)
                {
                    page.Text = await fileStorage.GetTextFile(file.FilePath, cancellationToken);
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
