﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookAsReader : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingBookAsReader() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            var book = BookBuilder.WithLibrary(LibraryId).WithAuthor(author).Build();
            book.Title = RandomData.Name;

            var payload = new BookView
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Copyrights = book.Copyrights.ToDescription(),
                Language = book.Language,
                YearPublished = book.YearPublished,
                Status = book.Status.ToDescription(),
                IsPublic = book.IsPublic,
                Authors = new[] { new AuthorView { Id = author.Id, Name = author.Name } },
                SeriesId = book.SeriesId,
                IsPublished = book.IsPublished,
                Source = book.Source,
                Publisher = book.Publisher
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{book.Id}", payload);
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
