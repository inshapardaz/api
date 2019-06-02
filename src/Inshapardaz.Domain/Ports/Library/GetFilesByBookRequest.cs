using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetFilesByBookRequest : BookRequest
    {
        public GetFilesByBookRequest(int bookId)
            : base(bookId)
        {
        }

        public IEnumerable<File> Result { get; set; }

        public Guid UserId { get; set; }
    }

    public class GetFilesByBookRequestHandler : RequestHandlerAsync<GetFilesByBookRequest>
    {
        private readonly IBookRepository _bookRepository;

        public GetFilesByBookRequestHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<GetFilesByBookRequest> HandleAsync(GetFilesByBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var files = await _bookRepository.GetFilesByBook(command.BookId, cancellationToken);

            command.Result = files ?? Enumerable.Empty<File>();
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

