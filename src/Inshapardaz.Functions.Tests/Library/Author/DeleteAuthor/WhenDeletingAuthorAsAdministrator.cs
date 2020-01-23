using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.DeleteAuthor
{
    [TestFixture]
    public class WhenDeletingAuthorAsAdministrator : FunctionTest
    {
        private NoContentResult _response;

        private Ports.Database.Entities.Library.Author _expected;
        private AuthorsDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<AuthorsDataBuilder>();
            var authors = _dataBuilder.Build(4);
            _expected = authors.First();
            
            var handler = Container.GetService<Functions.Library.Authors.DeleteAuthor>();
            _response = (NoContentResult) await handler.Run(request, NullLogger.Instance, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedAuthor()
        {
            var cat = _dataBuilder.GetById(_expected.Id);
            Assert.That(cat, Is.Null, "Author should be deleted.");
        }


        [Test]
        public void ShouldHaveDeletedTheAuthorImage()
        {
            var db = Container.GetService<IDatabaseContext>();
            var file = db.File.Where(i => i.Id == _expected.ImageId);
            Assert.That(file, Is.Empty, "Author Image should be deleted");
        }

        [Test]
        public void ShouldHaveDeletedTheAuthorBooks()
        {
            var db = Container.GetService<IDatabaseContext>();
            var books = db.Book.Where(b => _expected.Books.Contains(b));
            Assert.That(books, Is.Empty, "Author Books should be deleted");
        }
    }
}
