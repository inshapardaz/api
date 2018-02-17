using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.Middlewares;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Controllers
{
    public class RelationshipController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public RelationshipController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
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
            var request = new PostRelationshipRequest(id)
            {
                WordId = wordId,
                Relationship = relationship
            };
            await _commandProcessor.SendAsync(request);
            return Created(request.Result.Location, request.Result.Response);
        }

        [HttpPut("/api/dictionaries/{id}/words/{wordId}/relationships/{relationId}", Name = "UpdateRelation")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, long wordId, int relationId, [FromBody]RelationshipView relationship)
        {
            var request = new PutRelationshipRequest(id, wordId, relationId)
            {
                Relationship = relationship
            };
            await _commandProcessor.SendAsync(request);
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