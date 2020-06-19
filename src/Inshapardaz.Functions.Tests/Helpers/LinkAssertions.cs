using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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

        public static LinkView ShouldGet(this LinkView link, string url)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("get"), $"Link with reltype '{link.Rel}' should have method 'GET' but found '{link.Method}'");
            Uri uri = new Uri(link.Href);
            Assert.That(uri.AbsolutePath.ToLower(), Is.EqualTo(url.ToLower()), $"Link with reltype '{link.Rel}' not matching.");

            return link;
        }

        public static LinkView ShouldGetMatching(this LinkView link, string regEx)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("get"), $"Link with reltype '{link.Rel}' should have method 'GET' but found '{link.Method}'");
            Uri uri = new Uri(link.Href);
            uri.AbsolutePath.Should().MatchRegex(regEx, $"Link with reltype '{link.Rel}' not matching pattern `{regEx}`.");

            return link;
        }

        public static LinkView EndingWith(this LinkView link, string endingWith)
        {
            Uri uri = new Uri(link.Href);
            uri.AbsolutePath.Should().EndWith(endingWith, $"Link {uri.AbsoluteUri} reltype '{link.Rel}' not ending with `{endingWith}`.");

            return link;
        }

        public static LinkView ShouldPut(this LinkView link, string url)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("put"), $"Link with reltype '{link.Rel}' should have method 'PUT' but found '{link.Method}'");
            Uri uri = new Uri(link.Href);
            Assert.That(uri.AbsolutePath.ToLower(), Is.EqualTo(url.ToLower()), $"Link with reltype '{link.Rel}' not matching.");

            return link;
        }

        public static LinkView ShouldPost(this LinkView link, string url)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("post"), $"Link with reltype '{link.Rel}' should have method 'POST' but found '{link.Method}'");
            Uri uri = new Uri(link.Href);
            Assert.That(uri.AbsolutePath.ToLower(), Is.EqualTo(url.ToLower()), $"Link with reltype '{link.Rel}' not matching.");

            return link;
        }

        public static LinkView ShouldDelete(this LinkView link, string url)
        {
            Assert.That(link.Method.ToLower(), Is.EqualTo("delete"), $"Link with reltype '{link.Rel}' should have method 'DELETE' but found '{link.Method}'");
            Uri uri = new Uri(link.Href);
            Assert.That(uri.AbsolutePath.ToLower(), Is.EqualTo(url.ToLower()), $"Link with reltype '{link.Rel}' not matching.");

            return link;
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

        public static LinkView ShouldHaveAcceptLanguage(this LinkView link, string expected)
        {
            Assert.That(link.AcceptLanguage, Is.EqualTo(expected), $"Link with reltype '{link.Rel}' and method '{link.Method}' has invalid accepted language");
            return link;
        }

        public static LinkView ShouldHaveAccept(this LinkView link, string expected)
        {
            Assert.That(link.Accept, Is.EqualTo(expected), $"Link with reltype '{link.Rel}' and method '{link.Method}' has invalid accept");
            return link;
        }

        public static LinkView SelfLink(this ViewWithLinks view)
        {
            return view.Links.SingleOrDefault(l => l.Rel.Equals(RelTypes.Self, StringComparison.CurrentCultureIgnoreCase));
        }

        public static LinkView CreateLink(this ViewWithLinks view)
        {
            return view.Links.SingleOrDefault(l => l.Rel.Equals(RelTypes.Create, StringComparison.CurrentCultureIgnoreCase));
        }

        public static LinkView UpdateLink(this ViewWithLinks view)
        {
            return view.Links.SingleOrDefault(l => l.Rel.Equals(RelTypes.Update, StringComparison.CurrentCultureIgnoreCase));
        }

        public static LinkView DeleteLink(this ViewWithLinks view)
        {
            return view.Links.SingleOrDefault(l => l.Rel.Equals(RelTypes.Delete, StringComparison.CurrentCultureIgnoreCase));
        }

        public static LinkView CreateWordLink(this ViewWithLinks view)
        {
            return view.Links.SingleOrDefault(l => l.Rel.Equals(RelTypes.CreateWord, StringComparison.CurrentCultureIgnoreCase));
        }

        public static LinkView Link(this ViewWithLinks view, string relType)
        {
            return view.Links.SingleOrDefault(l => l.Rel.Equals(relType, StringComparison.CurrentCultureIgnoreCase));
        }

        public static IEnumerable<LinkView> Links(this ViewWithLinks view, string relType)
        {
            return view.Links.Where(l => l.Rel.Equals(relType, StringComparison.CurrentCultureIgnoreCase));
        }

        public static LinkView ShouldHaveUrl(this LinkView view, string path)
        {
            var uri = new Uri(view.Href);
            uri.AbsolutePath.Should().Be(path);
            return view;
        }

        public static LinkView ShouldHaveQueryParameter<T>(this LinkView view, string name, T value)
        {
            var uri = new Uri(view.Href);
            var parameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var param = parameters[name];
            param.Should().NotBeNull($"Query parameter '{name}' not found.");
            param.Should().Be(value.ToString(), $"Query parameter '{name}' has unexpected value.");
            return view;
        }

        public static void ShouldHaveUpdateLink(this ViewWithLinks view, string url)
        {
            view.UpdateLink()
                .ShouldBePut()
                .EndingWith(url);
        }

        public static void ShouldHaveDeleteLink(this ViewWithLinks view, string url)
        {
            view.DeleteLink()
                .ShouldBeDelete()
                .EndingWith(url);
        }
    }
}
