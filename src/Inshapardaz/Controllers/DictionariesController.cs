using System;
using System.Collections.Generic;

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
        public IActionResult Get()
        {
            string userId = _userHelper.GetUserId();

            var results = _queryProcessor.Execute(new GetDictionariesByUserQuery { UserId = userId });
            return Ok(_dictionariesRenderer.Render(results));
        }

        [HttpGet("/api/dictionaries/{id}", Name = "GetDictionaryById")]
        public IActionResult Get(int id)
        {
            string userId = _userHelper.GetUserId();
            var result = _queryProcessor.Execute(new GetDictionaryByIdQuery{ UserId = userId, DictionaryId = id });

            if (result == null)
            {
                return NotFound();
            }

            return Ok(_dictionaryRenderer.Render(result));
        }

        [Authorize]
        [HttpPost("/api/dictionaries", Name = "CreateDictioanry")]
        public IActionResult Post([FromBody]DictionaryView value)
        {
            string userId = _userHelper.GetUserId();

            AddDictionaryCommand addDictionaryCommand = new AddDictionaryCommand
            {
                Dictionary = value.Map<DictionaryView, Dictionary>()
            };

            addDictionaryCommand.Dictionary.UserId = userId;

            _commandProcessor.Send(addDictionaryCommand);

            var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary);
            return Created(response.Links.Single(x => x.Rel == "self").Href, response);
        }

        [Authorize]
        [HttpPut("/api/dictionaries/{id}", Name = "UpdateDictionary")]
        public IActionResult Put(int id, [FromBody]DictionaryView value)
        {
            string userId = _userHelper.GetUserId();

            var result = _queryProcessor.Execute(new GetDictionaryByIdQuery { UserId = userId, DictionaryId = id });

            if (result == null)
            {
                AddDictionaryCommand addDictionaryCommand = new AddDictionaryCommand
                {
                    Dictionary = value.Map<DictionaryView, Dictionary>()
                };

                addDictionaryCommand.Dictionary.UserId = userId;

                _commandProcessor.Send(addDictionaryCommand);

                var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary);
                return Created(response.Links.Single(x => x.Rel == "self").Href, response);
            }
            else
            {
                UpdateDictionaryCommand updateDictionaryCommand = new UpdateDictionaryCommand
                {
                    Dictionary = value.Map<DictionaryView, Dictionary>()
                };

                updateDictionaryCommand.Dictionary.UserId = userId;

                _commandProcessor.Send(updateDictionaryCommand);

                return Ok();
            }
        }

        [Authorize]
        [HttpDelete("/api/dictionaries/{id}", Name = "DeleteDictionary")]
        public IActionResult Delete(int id)
        {
            string userId = _userHelper.GetUserId();
            var result = _queryProcessor.Execute(new GetDictionaryByIdQuery { UserId = userId, DictionaryId = id });

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
