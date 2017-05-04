using Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using paramore.brighter.commandprocessor;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
    [Route("api/[controller]")]
    public class RelationshipController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderResponseFromObject<WordRelation, RelationshipView> _relationRender;
        private readonly IUserHelper _userHelper;

        public RelationshipController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IUserHelper userHelper,
            IRenderResponseFromObject<WordRelation, RelationshipView> relationRender)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _relationRender = relationRender;
        }

        [Route("/api/word/{id}/relationships", Name = "GetWordRelationsById")]
        public async Task<IActionResult> GetRelationshipForWord(int id)
        {
            if (!string.IsNullOrWhiteSpace(_userHelper.GetUserId()))
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = id });
                if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
                {
                    return Unauthorized();
                }
            }

            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByWordIdQuery { WordId = id });
            return Ok(relations.Select(r => _relationRender.Render(r)).ToList());
        }

        [HttpGet("/api/relationships/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!string.IsNullOrWhiteSpace(_userHelper.GetUserId()))
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = id });
                if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
                {
                    return Unauthorized();
                }
            }

            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = id });

            if (relations == null)
            {
                return NotFound();
            }

            return Ok(_relationRender.Render(relations));
        }

        [HttpPost("/api/word/{id}/Relation", Name = "AddRelation")]
        public async Task<IActionResult> Post(int id, [FromBody]RelationshipView relationship)
        {
            if (relationship == null)
            {
                return BadRequest();
            }

            var sourceWord = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = id });
            if (sourceWord == null)
            {
                return NotFound();
            }

            var relatedWord = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = relationship.RelatedWordId });
            if (relatedWord == null)
            {
                return NotFound();
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = id });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var dictonary2 = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = relationship.RelatedWordId });
            if (dictonary2 == null || dictonary2.Id != dictonary.Id)
            {
                return BadRequest();
            }

            var command = new AddWordRelationCommand
            {
                SourceWordId = id,
                RelatedWordId = relationship.RelatedWordId,
                RelationType = (RelationType)relationship.RelationTypeId
            };
            await _commandProcessor.SendAsync(command);

            var newRelationship = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = id });
            var responseView = _relationRender.Render(newRelationship);
            return Created(responseView.Links.Single(x => x.Rel == "self").Href, responseView);
        }

        [HttpPut("/api/relationship/{id}", Name = "UpdateRelation")]
        public async Task<IActionResult> Put(int id, [FromBody]RelationshipView relationship)
        {
            if (relationship == null)
            {
                return BadRequest();
            }

            var relation1 = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = id });

            if (relation1 == null)
            {
                return NotFound();
            }

            var relation2 = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = relationship.SourceWordId });

            if (relation2 == null)
            {
                return BadRequest();
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = relationship.SourceWordId });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var dictonary2 = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = relationship.RelatedWordId });
            if (dictonary2 == null || dictonary2.Id != dictonary.Id)
            {
                return BadRequest();
            }

            await _commandProcessor.SendAsync(new UpdateWordRelationCommand { Relation = relationship.Map<RelationshipView, WordRelation>() });

            return NoContent();
        }

        [HttpDelete("/api/relationship/{id}", Name = "DeleteRelation")]
        public async Task<IActionResult> Delete(int id)
        {
            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = id });

            if (relations == null)
            {
                return NotFound();
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new GetDictionaryByWordIdQuery { WordId = (int)relations.SourceWordId });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            await _commandProcessor.SendAsync(new DeleteWordRelationCommand { RelationId = id });

            return NoContent();
        }
    }
}