using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.UpdateCommonWord
{
    [TestFixture]
    public class WhenUpdatingCommonWordAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private CommonWordAssert _assert;
        private CommonWordDto _commonWord;
        private CommonWordView _update;

        public WhenUpdatingCommonWordAdmin()
            :base(Role.Admin)
        {

        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _commonWord = CommonWordBuilder.Build();
            _update = new CommonWordView
            {
                Word = _commonWord.Word + "2",
            };

            _response = await Client.PutObject($"/tools/{_commonWord.Language}/words/{_commonWord.Id}", _update);
            _assert = Services.GetService<CommonWordAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedCorrection()
        {
            _assert.ShouldMatchSavedWord(new CommonWordDto
            {
                Id = _commonWord.Id,
                Language = _commonWord.Language,
                Word = _update.Word
            });
        }
    }
}
