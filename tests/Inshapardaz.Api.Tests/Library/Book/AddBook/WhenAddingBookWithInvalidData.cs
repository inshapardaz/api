﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.AddBook
{
    [TestFixture]
    public partial class WhenAddingBookWithInvalidData
    {
        [TestFixture]
        public class AndUsingNonExistingLibrary : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();
                var book = new BookView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = author.Id } } };

                _response = await Client.PostObject($"/libraries/{-RandomData.Number}/books", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadRequestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingAuthorFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private LibraryDataBuilder _library2Builder;

            public AndUsingAuthorFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var author = AuthorBuilder.WithLibrary(library2.Id).Build();

                var book = new BookView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = author.Id } } };

                _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                _library2Builder.CleanUp();
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingNonExistingAuthor : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingAuthor() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var book = new BookView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = -RandomData.Number } } };

                _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingNonExistingSeries : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingSeries() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();
                var book = new BookView { Title = new Faker().Random.String(), Authors = new List<AuthorView> { new AuthorView { Id = author.Id } }, SeriesId = -RandomData.Number };

                _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingSeriesFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private LibraryDataBuilder _library2Builder;

            public AndUsingSeriesFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var series = SeriesBuilder.WithLibrary(library2.Id).Build();
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();

                var book = new BookView { Title = new Faker().Random.String(), Authors = new List<AuthorView> { new AuthorView { Id = author.Id } }, SeriesId = series.Id };

                _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                _library2Builder.CleanUp();
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingNonExistingCategory : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingCategory() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();
                var book = new BookView
                {
                    Title = new Faker().Random.String(),
                    Authors = new List<AuthorView> { new AuthorView { Id = author.Id } },
                    Categories = new CategoryView[] { new CategoryView { Id = -RandomData.Number } }
                };

                _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingCategorysFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private LibraryDataBuilder _library2Builder;

            public AndUsingCategorysFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var category = CategoryBuilder.WithLibrary(library2.Id).Build();
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();

                var book = new BookView
                {
                    Title = new Faker().Random.String(),
                    Authors = new List<AuthorView> { new AuthorView { Id = author.Id } },
                    Categories = new CategoryView[] { new CategoryView { Id = category.Id } }
                };

                _response = await Client.PostObject($"/libraries/{LibraryId}/books", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                _library2Builder.CleanUp();
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }
    }
}
