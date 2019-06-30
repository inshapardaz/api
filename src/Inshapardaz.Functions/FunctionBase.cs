using System;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace Inshapardaz.Functions
{
    public abstract class FunctionBase
    {
        protected readonly IAmACommandProcessor CommandProcessor;

        public FunctionBase(IAmACommandProcessor commandProcessor)
        {
            CommandProcessor = commandProcessor;    
        }
        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> Authenticate(HttpRequestMessage req, ILogger log) 
        {
            return await req.AuthenticateAsync(log);
        }

        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)> AuthenticateAsWriter(HttpRequestMessage req, ILogger log) 
        {
            var result =  await req.AuthenticateAsync(log);
            result.User.IsWriter();
            return result;
        }

        protected async Task<(ClaimsPrincipal User, SecurityToken ValidatedToken)?> TryAuthenticate(HttpRequestMessage req, ILogger log) 
        {
            try
            {
                return await req.AuthenticateAsync(log);
            }
            catch(AuthenticationExpectedException)
            {
                return null;
            }
        }

        protected async Task<T> ReadBody<T>(HttpRequestMessage request)
        {
            try
            {
                var body = await request.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch
            {
                throw new ExpectedException(System.Net.HttpStatusCode.BadRequest);
            }
        }

        protected static LinkView SelfLink(string href, string relType = RelTypes.Self, string method = "GET") => new LinkView { 
            Method = method.ToUpper(), 
            Rel = relType, 
            Href = href.StartsWith("http") ? href : new Uri(ConfigurationSettings.ApiRoot, $"api/{href}").ToString()
        };
    }
}