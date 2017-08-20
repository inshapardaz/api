using System;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Api.Configuration
{
    internal class ServiceHandlerFactory : IAmAHandlerFactoryAsync
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHandleRequestsAsync Create(Type handlerType)
        {
            return _serviceProvider.GetService(handlerType) as IHandleRequestsAsync;
        }

        public void Release(IHandleRequestsAsync handler)
        {
            var disposable = handler as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            handler = null;
        }

    }
}
