using Microsoft.Azure.WebJobs.Description;
using System;

namespace Inshapardaz.Functions.Authentication
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class AccessTokenAttribute : Attribute
    {
    }
}
