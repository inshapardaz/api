﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorAsReader : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingAuthorAsReader() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();

            author.Name = RandomData.Name;

            _response = await Client.PutObject($"/libraries/{LibraryId}/authors/{author.Id}", author);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
