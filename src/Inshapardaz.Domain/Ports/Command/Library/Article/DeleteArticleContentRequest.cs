﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Article;

public class DeleteArticleContentRequest : LibraryBaseCommand
{
    public DeleteArticleContentRequest(int libraryId, int articleId, string language)
        : base(libraryId)
    {
        ArticleId = articleId;
        Language = language;
    }

    public int ArticleId { get; }
    public string Language { get; }
}

public class DeleteArticleContentRequestHandler : RequestHandlerAsync<DeleteArticleContentRequest>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteArticleContentRequestHandler(IArticleRepository articleRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _articleRepository = articleRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteArticleContentRequest> HandleAsync(DeleteArticleContentRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var content = await _articleRepository.GetArticleContent(command.LibraryId, command.ArticleId, command.Language, cancellationToken);

        if (content != null)
        {
            await _commandProcessor.SendAsync(new DeleteTextFileCommand(content.FileId), cancellationToken: cancellationToken);
            await _articleRepository.DeleteArticleContent(command.LibraryId, command.ArticleId, command.Language, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
