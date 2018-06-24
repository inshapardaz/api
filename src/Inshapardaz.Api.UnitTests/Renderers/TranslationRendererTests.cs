using AutoMapper;
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
using EnumHelper = Inshapardaz.Api.Helpers.EnumHelper;
using Shouldly;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class TranslationRendererTests
    {
        [TestFixture]
        public class WhenRendereingTranslations
        {
            private readonly TranslationView _result;
            private readonly Translation _translation = Builder<Translation>.CreateNew().Build();
            private readonly int _dictionaryId = 432;

            public WhenRendereingTranslations()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new TranslationRenderer(fakeLinkRenderer, new EnumRenderer(), fakeUserHelper);

                _result = renderer.Render(_translation, _dictionaryId);
            }

            [Test]
            public void ShouldRenderTranslation()
            {
                _result.ShouldNotBeNull();
            }

            [Test]
            public void ShouldRenderId()
            {
                _result.Id.ShouldBe(_translation.Id);
            }

            [Test]
            public void ShouldRenderLanguage()
            {
                _result.Language.ShouldBe(EnumHelper.GetEnumDescription(_translation.Language));
            }

            [Test]
            public void ShouldRenderTranslationValue()
            {
                _result.Value.ShouldBe(_translation.Value);
            }

            [Test]
            public void ShouldRenderLanguageId()
            {
                _result.LanguageId.ShouldBe((int)_translation.Language);
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
            private readonly TranslationView _result;
            private readonly Translation _translation = Builder<Translation>.CreateNew().Build();
            private readonly int _dictionaryId = 121;

            public WhenRendereingForOwner()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new TranslationRenderer(fakeLinkRenderer, new EnumRenderer(), fakeUserHelper);

                _result = renderer.Render(_translation, _dictionaryId);
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
