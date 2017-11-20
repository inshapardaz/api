using AutoMapper;
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
    public class WordRendererTests
    {
        [TestFixture]
        public class WhenRendereingAnonymously
        {
            readonly WordView _result;

            private readonly Word _word = Builder<Word>.CreateNew().Build();

            public WhenRendereingAnonymously()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new WordRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_word, 1);
            }

            [Test]
            public void ShouldRenderWord()
            {
                _result.ShouldNotBeNull();
            }

            [Test]
            public void ShouldRenderId()
            {
                _result.Id.ShouldBe(_word.Id);
            }

            [Test]
            public void ShouldRenderWordTitle()
            {
                _result.Title.ShouldBe(_word.Title);
            }
            [Test]
            public void ShouldRenderWordTitleWithmovement()
            {
                _result.TitleWithMovements.ShouldBe(_word.TitleWithMovements);
            }

            [Test]
            public void ShouldRenderPronunciation()
            {
                _result.Pronunciation.ShouldBe(_word.Pronunciation);
            }

            [Test]
            public void ShouldRenderDescription()
            {
                _result.Description.ShouldBe(_word.Description);
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
            public void ShouldRenderMeaningsLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Meanings);
            }

            [Test]
            public void ShouldRenderTranslationLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Translations);
            }


            [Test]
            public void ShouldRenderRelationshipsLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Relationships);
            }
        }

        [TestFixture]
        public class WhenRendereingForOwner
        {
            readonly WordView _result;

            private readonly Word _word = Builder<Word>.CreateNew().Build();

            public WhenRendereingForOwner()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new WordRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_word, 1);
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

            [Test]
            public void ShouldRenderAddMeaningLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.AddMeaning);
            }

            [Test]
            public void ShouldRenderAddTranslationLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.AddTranslation);
            }

            [Test]
            public void ShouldRenderAddRelationLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.AddRelation);
            }
        }
    }
}
