using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Inshapardaz.Functions.Authentication
{

    public class AccessTokenBinding : IBinding
    {
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            // Get the HTTP request
            var request = context.BindingData["req"] as DefaultHttpRequest;

            // Get the configuration files for the OAuth token issuer
            var audience = Environment.GetEnvironmentVariable("Authentication.Audience");
            var issuer = Environment.GetEnvironmentVariable("Authentication.Authority");

            return Task.FromResult<IValueProvider>(new AccessTokenValueProvider(request, audience, issuer));
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) => null;

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }
}