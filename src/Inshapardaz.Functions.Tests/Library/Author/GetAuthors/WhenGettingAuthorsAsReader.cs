﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsAsReader : LibraryTest
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private PageView<AuthorView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _builder = Container.GetService<AuthorsDataBuilder>();
            _builder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            var handler = Container.GetService<Functions.Library.Authors.GetAuthors>();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

            _view = _response.Value as PageView<AuthorView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.Links.AssertLinkNotPresent("create");
        }

        [Test]
        public void ShouldHaveNoEditingLinkOnAuthor()
        {
            var actual = _view.Data.FirstOrDefault();
            actual.Links.AssertLinkNotPresent("update");
            actual.Links.AssertLinkNotPresent("delete");
        }
    }
}
