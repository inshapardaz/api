﻿using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.GetPeriodicals
{
    [TestFixture]
    public class WhenGettingPeriodicalsFirstPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<PeriodicalView> _assert;

        public WhenGettingPeriodicalsFirstPage() 
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            PeriodicalBuilder.WithLibrary(LibraryId).Build(20);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals?pageNumber=1&pageSize=10");

            _assert = Services.GetService<PagingAssert<PeriodicalView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals");
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals", 2);
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedPeriodicals()
        {
            var expectedItems = PeriodicalBuilder.Periodicals.OrderBy(a => a.Title).Take(10).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<PeriodicalAssert>().ForView(actual).ForLibrary(LibraryId)
                    .ShouldBeSameAs(expected);
            };
        }
    }
}
