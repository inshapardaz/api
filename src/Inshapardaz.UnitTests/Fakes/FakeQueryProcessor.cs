using Darker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Inshapardaz.UnitTests.Fakes
{
    public class FakeQueryProcessor : IQueryProcessor
    {
        private readonly IDictionary<Type, IDictionary<Predicate<IQueryRequest>, IQueryResponse>> _results;
        private readonly IDictionary<Type, IDictionary<Predicate<IQueryRequest>, Exception>> _exceptions;

        public IList<IQueryRequest> ExecutedQueries { get; }

        public FakeQueryProcessor()
        {
            _results = new Dictionary<Type, IDictionary<Predicate<IQueryRequest>, IQueryResponse>>();
            _exceptions = new Dictionary<Type, IDictionary<Predicate<IQueryRequest>, Exception>>();

            ExecutedQueries = new List<IQueryRequest>();
        }

        public TResponse Execute<TResponse>(IQueryRequest<TResponse> request) where TResponse : IQueryResponse
        {
            ExecutedQueries.Add(request);

            var queryType = request.GetType();
            if (_exceptions.ContainsKey(queryType))
            {
                var exception = _exceptions[queryType].Where(r => r.Key(request)).Select(r => r.Value).FirstOrDefault();
                if (exception != null)
                    throw exception;
            }

            if (!_results.ContainsKey(queryType))
                return default(TResponse);

            var result = _results[queryType].Where(r => r.Key(request)).Select(r => r.Value).FirstOrDefault();
            if (result == null)
                return default(TResponse);

            return (TResponse)result;
        }

        public Task<TResponse> ExecuteAsync<TResponse>(IQueryRequest<TResponse> request) where TResponse : IQueryResponse
        {
            return Task.FromResult(Execute(request));
        }

        public IEnumerable<T> GetExecutedQueries<T>()
        {
            return ExecutedQueries.Where(q => q is T).Cast<T>();
        }

        public void SetupResultFor<TRequest>(Predicate<TRequest> predicate, IQueryResponse result)
            where TRequest : IQueryRequest
        {
            var queryType = typeof(TRequest);
            var resultType = typeof(IQueryRequest<>).MakeGenericType(result.GetType());
            if (!resultType.IsAssignableFrom(queryType))
                throw new InvalidOperationException("Request and response types don't match");

            if (!_results.ContainsKey(queryType))
                _results.Add(queryType, new Dictionary<Predicate<IQueryRequest>, IQueryResponse>());

            Predicate<IQueryRequest> untypedPredicate = r => predicate((TRequest)r);
            _results[queryType].Add(untypedPredicate, result);
        }

        public void SetupResultFor<TRequest>(IQueryResponse result)
            where TRequest : IQueryRequest
        {
            var queryType = typeof(TRequest);
            var resultType = typeof(IQueryRequest<>).MakeGenericType(result.GetType());
            if (!resultType.IsAssignableFrom(queryType))
                throw new InvalidOperationException("Request and response types don't match");

            if (!_results.ContainsKey(queryType))
                _results.Add(queryType, new Dictionary<Predicate<IQueryRequest>, IQueryResponse>());

            _results[queryType].Add(_ => true, result);
        }

        public void SetupExceptionFor<TRequest>(Predicate<TRequest> predicate, Exception exception)
            where TRequest : IQueryRequest
        {
            var queryType = typeof(TRequest);
            if (!_exceptions.ContainsKey(queryType))
                _exceptions.Add(queryType, new Dictionary<Predicate<IQueryRequest>, Exception>());

            Predicate<IQueryRequest> untypedPredicate = r => predicate((TRequest)r);
            _exceptions[queryType].Add(untypedPredicate, exception);
        }

        public void SetupExceptionFor<TRequest>(Exception exception)
            where TRequest : IQueryRequest
        {
            var queryType = typeof(TRequest);
            if (!_exceptions.ContainsKey(queryType))
                _exceptions.Add(queryType, new Dictionary<Predicate<IQueryRequest>, Exception>());

            _exceptions[queryType].Add(_ => true, exception);
        }

        public void ShouldHaveExecuted<T>() where T : IQueryRequest
        {
            Assert.True(ExecutedQueries.Any(q => q is T));
        }

        public void ShouldHaveExecuted<T>(Predicate<T> predicate) where T : IQueryRequest
        {
            Assert.True(ExecutedQueries.Any(q => q is T && predicate((T)q)));
        }
    }
}
