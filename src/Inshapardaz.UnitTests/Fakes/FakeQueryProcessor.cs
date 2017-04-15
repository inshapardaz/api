using Darker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using System.Threading;

namespace Inshapardaz.UnitTests.Fakes
{
    public class FakeQueryProcessor : IQueryProcessor
    {
        private readonly IDictionary<Type, IDictionary<Predicate<IQuery>, object>> _results;
        private readonly IDictionary<Type, IDictionary<Predicate<IQuery>, Exception>> _exceptions;

        public IList<IQuery> ExecutedQueries { get; }

        public FakeQueryProcessor()
        {
            _results = new Dictionary<Type, IDictionary<Predicate<IQuery>, object>>();
            _exceptions = new Dictionary<Type, IDictionary<Predicate<IQuery>, Exception>>();

            ExecutedQueries = new List<IQuery>();
        }

        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            ExecutedQueries.Add(query);

            var queryType = query.GetType();
            if (_exceptions.ContainsKey(queryType))
            {
                var exception = _exceptions[queryType].Where(r => r.Key(query)).Select(r => r.Value).FirstOrDefault();
                if (exception != null)
                    throw exception;
            }

            if (!_results.ContainsKey(queryType))
                return default(TResult);

            var result = _results[queryType].Where(r => r.Key(query)).Select(r => r.Value).FirstOrDefault();
            if (result == null)
                return default(TResult);

            return (TResult)result;
        }

        public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Execute(query));
        }

        public IEnumerable<T> GetExecutedQueries<T>()
        {
            return ExecutedQueries.Where(q => q is T).Cast<T>();
        }

        public void SetupResultFor<TRequest, TResponse>(Predicate<TRequest> predicate, TResponse result)
            where TRequest : IQuery
        {
            var queryType = typeof(TRequest);
            var resultType = typeof(IQuery<>).MakeGenericType(result.GetType());
            if (!resultType.IsAssignableFrom(queryType))
                throw new InvalidOperationException("Request and response types don't match");

            if (!_results.ContainsKey(queryType))
                _results.Add(queryType, new Dictionary<Predicate<IQuery>, object>());

            Predicate<IQuery> untypedPredicate = r => predicate((TRequest)r);
            _results[queryType].Add(untypedPredicate, result);
        }

        public void SetupResultFor<TRequest, TResponse>(TResponse result)
            where TRequest : IQuery
        {
            var queryType = typeof(TRequest);
            var resultType = typeof(IQuery<>).MakeGenericType(result.GetType());
            if (!resultType.IsAssignableFrom(queryType))
                throw new InvalidOperationException("Request and response types don't match");

            if (!_results.ContainsKey(queryType))
                _results.Add(queryType, new Dictionary<Predicate<IQuery>, object>());

            _results[queryType].Add(_ => true, result);
        }

        public void SetupExceptionFor<TRequest>(Predicate<TRequest> predicate, Exception exception)
            where TRequest : IQuery
        {
            var queryType = typeof(TRequest);
            if (!_exceptions.ContainsKey(queryType))
                _exceptions.Add(queryType, new Dictionary<Predicate<IQuery>, Exception>());

            Predicate<IQuery> untypedPredicate = r => predicate((TRequest)r);
            _exceptions[queryType].Add(untypedPredicate, exception);
        }

        public void SetupExceptionFor<TRequest>(Exception exception)
            where TRequest : IQuery
        {
            var queryType = typeof(TRequest);
            if (!_exceptions.ContainsKey(queryType))
                _exceptions.Add(queryType, new Dictionary<Predicate<IQuery>, Exception>());

            _exceptions[queryType].Add(_ => true, exception);
        }

        public void ShouldHaveExecuted<T>() where T : IQuery
        {
            Assert.True(ExecutedQueries.Any(q => q is T));
        }

        public void ShouldHaveExecuted<T>(Predicate<T> predicate) where T : IQuery
        {
            Assert.True(ExecutedQueries.Any(q => q is T && predicate((T)q)));
        }
    }
}
