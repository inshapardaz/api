using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Darker;
using Hangfire;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Jobs;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Api.Controllers
{
    public class DictionariesController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView> _dictionariesRenderer;
        private readonly IRenderResponseFromObject<Dictionary, DictionaryView> _dictionaryRenderer;
        private readonly IBackgroundJobClient _backgroundJobClient;

        private readonly IRenderResponseFromObject<DictionaryDownload, DownloadDictionaryView>
            _dictionaryDownloadRenderer;

        public DictionariesController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView> dictionariesRenderer,
            IRenderResponseFromObject<Dictionary, DictionaryView> dictionaryRenderer,
            IBackgroundJobClient backgroundJobClient,
            IRenderResponseFromObject<DictionaryDownload, DownloadDictionaryView> dictionaryDownloadRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _dictionariesRenderer = dictionariesRenderer;
            _dictionaryRenderer = dictionaryRenderer;
            _backgroundJobClient = backgroundJobClient;
            _dictionaryDownloadRenderer = dictionaryDownloadRenderer;
        }

        [HttpGet("/api/dictionaries", Name = "GetDictionaries")]
        [Produces(typeof(DictionariesView))]
        public async Task<IActionResult> Get()
        {
            string userId = _userHelper.GetUserId();

            var results = await _queryProcessor.ExecuteAsync(new DictionariesByUserQuery { UserId = userId });
            return Ok(_dictionariesRenderer.Render(results));
        }

        [HttpGet("/api/dictionaries/{id}", Name = "GetDictionaryById")]
        [Produces(typeof(DictionaryView))]
        public async Task<IActionResult> Get(int id)
        {
            string userId = _userHelper.GetUserId();
            var result =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (result == null)
            {
                return NotFound();
            }

            return Ok(_dictionaryRenderer.Render(result));
        }

        [Authorize]
        [HttpPost("/api/dictionaries", Name = "CreateDictionary")]
        [Produces(typeof(DictionaryView))]
        public async Task<IActionResult> Post([FromBody] DictionaryView value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = _userHelper.GetUserId();

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
                return BadRequest(ModelState);
            }

            string userId = _userHelper.GetUserId();

            var result =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (result == null)
            {
                AddDictionaryCommand addDictionaryCommand = new AddDictionaryCommand
                {
                    Dictionary = value.Map<DictionaryView, Dictionary>()
                };

                addDictionaryCommand.Dictionary.UserId = userId;

                await _commandProcessor.SendAsync(addDictionaryCommand);

                var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary);
                return Created(response.Links.Single(x => x.Rel == "self").Href, response);
            }

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
            string userId = _userHelper.GetUserId();
            var result =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (result == null)
            {
                return NotFound();
            }

            _commandProcessor.Send(new DeleteDictionaryCommand
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
            string userId = _userHelper.GetUserId();
            var dictionary =
                await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (dictionary == null)
            {
                return NotFound();
            }

            var jobId = _backgroundJobClient.Enqueue<SqliteExport>(x => x.ExportDictionary(id));
            var view = new DictionaryDownload
            {
                DictionaryId = id,
                //Format = "dat",
                //JobId = jobId
            };
            var result = _dictionaryDownloadRenderer.Render(view);
            return Created(result.Links.Single(x => x.Rel == "self").Href, result);
        }

        [HttpGet("/api/dictionary/{id}.{format}", Name = "DownloadDictionary")]
        public IActionResult DownloadDictionary(int id, string format)
        {
            throw new NotImplementedException();
        }
    }
}