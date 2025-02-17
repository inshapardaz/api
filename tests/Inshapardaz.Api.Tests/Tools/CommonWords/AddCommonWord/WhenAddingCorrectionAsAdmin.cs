using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.AddCommonWord
{
    [TestFixture]
    public class WhenAddingCommonWordAsAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private CommonWordAssert _assert;
        private CommonWordView _commonWord;


        public WhenAddingCommonWordAsAdmin()
            :base(Role.Admin)
        { }
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            _commonWord = CommonWordBuilder.BuildCommonWordView();

            _response = await Client.PostObject($"/tools/{_commonWord.Language}/words", _commonWord);
            _assert = Services.GetService<CommonWordAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldBeCreated()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldReturnCorrectObject()
        {
            _assert.ShouldBeSameAs(_commonWord);
        }

        [Test]
        public void ShouldHaveSavedWord()
        {
            _assert.ShouldHaveSavedWord();
        }
    }
}
