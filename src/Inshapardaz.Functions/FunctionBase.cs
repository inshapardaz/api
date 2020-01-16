using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Functions
{
    public abstract class FunctionBase
    {
        protected async Task<string> ReadBody(HttpRequest request)
        {
            try
            {
                using (var reader = new StreamReader(request.Body, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch
            {
                throw new ExpectedException(System.Net.HttpStatusCode.BadRequest);
            }
        }

        protected async Task<T> ReadBody<T>(HttpRequest request)
        {
            try
            {
                var body = await ReadBody(request);
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch
            {
                throw new ExpectedException(System.Net.HttpStatusCode.BadRequest);
            }
        }

        protected static LinkView SelfLink(string href, string relType = RelTypes.Self, string method = "GET", Dictionary<string, string> queryString = null, string type = null)
        {
            var urlBuilder = new UriBuilder(ConfigurationSettings.ApiRoot)
            {
                Path = $"api/{href}"
            };
            
            if (queryString != null && queryString.Any())
            {
                var collection = HttpUtility.ParseQueryString(string.Empty);
                foreach (var item in queryString)
                {
                    collection[item.Key] = item.Value;
                }

                urlBuilder.Query = collection.ToString();
            }
            return new LinkView
            {
                Method = method.ToUpper(),
                Rel = relType,
                Href = href.StartsWith("http") ? href : urlBuilder.Uri.ToString(),
                Type = type
            };
        }

        protected int GetQueryParameter(HttpRequest request, string fieldName, int defaultValue = 0)
        {
            if (request != null)
            {
                var queryParams = request.GetQueryParameterDictionary();

                if (queryParams.TryGetValue(fieldName, out var value))
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        return intValue;
                    }
                }
            }

            return defaultValue;
        }

        protected T GetQueryParameter<T>(HttpRequest request, string fieldName, T defaultValue = default(T))
        {
            if (request != null)
            {
                var queryParams = request.GetQueryParameterDictionary();

                if (queryParams.TryGetValue(fieldName, out var value))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }

            return defaultValue;
        }

        protected string GetHeader(HttpRequest request, string headerName, string defaultValue)
        {
            if (request != null && request.Headers.ContainsKey(headerName))
            {
                return request.Headers["headerName"];
            }

            return defaultValue;
        }
    }

    public abstract class CommandBase : FunctionBase
    {
        protected readonly IAmACommandProcessor CommandProcessor;
        
        protected CommandBase(IAmACommandProcessor commandProcessor)
        {
            CommandProcessor = commandProcessor;
        }
    }

    public abstract class QueryBase : FunctionBase
    {
        protected readonly IQueryProcessor QueryProcessor;
        
        protected QueryBase(IQueryProcessor queryProcessor)
        {
            QueryProcessor = queryProcessor;
        }
    }
}
