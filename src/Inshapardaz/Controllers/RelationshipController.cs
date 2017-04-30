using System.Collections.Generic;
using Darker;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Api.Controllers
{
    [Route("api/[controller]")]
    public class RelationshipController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderResponseFromObject<IEnumerable<WordRelation>, RelationshipsView> _relationsRenderer;
        private readonly IRenderResponseFromObject<WordRelation, RelationshipView> _relationRender;

        public RelationshipController(IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IRenderResponseFromObject<IEnumerable<WordRelation>, RelationshipsView> relationsRenderer,
            IRenderResponseFromObject<WordRelation, RelationshipView> relationRender)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _relationsRenderer = relationsRenderer;
            _relationRender = relationRender;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var relations = _queryProcessor.Execute(new RelationshipByIdQuery { Id = id });

            if (relations == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(_relationRender.Render(relations));
        }

        [Route("/api/word/{id}/Relationships", Name = "GetWordRelationsById")]
        public IActionResult GetRelationshipForWord(int id)
        {
            var relations = _queryProcessor.Execute(new RelationshipByWordIdQuery { WordId = id });
            return new ObjectResult(_relationsRenderer.Render(relations));
        }

        [HttpPost("/api/word/{id}/Relation", Name = "AddRelation")]
        public IActionResult Post(int id, [FromBody]RelationshipView relationship)
        {
            var word = _queryProcessor.Execute(new WordByIdQuery {Id = id});
            if (word == null)
            {
                return NotFound();
            }

            var relatedWord = _queryProcessor.Execute( new WordByIdQuery {Id = relationship.RelatedWordId});
            if (relatedWord == null)
            {
                return NotFound();
            }

            _commandProcessor.Send(new AddWordRelationCommand
            {
                SourceWordId = id,
                RelatedWordId = relationship.RelatedWordId,
                RelationType = (RelationType)relationship.RelationTypeId
            });

            return Ok();
        }

        [HttpPut("/api/word/{id}/Relation/{relatedWith}", Name = "UpdateRelation")]
        public IActionResult Put(int id, [FromBody]RelationshipView relationship)
        {
            if (relationship == null)
            {
                return BadRequest();
            }

            var relations = _queryProcessor.Execute(new RelationshipByIdQuery { Id = relationship.Id });

            if (relations == null)
            {
                return Ok();
            }

            _commandProcessor.Send(new UpdateWordRelationCommand { Relation = relationship.Map<RelationshipView, WordRelation>() });

            return Ok();
        }

        [HttpDelete("/api/word/{id}/Relation/{relatedWith}", Name = "DeleteRelation")]
        public IActionResult Delete(int id)
        {
            var relations = _queryProcessor.Execute(new RelationshipByIdQuery { Id = id });

            if (relations == null || relations.Id != id)
            {
                return NotFound();
            }

            _commandProcessor.Send(new DeleteWordRelationCommand { RelationId =  id });

            return Ok();
        }
    }
}
