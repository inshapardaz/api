using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageWhenNoExistingImage : LibraryTest<Functions.Library.Authors.UpdateAuthorImage>
    {
        private CreatedResult _response;
        private AuthorsDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _authorId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<AuthorsDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;

            var author = _builder.WithLibrary(LibraryId).WithoutImage().Build();
            _authorId = author.Id;

            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (CreatedResult)await handler.Run(request, LibraryId, author.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            var authorAssert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
            authorAssert.ShouldHaveCorrectImageLocationHeader(_authorId);
        }

        [Test]
        public void ShouldHaveAddedImageToAuthor()
        {
            AuthorAssert.ShouldHaveAddedAuthorImage(_authorId, DatabaseConnection, _fileStorage);
        }

        [Test]
        public void ShouldSavePublicImage()
        {
            AuthorAssert.ShouldHavePublicImage(_authorId, DatabaseConnection);
        }
    }
}
