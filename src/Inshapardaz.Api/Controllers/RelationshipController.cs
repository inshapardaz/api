using System;
using Paramore.Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Controllers
{
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

        [HttpGet("/api/words/{id}/relationships", Name = "GetWordRelationsById")]
        public async Task<IActionResult> GetRelationshipForWord(int id)
        {
            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByWordIdQuery { WordId = id });
            return Ok(relations.Select(r => _relationRender.Render(r)).ToList());
        }

        [HttpGet("/api/relationships/{id}", Name = "GetRelationById")]
        public async Task<IActionResult> Get(int id)
        {
            if (_userHelper.GetUserId() != Guid.Empty)
            {
                var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = id });
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

        [HttpPost("/api/words/{id}/relationships", Name = "AddRelation")]
        public async Task<IActionResult> Post(int id, [FromBody]RelationshipView relationship)
        {
            if (relationship == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sourceWord = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = id });
            if (sourceWord == null)
            {
                return NotFound();
            }

            var relatedWord = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = relationship.RelatedWordId });
            if (relatedWord == null)
            {
                return NotFound();
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = id });
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var dictionary2 = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = relationship.RelatedWordId });
            if (dictionary2 == null || dictionary2.Id != dictionary.Id)
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

            var newRelationship = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = command.RelationId });
            var responseView = _relationRender.Render(newRelationship);
            return Created(responseView.Links.Single(x => x.Rel == "self").Href, responseView);
        }

        [HttpPut("/api/relationships/{id}", Name = "UpdateRelation")]
        public async Task<IActionResult> Put(int id, [FromBody]RelationshipView relationship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = relationship.SourceWordId });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            var dictonary2 = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = relationship.RelatedWordId });
            if (dictonary2 == null || dictonary2.Id != dictonary.Id)
            {
                return BadRequest();
            }

            await _commandProcessor.SendAsync(new UpdateWordRelationCommand { Relation = relationship.Map<RelationshipView, WordRelation>() });

            return NoContent();
        }

        [HttpDelete("/api/relationships/{id}", Name = "DeleteRelation")]
        public async Task<IActionResult> Delete(int id)
        {
            var relations = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = id });

            if (relations == null)
            {
                return NotFound();
            }

            var dictonary = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = (int)relations.SourceWordId });
            if (dictonary == null || dictonary.UserId != _userHelper.GetUserId())
            {
                return Unauthorized();
            }

            await _commandProcessor.SendAsync(new DeleteWordRelationCommand { RelationId = id });

            return NoContent();
        }
    }
}