using AutoMapper;
using FizzWare.NBuilder;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Shouldly;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class MeaningRendererTests
    {
        public class WhenRendereingMeanings
        {
            private readonly MeaningView _result;
            private readonly Meaning _meaning = Builder<Meaning>.CreateNew().Build();
            private readonly int _dictionaryId = 432;

            public WhenRendereingMeanings()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new MeaningRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_meaning, _dictionaryId);
            }

            [Fact]
            public void ShouldRenderMeaning()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderId()
            {
                _result.Id.ShouldBe(_meaning.Id);
            }

            [Fact]
            public void ShouldRenderContext()
            {
                _result.Context.ShouldBe(_meaning.Context);
            }

            [Fact]
            public void ShouldRenderMeaningValue()
            {
                _result.Value.ShouldBe(_meaning.Value);
            }

            [Fact]
            public void ShouldRenderExample()
            {
                _result.Example.ShouldBe(_meaning.Example);
            }

            [Fact]
            public void ShouldRenderWordId()
            {
                _result.WordId.ShouldBe(_result.WordId);
            }

            [Fact]
            public void ShouldRenderLinks()
            {
                _result.Links.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderSelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldRenderWordLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Word);
            }
        }

        public class WhenRendereingForOwner
        {
            private readonly MeaningView _result;
            private readonly Meaning _meaning = Builder<Meaning>.CreateNew().Build();
            private readonly int _dictionaryId = 121;

            public WhenRendereingForOwner()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new MeaningRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_meaning, _dictionaryId);
            }

            [Fact]
            public void ShouldRenderUpdateLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Update);
            }

            [Fact]
            public void ShouldRenderDeleteLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Delete);
            }
        }
    }
}
