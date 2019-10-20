using System;
using Microsoft.Azure.WebJobs.Description;

namespace Inshapardaz.Functions.Authentication
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class AccessTokenAttribute : Attribute
    {
    }
}