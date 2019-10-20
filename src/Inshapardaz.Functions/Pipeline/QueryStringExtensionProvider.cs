using Microsoft.Azure.WebJobs.Host.Config;

namespace Inshapardaz.Functions.Pipeline
{
    public class QueryStringExtensionProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var provider = new QueryStringBindingProvider();
            var rule = context.AddBindingRule<QueryStringAttribute>().Bind(provider);
        }
    }
}
