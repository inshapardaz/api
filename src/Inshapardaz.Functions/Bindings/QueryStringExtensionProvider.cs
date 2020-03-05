using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace Inshapardaz.Functions.Bindings
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class QueryStringAttribute : Attribute
    {
        // [AutoResolve]
        public string Key { get; set; }
    }

    public class QueryStringExtensionProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<QueryStringAttribute>();
            rule.BindToInput<string>(a => a.Key);
        }
    }

    public class QueryStringBindingProvider : IBindingProvider
    {
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            IBinding binding = new QueryStringBinding();
            return Task.FromResult(binding);
        }
    }

    public class QueryStringBinding : IBinding
    {
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            var queryString = context.BindingData["Query"] as Dictionary<string, string>;

            return Task.FromResult<IValueProvider>(new QueryStringValueProvider(queryString));
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) => null;

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }

    public class QueryStringValueProvider : IValueProvider
    {
        private readonly Dictionary<string, string> _queryString;

        public QueryStringValueProvider(Dictionary<string, string> queryString)
        {
            _queryString = queryString;
        }

        public Task<object> GetValueAsync()
        {
            return null;
        }

        public Type Type => typeof(string);

        public string ToInvokeString() => string.Empty;
    }
}
