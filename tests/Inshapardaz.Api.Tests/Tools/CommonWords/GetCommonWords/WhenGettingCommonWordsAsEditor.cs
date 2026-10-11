using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.GetCommonWords
{
    [TestFixture(Role.Writer, false)]
    [TestFixture(Role.LibraryAdmin, false)]
    [TestFixture(Role.Reader, true)]
    public class WhenGettingCommonWordsAsEditor(Role role, bool isDictionaryWriter) : TestBase(role, isDictionaryWriter: isDictionaryWriter)
    {
        private HttpResponseMessage _response;
        private PagingAssert<CommonWordView> _assert;
        private readonly string _language = new Faker().Random.String2(4);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).Build(4);
            
            _response = await Client.GetAsync($"/tools/{_language}/words?pageNumber=1&pageSize=10");
            _assert = Services.GetService<PagingAssert<CommonWordView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSelfLink() => _assert.ShouldHaveSelfLink($"/tools/{_language}/words");

        [Test]
        public void ShouldHaveCreateLink() => _assert.ShouldHaveCreateLink($"/tools/{_language}/words");

        [Test]
        public void ShouldReturnCorrectDataWithUpdateLinks()
        {
            var expectedItems = CommonWordBuilder.Words.OrderBy(a => a.Word);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<CommonWordAssert>().ForView(actual)
                    .ShouldBeSameAs(item)
                    .ShouldHaveUpdateLink();
            }
        }

        [Test]
        public void ShouldHaveDeleteLinksOnlyForDictionaryWriters()
        {
            foreach (var item in CommonWordBuilder.Words)
            {
                var actual = Services.GetService<CommonWordAssert>().ForView(_assert.Data.FirstOrDefault(x => x.Id == item.Id));
                if (isDictionaryWriter)
                    actual.ShouldHaveDeleteLink();
                else
                    actual.ShouldNotHaveDeleteLink();
            }
        }
    }
}
