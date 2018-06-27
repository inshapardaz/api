using System.Collections.Generic;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class RelationshipController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderRelation _relationRender;

        public RelationshipController(IAmACommandProcessor commandProcessor, IRenderRelation relationRender)
        {
            _commandProcessor = commandProcessor;
            _relationRender = relationRender;
        }

        [HttpGet("/api/dictionaries/{id}/words/{wordId}/relationships", Name = "GetWordRelationsById")]
        [Produces(typeof(IEnumerable<RelationshipView>))]
        public async Task<IActionResult> GetRelationshipsForWord(int id, int wordId)
        {
            var request = new GetRelationshipsForWordRequest(id)
            {
                WordId = wordId
            };
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("/api/dictionaries/{id}/words/{wordId}/relationships/{relationId}", Name = "GetRelationById")]
        [Produces(typeof(RelationshipView))]
        public async Task<IActionResult> Get(int id, long wordId, int relationId)
        {
            var request = new GetRelationshipRequest(id, wordId, relationId);
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpPost("/api/dictionaries/{id}/words/{wordId}/relationships", Name = "AddRelation")]
        [Produces(typeof(RelationshipView))]
        [ValidateModel]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]RelationshipView relationship)
        {
            var request = new AddRelationshipRequest(id, wordId, relationship.Map<RelationshipView, WordRelation>());
            await _commandProcessor.SendAsync(request);
            var response = _relationRender.Render(request.Result, id);
            return Created(response.Links.Self(), response);
        }

        [HttpPut("/api/dictionaries/{id}/words/{wordId}/relationships/{relationId}", Name = "UpdateRelation")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, long wordId, int relationId, [FromBody]RelationshipView relationship)
        {
            var request = new UpdateRelationshipRequest(id, wordId, relationship.Map<RelationshipView, WordRelation>());
            await _commandProcessor.SendAsync(request);
            if (request.Result.HasAddedNew)
            {
                var response = _relationRender.Render(request.Result.Relationship, id);
                return Created(response.Links.Self(), response);
            }

            return NoContent();
        }

        [HttpDelete("/api/dictionaries/{id}/words/{wordId}/relationships/{relationId}", Name = "DeleteRelation")]
        public async Task<IActionResult> Delete(int id, long wordId, int relationId)
        {
            var request = new DeleteRelationshipRequest(id, wordId, relationId);
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}