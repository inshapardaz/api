using Microsoft.Azure.WebJobs;
using System;

namespace Inshapardaz.Functions.Authentication
{
    public static class AccessTokenExtensions
    {
        public static IWebJobsBuilder AddAccessTokenBinding(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<AccessTokenExtensionProvider>();
            return builder;
        }
    }
}
