using System;
using Microsoft.Azure.WebJobs;

namespace Inshapardaz.Functions.Pipeline
{
    public static class QueryStringExtension
    {
        public static IWebJobsBuilder AddQueryStringBinding(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<QueryStringExtensionProvider>();
            return builder;
        }
    }   
}
