using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class BookWriteRequestValidationHandler<T> 
        : RequestHandlerAsync<T> where T : BookRequest
    {
        private readonly IBookRepository _bookRepository;

        public BookWriteRequestValidationHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _bookRepository.GetBookById(command.BookId, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            /*var userId = _userHelper.GetUserId();
            if (dictionary.UserId != userId)
            {
                throw new UnauthorizedException();
            }*/
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }

    public class BookWriteRequestValidationAttribute : RequestHandlerAttribute
    {
        public BookWriteRequestValidationAttribute(int step, HandlerTiming timing)
            : base(step, timing)
        { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(BookWriteRequestValidationHandler<>);
        }
    }
}
