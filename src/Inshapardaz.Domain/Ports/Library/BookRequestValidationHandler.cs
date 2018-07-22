using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class BookRequestValidationHandler<T> 
        : RequestHandlerAsync<T> where T : BookRequest
    {
        private readonly IBookRepository _bookRepository;

        private readonly IUserHelper _userHelper;

        public BookRequestValidationHandler(IBookRepository bookRepository, IUserHelper userHelper)
        {
            _bookRepository = bookRepository;
            _userHelper = userHelper;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.BookId, cancellationToken);

            if (book == null)
            {
                throw new NotFoundException();
            }

            /*var userId = _userHelper.GetUserId();
            if (!book.IsPublic && book.UserId != userId)
            {
                throw new UnauthorizedException();
            }*/
            
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