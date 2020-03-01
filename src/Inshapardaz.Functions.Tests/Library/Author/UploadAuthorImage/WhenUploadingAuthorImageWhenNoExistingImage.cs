using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageWhenNoExistingImage : LibraryTest
    {
        private CreatedResult _response;
        private AuthorsDataBuilder _builder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<AuthorsDataBuilder>();

            var author = _builder.WithLibrary(LibraryId).WithoutImage().Build();
            var handler = Container.GetService<Functions.Library.Authors.UpdateAuthorImage>();
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
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test]
        public void ShouldHaveAddedImageToAuthor()
        {
            var actual = _response.Value as FileView;
            Assert.That(actual, Is.Not.Null);

            var cat = DatabaseConnection.GetAuthorById(actual.Id);
            Assert.That(cat.ImageId, Is.Not.Null, "Author should have an image.");
        }
    }
}
