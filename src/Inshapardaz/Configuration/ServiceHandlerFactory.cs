using System;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Api.Configuration
{
    internal class ServiceHandlerFactory : IAmAHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHandleRequests Create(Type handlerType)
        {
            return _serviceProvider.GetService(handlerType) as IHandleRequests;
        }

        public void Release(IHandleRequests handler)
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
