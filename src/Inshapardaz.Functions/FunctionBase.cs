using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace Inshapardaz.Functions
{
    public abstract class FunctionBase
    {
        protected readonly IAmACommandProcessor CommandProcessor;

        protected readonly IFunctionAppAuthenticator Authenticator;

        public FunctionBase(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator)
        {
            CommandProcessor = commandProcessor;
            Authenticator = authenticator;
        }
        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> Authenticate(HttpRequest req, ILogger log) 
        {
            return await Authenticator.AuthenticateAsync(req, log);
        }

        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsWriter(HttpRequest req, ILogger log) 
        {
            var result =  await Authenticator.AuthenticateAsync(req, log);
            result.User.IsWriter();
            return result;
        }

        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)?> TryAuthenticate(HttpRequest req, ILogger log) 
        {
            try
            {
                return await Authenticator.AuthenticateAsync(req, log);
            }
            catch(AuthenticationExpectedException)
            {
                return null;
            }
        }

        protected async Task<T> ReadBody<T>(HttpRequest request)
        {
            try
            {
                using (var reader = new StreamReader(request.Body, Encoding.UTF8))
                {
                    var body = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<T>(body);
                }
            }
            catch
            {
                throw new ExpectedException(System.Net.HttpStatusCode.BadRequest);
            }
        }

        protected static LinkView SelfLink(string href, string relType = RelTypes.Self, string method = "GET", Dictionary<string, string> queryString = null)
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
                Href = href.StartsWith("http") ? href : urlBuilder.Uri.ToString()
            };
        }

        protected int GetQueryParameter(HttpRequest request, string fieldName, int defaultValue = 0)
        {
            var queryParams = request.GetQueryParameterDictionary();

            if (queryParams.TryGetValue(fieldName, out var value))
            {
                if (int.TryParse(value, out var intValue))
                {
                    return intValue;
                }
            }

            return defaultValue;
        }
    }
}