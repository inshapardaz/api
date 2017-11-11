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
    public class WordRendererTests
    {
        public class WhenRendereingAnonymously
        {
            readonly WordView _result;

            private readonly Word _word = Builder<Word>.CreateNew().Build();

            public WhenRendereingAnonymously()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new WordRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_word, 1);
            }

            [Fact]
            public void ShouldRenderWord()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderId()
            {
                _result.Id.ShouldBe(_word.Id);
            }

            [Fact]
            public void ShouldRenderWordTitle()
            {
                _result.Title.ShouldBe(_word.Title);
            }
            [Fact]
            public void ShouldRenderWordTitleWithmovement()
            {
                _result.TitleWithMovements.ShouldBe(_word.TitleWithMovements);
            }

            [Fact]
            public void ShouldRenderPronunciation()
            {
                _result.Pronunciation.ShouldBe(_word.Pronunciation);
            }

            [Fact]
            public void ShouldRenderDescription()
            {
                _result.Description.ShouldBe(_word.Description);
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
            public void ShouldRenderMeaningsLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Meanings);
            }

            [Fact]
            public void ShouldRenderTranslationLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Translations);
            }


            [Fact]
            public void ShouldRenderRelationshipsLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Relationships);
            }
        }

        public class WhenRendereingForOwner
        {
            readonly WordView _result;

            private readonly Word _word = Builder<Word>.CreateNew().Build();

            public WhenRendereingForOwner()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new WordRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_word, 1);
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

            [Fact]
            public void ShouldRenderAddMeaningLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.AddMeaning);
            }

            [Fact]
            public void ShouldRenderAddTranslationLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.AddTranslation);
            }

            [Fact]
            public void ShouldRenderAddRelationLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.AddRelation);
            }
        }
    }
}
