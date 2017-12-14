using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class DictionariesRendererTests
    {
        [TestFixture]
        public class WhenRenderingDictionaryForAnonymousUser
        {
            private readonly DictionariesView _result;
            private readonly IList<Dictionary> _dictionaries;

            public WhenRenderingDictionaryForAnonymousUser()
            {
                var linkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var fakeDictionaryRenderer = new FakeDictionaryRenderer();
                var renderer = new DictionariesRenderer(linkRenderer, fakeUserHelper, fakeDictionaryRenderer);
                _dictionaries = Builder<Dictionary>.CreateListOfSize(3).Build();
                _result = renderer.Render(_dictionaries, new Dictionary<int, int>(), new Dictionary<int, IEnumerable<DictionaryDownload>>());
            }

            [Test]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Test]
            public void ShouldNotRenderCreateLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Create);
            }

            [Test]
            public void ShouldRenderDictionaries()
            {
                _result.Items.ShouldNotBeNull();
                _result.Items.ShouldNotBeEmpty();
                _result.Items.Count().ShouldBe(_dictionaries.Count);
            }
        }

        [TestFixture]
        public class WhenRenderingDictionaryForLoggedInUser
        {
            private readonly DictionariesView _result;

            public WhenRenderingDictionaryForLoggedInUser()
            {
                var linkRenderer = new FakeLinkRenderer();
                var fakeDictionaryRenderer = new FakeDictionaryRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new DictionariesRenderer(linkRenderer, fakeUserHelper, fakeDictionaryRenderer);
                var dictionaries = Builder<Dictionary>.CreateListOfSize(3).Build();
                _result = renderer.Render(dictionaries, new Dictionary<int, int>(), new Dictionary<int, IEnumerable<DictionaryDownload>>());
            }

            [Test]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Test]
            public void ShouldRenderCreateLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Create);
            }
        }
    }
}