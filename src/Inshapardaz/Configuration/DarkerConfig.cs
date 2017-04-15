using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Darker;
using Darker.Decorators;

namespace Inshapardaz.Configuration
{
    public class DarkerConfig : IQueryHandlerFactory, IQueryHandlerDecoratorFactory
    {
        private readonly IServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;
        public IQueryHandlerRegistry HandlerRegistry { get; } = new QueryHandlerRegistry();

        public DarkerConfig(IServiceCollection services, IServiceProvider serviceProvider)
        {
            _services = services;
            _serviceProvider = serviceProvider;
        }

        public void Register<TRequest, TResponse, THandler>()
            where TRequest : IQuery<TResponse>
            where THandler : IQueryHandler<TRequest, TResponse>
        {
            HandlerRegistry.Register<TRequest, TResponse, THandler>();
            _services.AddTransient(typeof(THandler));
        }

        public void RegisterQueriesAndHandlersFromAssembly(Assembly assembly)
        {
            var subscribers =
                from t in assembly.GetExportedTypes()
                where t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface
                from i in t.GetInterfaces()
                where i.GetTypeInfo().IsGenericType && (i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                select new { Request = i.GetGenericArguments().First(), ResponseType = i.GetGenericArguments().ElementAt(1), Handler = t };

            foreach (var subscriber in subscribers)
            {
                HandlerRegistry.Register(subscriber.Request, subscriber.ResponseType, subscriber.Handler);
                _services.AddTransient(subscriber.Handler);
            }
        }

        public void RegisterDefaultDecorators()
        {
            //_services.AddTransient(typeof(QueryLoggingDecorator<,>));
            //_services.AddTransient(typeof(RetryableQueryDecorator<,>));
            _services.AddTransient(typeof(FallbackPolicyDecorator<,>));
        }

        T IQueryHandlerFactory.Create<T>(Type handlerType)
        {
            try
            {
                return _serviceProvider.GetService(handlerType) as T;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        T IQueryHandlerDecoratorFactory.Create<T>(Type decoratorType)
        {
            return (T) _serviceProvider.GetService(decoratorType);
        }
    }
}