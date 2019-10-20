using Microsoft.Azure.WebJobs.Description;
using System;

namespace Inshapardaz.Functions.Pipeline
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class QueryStringAttribute : Attribute
    {
        public QueryStringAttribute(string name, int defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        public string Name { get; set; }

        public int DefaultValue { get; set; }
    }
}
