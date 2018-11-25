﻿using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetAuthorByIdRequest : RequestBase
    {
        public GetAuthorByIdRequest(int authorId)
        {
            AuthorId = authorId;
        }

        public int AuthorId { get; }

        public Author Result { get; set; }
    }

    public class GetAuthorByIdRequestHandler : RequestHandlerAsync<GetAuthorByIdRequest>
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public GetAuthorByIdRequestHandler(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<GetAuthorByIdRequest> HandleAsync(GetAuthorByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _authorRepository.GetAuthorById(command.AuthorId, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException();
            }

            result.BookCount  = await _bookRepository.GetBookCountByAuthor(command.AuthorId, cancellationToken);

            command.Result = result;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
