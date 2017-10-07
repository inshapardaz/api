using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class DictionariesController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView> _dictionariesRenderer;
        private readonly IRenderResponseFromObject<Dictionary, DictionaryView> _dictionaryRenderer;
        private readonly IRenderResponseFromObject<DownloadJobModel, DownloadDictionaryView> _dictionaryDownloadRenderer;

        private readonly ILogger _logger;

        public DictionariesController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView> dictionariesRenderer,
            IRenderResponseFromObject<Dictionary, DictionaryView> dictionaryRenderer,
            IRenderResponseFromObject<DownloadJobModel, DownloadDictionaryView> dictionaryDownloadRenderer, 
            ILogger<DictionariesController> logger, IActionContextAccessor a)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _dictionariesRenderer = dictionariesRenderer;
            _dictionaryRenderer = dictionaryRenderer;
            _dictionaryDownloadRenderer = dictionaryDownloadRenderer;
            _logger = logger;
        }

        [HttpGet("/api/dictionaries", Name = "GetDictionaries")]
        [Produces(typeof(DictionariesView))]
        public async Task<IActionResult> Get()
        {
            var userId = _userHelper.GetUserId();

            var results = await _queryProcessor.ExecuteAsync(new DictionariesByUserQuery { UserId = userId });
            return Ok(_dictionariesRenderer.Render(results));
        }

        [HttpGet("/api/dictionaries/{id}", Name = "GetDictionaryById")]
        [Produces(typeof(DictionaryView))]
        public async Task<IActionResult> Get(int id)
        {
            var userId = _userHelper.GetUserId();
            var result =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (result == null)
            {
                _logger.LogDebug("Unable to find dictionary with id {0}", id);
                return NotFound();
            }

            return Ok(_dictionaryRenderer.Render(result));
        }

        [Authorize]
        [HttpPost("/api/dictionaries", Name = "CreateDictionary")]
        [Produces(typeof(DictionaryView))]
        public async Task<IActionResult> Post([FromBody]DictionaryView value)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                _logger.LogDebug("Unable to create dictionary. Model not valid. {0}", string.Join("\n", allErrors.First().ErrorMessage));

                return BadRequest(ModelState);
            }

            var userId = _userHelper.GetUserId();

            AddDictionaryCommand addDictionaryCommand = new AddDictionaryCommand
            {
                Dictionary = value.Map<DictionaryView, Dictionary>()
            };

            addDictionaryCommand.Dictionary.UserId = userId;

            await _commandProcessor.SendAsync(addDictionaryCommand);

            var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary);
            return Created(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [Authorize]
        [HttpPut("/api/dictionaries/{id}", Name = "UpdateDictionary")]
        public async Task<IActionResult> Put(int id, [FromBody] DictionaryView value)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                _logger.LogDebug("Unable to create dictionary. Model not valid. {0}", string.Join("\n", allErrors.First().ErrorMessage));

                return BadRequest(ModelState);
            }

            var userId = _userHelper.GetUserId();

            var result =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (result == null)
            {
                _logger.LogDebug("Existing dictionary not found. Creating new with name '{0}'", value.Name);
                AddDictionaryCommand addDictionaryCommand = new AddDictionaryCommand
                {
                    Dictionary = value.Map<DictionaryView, Dictionary>()
                };

                addDictionaryCommand.Dictionary.UserId = userId;

                await _commandProcessor.SendAsync(addDictionaryCommand);

                var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary);
                return Created(response.Links.Single(x => x.Rel == "self").Href, response);
            }

            _logger.LogDebug("Updating existing dictionary with id '{0}'", value.Id);

            UpdateDictionaryCommand updateDictionaryCommand = new UpdateDictionaryCommand
            {
                Dictionary = value.Map<DictionaryView, Dictionary>()
            };

            updateDictionaryCommand.Dictionary.UserId = userId;

            await _commandProcessor.SendAsync(updateDictionaryCommand);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/dictionaries/{id}", Name = "DeleteDictionary")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userHelper.GetUserId();
            var result =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (result == null)
            {
                _logger.LogDebug("Dictionary with id '{0}' not found. Nothing deleted", id);
                return NotFound();
            }

            _logger.LogDebug("Deleting dictionary with id '{0}'", id);

            await _commandProcessor.SendAsync(new DeleteDictionaryCommand
            {
                UserId = userId,
                DictionaryId = id
            });

            return NoContent();
        }

        [Authorize]
        [HttpPost("/api/dictionaries/{id}/download", Name = "CreateDictionaryDownload")]
        [Produces(typeof(DictionaryView))]
        public async Task<IActionResult> CreateDownloadForDictionary(int id)
        {
            var userId = _userHelper.GetUserId();
            var dictionary =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (dictionary == null)
            {
                _logger.LogDebug("Dictionary with id '{0}' not found. Download cannot be created", id);

                return NotFound();
            }

            var addDictionaryDownloadCommand = new AddDictionaryDownloadCommand
            {
                DitionarayId = id,
                DownloadType = MimeTypes.SqlLite
            };

            await _commandProcessor.SendAsync(addDictionaryDownloadCommand);

            var result = _dictionaryDownloadRenderer.Render(new DownloadJobModel
            {
                Id = id,
                Type = MimeTypes.SqlLite,
                JobId = addDictionaryDownloadCommand.JobId
            });
            return Created(result.Links.Single(x => x.Rel == "self").Href, result);
        }

        [HttpGet("/api/dictionary/{id}/download", Name = "DownloadDictionary")]
        public async Task<IActionResult> DownloadDictionary(int id, [FromHeader(Name = "Accept")] string accept = MimeTypes.SqlLite)
        {
            var file = await _queryProcessor.ExecuteAsync(new GetDownloadByDictionaryIdQuery
            {
                DictionaryId =  id,
                UserId = _userHelper.GetUserId(),
                MimeType = accept
            });

            if (file != null)
            {
                return File(file.Contents, file.MimeType, file.FileName);
            }

            _logger.LogDebug("Dictionary with id '{0}' not found. Download cannot be found", id);

            return NotFound();
        }
    }
}