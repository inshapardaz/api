using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Domain.Model;
using Moq;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class DictionaryRendererTests
    {
        public class WhenRendereingAnonymously
        {
            private readonly DictionaryView _result;

            private readonly Dictionary _dictionary = new Domain.Model.Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = 4,
                IsPublic = false,
                UserId = "12",
                Words = new List<Word> { new Word(), new Word() }
            };

            public WhenRendereingAnonymously()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var mockLinkRenderer = new Mock<IRenderLink>();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new DictionaryRenderer(mockLinkRenderer.Object, fakeUserHelper);

                mockLinkRenderer.Setup(x => x.Render(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                                .Returns((string x, string r, object o) => new LinkView { Rel = r, Href = new Uri("http://link/") });

                _result = renderer.Render(_dictionary);
            }

            [Fact]
            public void ShouldRenderDictionary()
            {
                Assert.NotNull(_result);
            }

            [Fact]
            public void ShouldRenderDictionaryId()
            {
                Assert.Equal(_result.Id, _dictionary.Id);
            }

            [Fact]
            public void ShouldRenderDictionaryName()
            {
                Assert.Equal(_result.Name, _dictionary.Name);
            }

            [Fact]
            public void ShouldRenderDictionaryLanguage()
            {
                Assert.Equal(_result.Language, _dictionary.Language);
            }

            [Fact]
            public void ShouldRenderWordCount()
            {
                Assert.Equal(_result.WordCount, _dictionary.Words.Count);
            }

            [Fact]
            public void ShouldRenderPublicFlag()
            {
                Assert.Equal(_result.IsPublic, _dictionary.IsPublic);
            }

            [Fact]
            public void ShouldRenderDictionaryLinks()
            {
                Assert.NotNull(_result.Links);
            }

            [Fact]
            public void ShouldRenderIndexes()
            {
                Assert.NotNull(_result.Indexes);
                Assert.NotEqual(_result.Indexes.Count(), 0);
            }

            [Fact]
            public void ShouldRenderDictionarySelfLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "self"));
            }

            [Fact]
            public void ShouldRenderDictionaryIndexLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "index"));
            }

            [Fact]
            public void ShouldRenderDictionarySearchLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "search"));
            }
        }

        public class WhenRendereingForOwner
        {
            private DictionaryView _result;

            private Domain.Model.Dictionary _dictionary = new Domain.Model.Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = 4,
                IsPublic = false,
                UserId = "12"
            };

            public WhenRendereingForOwner()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var mockLinkRenderer = new Mock<IRenderLink>();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new DictionaryRenderer(mockLinkRenderer.Object, fakeUserHelper);

                mockLinkRenderer.Setup(x => x.Render(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                                .Returns((string x, string r, object o) => new LinkView { Rel = r, Href = new Uri("http://link/") });

                _result = renderer.Render(_dictionary);
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
            public void ShouldRenderDictionaryCreateWordLink()
            {
                Assert.NotNull(_result.Links.SingleOrDefault(l => l.Rel == "create-word"));
            }
        }
    }
}