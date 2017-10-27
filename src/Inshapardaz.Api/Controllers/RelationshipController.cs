using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Inshapardaz.Api.Ports;
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
        public async Task<IActionResult> GetRelationshipForWord(int id, int wordId)
        {
            var request = new GetRelationshipsForWordRequest
            {
                DictionaryId = id,
                WordId = wordId
            };
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpGet("/api/dictionaries/{id}/relationships/{relationId}", Name = "GetRelationById")]
        public async Task<IActionResult> Get(int id, int relationId)
        {
            var request = new GetRelationshipRequest
            {
                DictionaryId = id,
                RelationId = relationId
            };
            await _commandProcessor.SendAsync(request);
            return Ok(request.Result);
        }

        [HttpPost("/api/dictionaries/{id}/words/{wordId}/relationships", Name = "AddRelation")]
        public async Task<IActionResult> Post(int id, int wordId, [FromBody]RelationshipView relationship)
        {
            var request = new PostRelationshipRequest
            {
                DictionaryId = id,
                WordId = wordId,
                Relationship = relationship
            };
            await _commandProcessor.SendAsync(request);
            return Created(request.Result.Location, request.Result.Response);
        }

        [HttpPut("/api/dictionaries/{id}/relationships/{relationId}", Name = "UpdateRelation")]
        public async Task<IActionResult> Put(int id, int relationId, [FromBody]RelationshipView relationship)
        {
            var request = new PutRelationshipRequest
            {
                DictionaryId = id,
                RelationshipId = relationId,
                Relationship = relationship
            };
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }

        [HttpDelete("/api/dictionaries/{id}/relationships/{relationId}", Name = "DeleteRelation")]
        public async Task<IActionResult> Delete(int id, int relationId)
        {
            var request = new DeleteRelationshipRequest
            {
                DictionaryId = id,
                RelationshipId = relationId
            };
            await _commandProcessor.SendAsync(request);
            return NoContent();
        }
    }
}