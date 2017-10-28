using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DictionaryRequestValidationHandler<T> 
        : RequestHandlerAsync<T> where T : DictionaryRequest
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;

        public DictionaryRequestValidationHandler(IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
        }

        public override async Task<T> HandleAsync(T command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery
            {
                DictionaryId = command.DictionaryId
            }, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            if (!dictionary.IsPublic && dictionary.UserId != _userHelper.GetUserId())
            {
                throw new UnauthorizedException();
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }

    public class DictionaryRequestValidationAttribute : RequestHandlerAttribute
    {
        public DictionaryRequestValidationAttribute(int step, HandlerTiming timing)
            : base(step, timing)
        { }

        public override object[] InitializerParams()
        {
            return new object[] { Timing };
        }

        public override Type GetHandlerType()
        {
            return typeof(DictionaryRequestValidationHandler<>);
        }
    }
}