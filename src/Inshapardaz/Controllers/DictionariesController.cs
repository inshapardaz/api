﻿using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Inshapardaz.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Renderers;

using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using paramore.brighter.commandprocessor;
using Inshapardaz.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Controllers
{
    public class DictionariesController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView> _dictionariesRenderer;
        private readonly IRenderResponseFromObject<Dictionary, DictionaryView> _dictionaryRenderer;

        public DictionariesController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView> dictionariesRenderer,
            IRenderResponseFromObject<Dictionary, DictionaryView> dictionaryRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _dictionariesRenderer = dictionariesRenderer;
            _dictionaryRenderer = dictionaryRenderer;
        }

        [HttpGet("/api/dictionaries", Name = "GetDictionaries")]
        public async Task<IActionResult> Get()
        {
            string userId = _userHelper.GetUserId();

            var results = await _queryProcessor.ExecuteAsync(new GetDictionariesByUserQuery { UserId = userId });
            return Ok(_dictionariesRenderer.Render(results));
        }

        [HttpGet("/api/dictionaries/{id}", Name = "GetDictionaryById")]
        public async Task<IActionResult> Get(int id)
        {
            string userId = _userHelper.GetUserId();
            var result = await _queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery{ UserId = userId, DictionaryId = id });

            if (result == null)
            {
                return NotFound();
            }

            return Ok(_dictionaryRenderer.Render(result));
        }

        [Authorize]
        [HttpPost("/api/dictionaries", Name = "CreateDictioanry")]
        public async Task<IActionResult> Post([FromBody]DictionaryView value)
        {
            if (string.IsNullOrWhiteSpace(value.Name))
            {
                return BadRequest("Name is not valid");
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
        public async Task<IActionResult> Put(int id, [FromBody]DictionaryView value)
        {
            if (string.IsNullOrWhiteSpace(value.Name))
            {
                return BadRequest("Name is not valid");
            }
            
            string userId = _userHelper.GetUserId();

            var result = await _queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery { UserId = userId, DictionaryId = id });

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
            var result = await _queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery { UserId = userId, DictionaryId = id });

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
    }
}