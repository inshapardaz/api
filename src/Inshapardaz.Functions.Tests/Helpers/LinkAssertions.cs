using System;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Functions.Views;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public static class LinkAssertions
    {
        public static LinkView AssertLink(this IEnumerable<LinkView> links, string relType, string method = "GET")
        {
            var link = links.SingleOrDefault(l => l.Rel.Equals(relType, StringComparison.CurrentCultureIgnoreCase));

            Assert.That(link, Is.Not.Null, $"Link with reltype '{relType}' doesn't exists");

            return link;
        }

        public static void AssertLinkNotPresent(this IEnumerable<LinkView> links, string relType, string method = "GET")
        {
            var link = links.SingleOrDefault(l => l.Rel.Equals(relType, StringComparison.CurrentCultureIgnoreCase));

            Assert.That(link, Is.Null, $"Link with reltype {relType} shouldn't exists");
        }

        public static LinkView ShouldBeGet(this LinkView link)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("get"), $"Link with reltype '{link.Rel}' should have method 'GET' but found '{link.Method}'");
            return link;
        }

        public static LinkView ShouldBePost(this LinkView link)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("post"), $"Link with reltype '{link.Rel}' should have method 'POST' but found '{link.Method}'");
            return link;
        }

        public static LinkView ShouldBePut(this LinkView link)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("put"), $"Link with reltype '{link.Rel}' should have method 'PUT' but found '{link.Method}'");
            return link;
        }
        
        public static LinkView ShouldBeDelete(this LinkView link)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("delete"), $"Link with reltype '${link.Rel}' should have method 'DELETE' but found '{link.Method}'");
            return link;
        }

        public static LinkView ShouldHaveHref(this LinkView link, string href)
        {
            Assert.That(link.Href.ToLower(), Is.EqualTo("delete"), $"Link with reltype '{link.Rel}' should have method 'DELETE' but found '{link.Method}'");
            return link;
        }

        public static LinkView ShouldHaveSomeHref(this LinkView link)
        {
            Assert.That(link.Href, Is.Not.Empty, $"Link with reltype '{link.Rel}' and method '{link.Method}' has no href");
            return link;
        }
    }
}