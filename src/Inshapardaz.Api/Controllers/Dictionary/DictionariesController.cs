using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using ObjectMapper = Inshapardaz.Domain.Helpers.ObjectMapper;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class DictionariesController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderDictionaries _dictionariesRenderer;
        private readonly IRenderDictionary _dictionaryRenderer;

        public DictionariesController(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderDictionaries dictionariesRenderer, IRenderDictionary dictionaryRenderer)
        {
            _commandProcessor = commandProcessor;
            _userHelper = userHelper;
            _dictionariesRenderer = dictionariesRenderer;
            _dictionaryRenderer = dictionaryRenderer;
        }

        [HttpGet("/api/dictionaries", Name = "GetDictionaries")]
        [Produces(typeof(DictionariesView))]
        public async Task<IActionResult> GetDictionaries()
        {
            var request = new GetDictionariesRequest(_userHelper.GetUserId());
            await _commandProcessor.SendAsync(request);
            
            return Ok(_dictionariesRenderer.Render(request.Result) );
        }

        [HttpGet("/api/dictionaries/{id}", Name = "GetDictionaryById")]
        [Produces(typeof(DictionaryView))]
        public async Task<IActionResult> GetDictionaryById(int id)
        {
            var request = new GetDictionaryByIdRequest(id);
            await _commandProcessor.SendAsync(request);

            return Ok(_dictionaryRenderer.Render(request.Result));
        }

        [Authorize]
        [HttpPost("/api/dictionaries", Name = "CreateDictionary")]
        [Produces(typeof(DictionaryView))]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]DictionaryView value)
        {
            var request = new AddDictionaryRequest(_userHelper.GetUserId(), ObjectMapper.Map<DictionaryView, Domain.Entities.Dictionary.Dictionary>(value));
            await _commandProcessor.SendAsync(request);

            var response = _dictionaryRenderer.Render(request.Result);
            return Created(response.Links.Self(), response);
        }

        [Authorize]
        [HttpPut("/api/dictionaries/{id}", Name = "UpdateDictionary")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] DictionaryView value)
        {
            var request = new UpdateDictionaryRequest(id, ObjectMapper.Map<DictionaryView, Domain.Entities.Dictionary.Dictionary>(value));
            await _commandProcessor.SendAsync(request);

            if (request.Result.HasAddedNew)
            {
                var response = _dictionaryRenderer.Render(request.Result.Dictionary);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/dictionaries/{id}", Name = "DeleteDictionary")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new DeleteDictionaryRequest(id);
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}