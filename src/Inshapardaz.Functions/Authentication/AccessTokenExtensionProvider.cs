using Microsoft.Azure.WebJobs.Host.Config;

namespace Inshapardaz.Functions.Authentication
{
    public class AccessTokenExtensionProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            // Creates a rule that links the attribute to the binding
            var provider = new AccessTokenBindingProvider();
            var rule = context.AddBindingRule<AccessTokenAttribute>().Bind(provider);
        }
    }
}