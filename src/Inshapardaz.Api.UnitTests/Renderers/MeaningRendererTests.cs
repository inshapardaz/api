using FizzWare.NBuilder;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    [TestFixture]
    public class MeaningRendererTests
    {
        [TestFixture]
        public class WhenRendereingMeanings
        {
            private MeaningView _result;
            private readonly Meaning _meaning = Builder<Meaning>.CreateNew().Build();
            private readonly int _dictionaryId = 432;

            [OneTimeSetUp]
            public void Setup()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new MeaningRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_meaning, _dictionaryId);
            }

            [Test]
            public void ShouldRenderMeaning()
            {
                _result.ShouldNotBeNull();
            }

            [Test]
            public void ShouldRenderId()
            {
                _result.Id.ShouldBe(_meaning.Id);
            }

            [Test]
            public void ShouldRenderContext()
            {
                _result.Context.ShouldBe(_meaning.Context);
            }

            [Test]
            public void ShouldRenderMeaningValue()
            {
                _result.Value.ShouldBe(_meaning.Value);
            }

            [Test]
            public void ShouldRenderExample()
            {
                _result.Example.ShouldBe(_meaning.Example);
            }

            [Test]
            public void ShouldRenderWordId()
            {
                _result.WordId.ShouldBe(_result.WordId);
            }

            [Test]
            public void ShouldRenderLinks()
            {
                _result.Links.ShouldNotBeNull();
            }

            [Test]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Test]
            public void ShouldRenderWordLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Word);
            }
        }

        [TestFixture]
        public class WhenRendereingForOwner
        {
            private readonly MeaningView _result;
            private readonly Meaning _meaning = Builder<Meaning>.CreateNew().Build();
            private readonly int _dictionaryId = 121;

            public WhenRendereingForOwner()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new MeaningRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_meaning, _dictionaryId);
            }

            [Test]
            public void ShouldRenderUpdateLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Update);
            }

            [Test]
            public void ShouldRenderDeleteLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Delete);
            }
        }
    }
}
