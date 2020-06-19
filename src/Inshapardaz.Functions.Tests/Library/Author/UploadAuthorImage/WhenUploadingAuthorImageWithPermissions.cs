using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
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
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUploadingAuthorImageWithPermissions : LibraryTest<Functions.Library.Authors.UpdateAuthorImage>
    {
        private readonly ClaimsPrincipal _claim;
        private OkResult _response;
        private AuthorsDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _authorId;
        private byte[] _oldImage;

        public WhenUploadingAuthorImageWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

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
            _response = (OkResult)await handler.Run(request, LibraryId, _authorId, _claim, CancellationToken.None);
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
