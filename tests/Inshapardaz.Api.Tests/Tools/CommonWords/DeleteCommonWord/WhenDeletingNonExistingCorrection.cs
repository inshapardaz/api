using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.DeleteCommonWord
{
    [TestFixture]
    public class WhenDeletingNonExistingCommonWord : TestBase
    {
        private HttpResponseMessage _response;
        private CommonWordAssert _assert;
        private CommonWordDto _commonWord;

        public WhenDeletingNonExistingCommonWord() 
            : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _commonWord = CommonWordBuilder.Build();

            _response = await Client.DeleteAsync($"/tools/{_commonWord.Language}/words/-{_commonWord.Id}");
            _assert = Services.GetService<CommonWordAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedAuthor()
        {
            _assert.ShouldNotHaveDeletedWord(_commonWord.Id);
        }
    }
}
