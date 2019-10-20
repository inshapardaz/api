using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class BookRequestValidationHandler<T> 
        : RequestHandlerAsync<T> where T : BookRequest
    {
        private readonly IBookRepository _bookRepository;

        public BookRequestValidationHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);

            if (book == null)
            {
                //if (!book.IsPublic && command.UserId == Guid.Empty)
                //{
                    return await Task.FromResult<T>(null);
                //}
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }

    public class BookRequestValidationAttribute : RequestHandlerAttribute
    {
        public BookRequestValidationAttribute(int step, HandlerTiming timing)
            : base(step, timing)
        { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(BookRequestValidationHandler<>);
        }
    }
}
