using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Inshapardaz.Functions.Pipeline
{
    public class QueryStringValueProvider : IValueProvider
    {
        private readonly HttpRequest _request;

        public QueryStringValueProvider(HttpRequest request)
        {
            _request = request;
        }

        public Type Type => throw new NotImplementedException();

        public async Task<object> GetValueAsync()
        {
            _request.Query.TryGetValue("pageNumber", out var pageNumberValues);
            if (pageNumberValues.Count > 0)
            {
                var pageNumber = pageNumberValues[0];
                if (int.TryParse(pageNumber, out int value))
                {
                    return value;
                }
            }

            return 0;
        }

        public string ToInvokeString() => string.Empty;
    }
}
