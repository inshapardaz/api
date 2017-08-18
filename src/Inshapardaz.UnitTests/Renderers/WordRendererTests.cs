using System;
using System.Linq;
using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.View;
using Moq;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class WordRendererTests
    {
        public class WhenRendereingAnonymously
        {
            WordView _result;
            Domain.Model.Word _word = new Domain.Model.Word
            {
                Id = 1,
                Title = "Test",
                TitleWithMovements = "Test2",
                Description = "Test description",
                Pronunciation = "T^e`st",
                DictionaryId = 12
            };

            public WhenRendereingAnonymously()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var mockLinkRenderer = new Mock<IRenderLink>();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new WordRenderer(mockLinkRenderer.Object, fakeUserHelper);

                mockLinkRenderer.Setup(x => x.Render(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                                .Returns((string x, string r, object o) => new LinkView { Rel = r, Href = new Uri("http://link/") });

                _result = renderer.Render(_word);
            }

            [Fact]
            public void ShouldRenderWord()
            {
                Assert.NotNull(_result);
            }

            [Fact]
            public void ShouldRenderId()
            {
                Assert.Equal(_result.Id, _word.Id);
            }

            [Fact]
            public void ShouldRenderWordTitle()
            {
                Assert.Equal(_result.Title, _word.Title);
            }
            [Fact]
            public void ShouldRenderWordTitleWithmovement()
            {
                Assert.Equal(_result.TitleWithMovements, _word.TitleWithMovements);
            }

            [Fact]
            public void ShouldRenderPronunciation()
            {
                Assert.Equal(_result.Pronunciation, _word.Pronunciation);
            }

            [Fact]
            public void ShouldRenderDescription()
            {
                Assert.Equal(_result.Description, _word.Description);
            }

            [Fact]
            public void ShouldRenderDictionaryLinks()
            {
                Assert.NotNull(_result.Links);
            }

            [Fact]
            public void ShouldRenderDictionarySelfLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "self"));
            }

            [Fact]
            public void ShouldRenderDictionaryDetailsLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "details"));
            }

            [Fact]
            public void ShouldRenderDictionaryRelationsLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "relations"));
            }
        }

        public class WhenRendereingForOwner
        {
            WordView _result;
            Domain.Model.Word _word = new Domain.Model.Word
            {
                Id = 1,
                Title = "Test",
                TitleWithMovements = "Test2",
                Description = "Test description",
                Pronunciation = "T^e`st",
                DictionaryId = 12
            };

            public WhenRendereingForOwner()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var mockLinkRenderer = new Mock<IRenderLink>();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new WordRenderer(mockLinkRenderer.Object, fakeUserHelper);

                mockLinkRenderer.Setup(x => x.Render(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                                .Returns((string x, string r, object o) => new LinkView { Rel = r, Href = new Uri("http://link/") });

                _result = renderer.Render(_word);
            }

            [Fact]
            public void ShouldRenderDictionaryUpdateLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "update"));
            }

            [Fact]
            public void ShouldRenderDictionaryDeleteLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "delete"));
            }

            [Fact]
            public void ShouldRenderDictionaryAddDetailLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "add-detail"));
            }

            [Fact]
            public void ShouldRenderDictionaryAddRelationLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "add-relation"));
            }
        }
    }
}
