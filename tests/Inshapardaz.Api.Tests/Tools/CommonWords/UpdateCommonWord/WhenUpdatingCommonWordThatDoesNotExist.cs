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
    public class WhenUpdatingCommonWordThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private CommonWordAssert _assert;
        private CommonWordView _commonWord;
        private readonly string _language = RandomData.Locale;
        
        public WhenUpdatingCommonWordThatDoesNotExist()
            :base(Role.Admin)
        {

        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _commonWord = new CommonWordView
            {
                Word = RandomData.Words(2),
            };

            _response = await Client.PutObject($"/tools/{_language}/words/{RandomData.Number}", _commonWord);
            _assert = Services.GetService<CommonWordAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnCreated()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveCorrectObjectReturned()
        {
            _assert.ShouldBeSameAs(new CommonWordDto
            {
                Language = _language,
                Word = _commonWord.Word,
            });
        }


        [Test]
        public void ShouldHaveCorrectObjectSaved()
        {
            _assert.ShouldMatchSavedWord(new CommonWordDto
            {
                Id = _assert.View.Id,
                Language = _language,
                Word = _commonWord.Word
            });
        }
    }
}
