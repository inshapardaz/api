using AutoMapper;
using FizzWare.NBuilder;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using EnumHelper = Inshapardaz.Api.Helpers.EnumHelper;
using Shouldly;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class TranslationRendererTests
    {
        public class WhenRendereingTranslations
        {
            private readonly TranslationView _result;
            private readonly Translation _translation = Builder<Translation>.CreateNew().Build();
            private readonly int _dictionaryId = 432;

            public WhenRendereingTranslations()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new TranslationRenderer(fakeLinkRenderer, new EnumRenderer(), fakeUserHelper);

                _result = renderer.Render(_translation, _dictionaryId);
            }

            [Fact]
            public void ShouldRenderTranslation()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderId()
            {
                _result.Id.ShouldBe(_translation.Id);
            }

            [Fact]
            public void ShouldRenderLanguage()
            {
                _result.Language.ShouldBe(EnumHelper.GetEnumDescription(_translation.Language));
            }

            [Fact]
            public void ShouldRenderTranslationValue()
            {
                _result.Value.ShouldBe(_translation.Value);
            }

            [Fact]
            public void ShouldRenderLanguageId()
            {
                _result.LanguageId.ShouldBe((int)_translation.Language);
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
            private readonly TranslationView _result;
            private readonly Translation _translation = Builder<Translation>.CreateNew().Build();
            private readonly int _dictionaryId = 121;

            public WhenRendereingForOwner()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new TranslationRenderer(fakeLinkRenderer, new EnumRenderer(), fakeUserHelper);

                _result = renderer.Render(_translation, _dictionaryId);
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
