using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpdateUserBookProgress
{
    [TestFixture]
    public class WhenUpdatingBookProgress
        : TestBase
    {
        private HttpResponseMessage _response;
        private ReadProgressAssert _assert;
        private ReadProgressView _payload;
        private BookDto _book;

        public WhenUpdatingBookProgress()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).Build();
            _payload = new ReadProgressView { 
                ProgressId = _book.Id, 
                ProgressType = ProgressType.Unknown.ToDescription(), 
                ProgressValue = 0.1 };

            _response = await Client.PostObject($"/libraries/{LibraryId}/my/books/{_payload.ProgressId}/", _payload);
            _assert = Services.GetService<ReadProgressAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCorrectReturnObject()
        {
            _assert.ShouldMatch(_payload);
        }
        
        [Test]
        public void ShouldHaveSavedProgress()
        {
            _assert.ShouldHaveSaved(_book.Id, AccountId, _payload);
        }
    }
}
