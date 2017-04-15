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
        public DictionariesView Get()
        {
            string userId = _userHelper.GetUserId();

            var results = _queryProcessor.Execute(new GetDictionariesByUserQuery { UserId = userId });
            return _dictionariesRenderer.Render(results.Dictionaries);
        }

        [HttpGet("/api/dictionaries/{id}", Name = "GetDictionaryById")]
        public DictionaryView Get(int id)
        {
            string userId = _userHelper.GetUserId();
            var result = _queryProcessor.Execute(new GetDictionaryByIdQuery{ UserId = userId, DictionaryId = id });
            return _dictionaryRenderer.Render(result.Dictionary);
        }

        [HttpPost("/api/dictionaries", Name = "CreateDictioanry")]
        public void Post([FromBody]DictionaryView value)
        {
            string userId = _userHelper.GetUserId();

            _commandProcessor.Send(new AddDictionaryCommand
            {
                UserId = userId,
                Dictionary = value.Map<DictionaryView, Dictionary>()
            });
        }

        [HttpPut("/api/dictionaries/{id}", Name = "UpdateDictionary")]
        public void Put(int id, [FromBody]DictionaryView value)
        {
            string userId = _userHelper.GetUserId();

            _commandProcessor.Send(new UpdateDictionaryCommand
            {
                UserId = userId,
                Dictionary = value.Map<DictionaryView, Dictionary>()
            });
        }

        [HttpDelete("/api/dictionaries/{id}", Name = "DeleteDictionary")]
        public void Delete(int id)
        {
            string userId = _userHelper.GetUserId();

            _commandProcessor.Send(new DeleteDictionaryCommand
            {
                UserId = userId,
                DictionaryId = id
            });
        }
    }
}
