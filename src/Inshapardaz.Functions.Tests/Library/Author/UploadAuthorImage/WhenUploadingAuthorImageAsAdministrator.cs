using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageAsAdministrator : LibraryTest<Functions.Library.Authors.UpdateAuthorImage>
    {
        private OkResult _response;
        private AuthorsDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _authorId;
        private byte[] _oldImage;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<AuthorsDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;

            var author = _builder.WithLibrary(LibraryId).Build();
            _authorId = author.Id;

            var imageUrl = DatabaseConnection.GetAuthorImageUrl(_authorId);

            _oldImage = await _fileStorage.GetFile(imageUrl, CancellationToken.None);
            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (OkResult)await handler.Run(request, LibraryId, _authorId, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedAuthorImage()
        {
            AuthorAssert.ShouldHaveUpdatedAuthorImage(_authorId, _oldImage, DatabaseConnection, _fileStorage);
        }
    }
}
