using System.Net;
using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.GetCorrections
{
    [TestFixture]
    public class WhenGettingFullCorrectionList() : TestBase()
    {
        private HttpResponseMessage _response;
        private HttpResponseMessage _cached;
        private List<CorrectionSimpleView> _list;
        private readonly string _language = new Faker().Random.String2(6);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CorrectionBuilder.WithLanguage(_language).WithProfile(CorrectionProfiles.Punctuation)
                .BuildWith(("a", "1", 3), ("b", "2", 9), ("c", "3", 0));

            _response = await Client.GetAsync($"/tools/{_language}/spellchecker/punctuation");
            _list = (await _response.GetContent<IEnumerable<CorrectionSimpleView>>()).ToList();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/tools/{_language}/spellchecker/punctuation");
            request.Headers.IfNoneMatch.Add(_response.Headers.ETag);
            _cached = await Client.SendAsync(request);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldListMostUsedFirst() => _list.Select(c => c.IncorrectText).Should().Equal("b", "a", "c");

        [Test]
        public void ShouldIncludeIdsAndUsage()
        {
            _list.Should().OnlyContain(c => c.Id > 0);
            _list.Select(c => c.Usage).Should().Equal(9, 3, 0);
        }

        [Test]
        public void ShouldReturnNotModifiedForSameVersion() => _cached.StatusCode.Should().Be(HttpStatusCode.NotModified);
    }

    [TestFixture]
    public class WhenGettingCorrectionsSortedByUsage() : TestBase()
    {
        private PageView<CorrectionView> _page;
        private readonly string _language = new Faker().Random.String2(6);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CorrectionBuilder.WithLanguage(_language).WithProfile(CorrectionProfiles.Autocorrect)
                .BuildWith(("aaa", "x", 1), ("bbb", "y", 50), ("ccc", "z", 7));

            var response = await Client.GetAsync($"/tools/{_language}/corrections/autocorrect?sortBy=usage&pageSize=2");
            response.ShouldBeOk();
            _page = await response.GetContent<PageView<CorrectionView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnMostUsedFirst() => _page.Data.Select(c => c.IncorrectText).Should().Equal("bbb", "ccc");

        [Test]
        public void ShouldCountAllMatches() => _page.TotalCount.Should().Be(3);

        [Test]
        public void ShouldKeepSortInPagingLinks() => _page.Link(RelTypes.Next).ShouldHaveQueryParameter("sortBy", "usage");
    }
}
