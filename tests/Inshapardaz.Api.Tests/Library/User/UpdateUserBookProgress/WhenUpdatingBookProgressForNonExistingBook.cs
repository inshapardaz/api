using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpdateUserBookProgress
{
    [TestFixture]
    public class WhenUpdatingBookProgressForNonExistingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingBookProgressForNonExistingBook()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var payload = new ReadProgressView { 
                ProgressId = -RandomData.Number, 
                ProgressType = ProgressType.Unknown.ToDescription(), 
                ProgressValue = 0.1 };

            _response = await Client.PostObject($"/libraries/{LibraryId}/my/books/{payload.ProgressId}/", payload);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
