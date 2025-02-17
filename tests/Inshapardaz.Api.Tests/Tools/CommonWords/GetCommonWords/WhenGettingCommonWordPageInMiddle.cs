using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Tools;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.GetCommonWords
{
    [TestFixture]
    public class WhenGettingCommonWordPageInMiddle : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<CommonWordView> _assert;
        private readonly string _language = new Faker().Random.String2(4);

        public WhenGettingCommonWordPageInMiddle()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).Build(50);

            _response = await Client.GetAsync($"/tools/{_language}/words?pageNumber=3&pageSize=10");
            _assert = Services.GetService<PagingAssert<CommonWordView>>().ForResponse(_response);
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
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/tools/{_language}/words", 3, 10);
        }

        

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/tools/{_language}/words", 4, 10);
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/tools/{_language}/words", 2, 10);
        }

        [Test]
        public void ShouldReturnExpectedData()
        {
            var expectedItems = CommonWordBuilder
                .Words
                .OrderBy(a => a.Word)
                .Skip(2 * 10).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<CommonWordAssert>().ForView(actual)
                    .ShouldBeSameAs(item)
                    .ShouldNotHaveUpdateLink()
                    .ShouldNotHaveDeleteLink();
            }
        }
    }
}
