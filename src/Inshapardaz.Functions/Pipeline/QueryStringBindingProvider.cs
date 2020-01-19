using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Inshapardaz.Functions.Pipeline
{
    public class QueryStringBindingProvider : IBindingProvider
    {
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            IBinding binding = new QueryStringBinding();
            return Task.FromResult(binding);
        }
    }
}
