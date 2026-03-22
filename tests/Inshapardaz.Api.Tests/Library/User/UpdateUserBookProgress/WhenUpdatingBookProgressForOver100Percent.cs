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
    public class WhenUpdatingBookProgressForOver100Percent() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var payload = new ReadProgressView { 
                ProgressId = -RandomData.Number, 
                ProgressType = ProgressType.Unknown.ToDescription(), 
                ProgressValue = 101.1 };

            _response = await Client.PostObject($"/libraries/{LibraryId}/my/books/{payload.ProgressId}/", payload);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveBadRequest() => _response.ShouldBeBadRequest();
    }
}
