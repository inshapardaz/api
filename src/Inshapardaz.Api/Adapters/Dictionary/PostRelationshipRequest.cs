using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostRelationshipRequest : IRequest
    {
        public Guid Id { get; set; }

        public RelationshipView Relationship { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public int DictionaryId { get; set; }

        public long WordId { get; set; }

        public class RequestResult
        {
            public RelationshipView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostRelationshipRequestHandler : RequestHandlerAsync<PostRelationshipRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderRelation _relationRender;

        private readonly IUserHelper _userHelper;

        public PostRelationshipRequestHandler(IAmACommandProcessor commandProcessor, 
            IQueryProcessor queryProcessor,
            IRenderRelation relationRender, 
            IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _relationRender = relationRender;

            _userHelper = userHelper;
        }

        public override async Task<PostRelationshipRequest> HandleAsync(PostRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = command.DictionaryId }, cancellationToken);
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                throw new UnauthorizedAccessException();
            }

            var sourceWord = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = command.WordId }, cancellationToken);
            if (sourceWord == null)
            {
                throw new NotFoundException();
            }

            var relatedWord = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = command.Relationship.RelatedWordId }, cancellationToken);
            if (relatedWord == null)
            {
                throw new NotFoundException();
            }

            var dictionary2 = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery { WordId = command.Relationship.RelatedWordId }, cancellationToken);
            if (dictionary2 == null || dictionary2.Id != dictionary.Id)
            {
                throw new BadRequestException();
            }

            var addCommand = new AddWordRelationCommand
            {
                SourceWordId = command.WordId,
                RelatedWordId = command.Relationship.RelatedWordId,
                RelationType = (RelationType)command.Relationship.RelationTypeId
            };
            await _commandProcessor.SendAsync(addCommand, cancellationToken: cancellationToken);

            var newRelationship = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = addCommand.RelationId }, cancellationToken);
            var response = _relationRender.Render(newRelationship);
            command.Result.Location =  response.Links.Single(x => x.Rel == "self").Href;
            command.Result.Response =  response;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
