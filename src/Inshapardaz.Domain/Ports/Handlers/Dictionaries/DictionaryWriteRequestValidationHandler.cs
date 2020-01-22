using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class DictionaryWriteRequestValidationHandler<T> 
        : RequestHandlerAsync<T> where T : DictionaryRequest
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public DictionaryWriteRequestValidationHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _dictionaryRepository.GetDictionaryById(command.DictionaryId, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            if (dictionary.UserId != command.UserId)
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
