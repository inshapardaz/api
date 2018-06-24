using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class DictionaryWriteRequestValidationHandler<T> 
        : RequestHandlerAsync<T> where T : DictionaryRequest
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IUserHelper _userHelper;

        public DictionaryWriteRequestValidationHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
        {
            _dictionaryRepository = dictionaryRepository;
            _userHelper = userHelper;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _dictionaryRepository.GetDictionaryById(command.DictionaryId, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            var userId = _userHelper.GetUserId();
            if (dictionary.UserId != userId)
            {
                throw new UnauthorizedException();
            }
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }

    public class DictionaryWriteRequestValidationAttribute : RequestHandlerAttribute
    {
        public DictionaryWriteRequestValidationAttribute(int step, HandlerTiming timing)
            : base(step, timing)
        { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(DictionaryWriteRequestValidationHandler<>);
        }
    }
}