using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.Tools;
using Inshapardaz.Domain.Ports.Command.Tools.CommonWords;
using Inshapardaz.Domain.Ports.Query.Tools.CommonWords;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class CommonWordController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderCommonWord _commonWordRenderer;

    public CommonWordController(IAmACommandProcessor commandProcessor, 
        IQueryProcessor queryProcessor, 
        IRenderCommonWord commonWordRenderer)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _commonWordRenderer = commonWordRenderer;
    }

    [HttpGet("/tools/{language}/words/list", Name = nameof(GetAllCommonWords))]
    public async Task<IActionResult> GetAllCommonWords(string language, CancellationToken cancellationToken)
    {
        var query = new GetAllWordsForLanguageQuery(language);
        var result = await _queryProcessor.ExecuteAsync(query, cancellationToken: cancellationToken);
        if (result is null || !result.Any())
        {
            return NotFound();
        }
        
        return Ok(result);
    }
    
    [HttpGet("/tools/{language}/words", Name = nameof(GetCommonWords))]
    public async Task<IActionResult> GetCommonWords(string language, 
        string query, 
        int pageNumber = 1, 
        int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var wordsQuery = new GetCommonWordsQuery(language)
        {
            Query = query,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _queryProcessor.ExecuteAsync(wordsQuery, cancellationToken: cancellationToken);
        
        var args = new PageRendererArgs<CommonWordModel>
        {
            Page = result,
            RouteArguments = new PagedRouteArgs
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Query = query
            },
        };
        
        return Ok(_commonWordRenderer.Render(args, language));
    }
    
    [HttpGet("/tools/{language}/words/{id:long}", Name = nameof(GetCommonWordById))]
    public async Task<IActionResult> GetCommonWordById(string language, long id, CancellationToken cancellationToken)
    {
        var query = new GetCommonWordByIdQuery(language, id);
        var result = await _queryProcessor.ExecuteAsync(query, cancellationToken: cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        var response = _commonWordRenderer.Render(result, language);
        return Ok(response);
    }
    
    [HttpPost("/tools/{language}/words", Name = nameof(AddCommonWord))]
    public async Task<IActionResult> AddCommonWord(string language, [FromBody] CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        commonWordModel.Language = language;
        var query = new AddCommonWordRequest(commonWordModel);
        await _commandProcessor.SendAsync(query, cancellationToken: cancellationToken);
        var response = _commonWordRenderer.Render(query.Result, language);
        return Created(response.Links.Self(), response);
    }
    
    [HttpPut("/tools/{language}/words/{id:long}", Name = nameof(UpdateCommonWord))]
    public async Task<IActionResult> UpdateCommonWord(long id, string language, [FromBody] CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        commonWordModel.Id = id;
        commonWordModel.Language = language;
        var command = new UpdateCommonWordRequest(commonWordModel);
        await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
        var response = _commonWordRenderer.Render(command.Result.WordModel, language);
        if (command.Result.HasAddedNew)
        {
            return Created(response.Links.Self(), response);
        }
        
        return Ok(response);
    }
    
    [HttpDelete("/tools/{language}/words/{id:long}", Name = nameof(DeleteCommonWord))]
    public async Task<IActionResult> DeleteCommonWord(long id, string language, CancellationToken cancellationToken)
    {
        var query = new DeleteCommonWordRequest(id, language);
        await _commandProcessor.SendAsync(query, cancellationToken: cancellationToken);
        return NoContent();
    }
}
