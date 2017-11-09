using System;
using System.Linq;
using AutoMapper;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Shouldly;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class DictionaryRendererTests
    {
        public class WhenRendereingAnonymously
        {
            private readonly DictionaryView _result;
            private int wordCount = 23;

            private readonly Dictionary _dictionary = new Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = Languages.French,
                IsPublic = false,
                UserId = Guid.NewGuid()
            };

            public WhenRendereingAnonymously()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, wordCount);
            }

            [Fact]
            public void ShouldRenderDictionary()
            {
                _result.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldRenderDictionaryId()
            {
                _result.Id.ShouldBe(_dictionary.Id);
            }

            [Fact]
            public void ShouldRenderDictionaryName()
            {
                _result.Name.ShouldBe(_dictionary.Name);
            }

            [Fact]
            public void ShouldRenderDictionaryLanguage()
            {
                _result.Language.ShouldBe(_dictionary.Language);
            }

            [Fact]
            public void ShouldRenderWordCount()
            {
                _result.WordCount.ShouldBe(wordCount);
            }

            [Fact]
            public void ShouldRenderPublicFlag()
            {
                _result.IsPublic.ShouldBe(_dictionary.IsPublic);
            }

            [Fact]
            public void ShouldRenderDictionaryLinks()
            {
                _result.Links.ShouldNotBeNull();
                _result.Links.ShouldNotBeEmpty();
            }

            [Fact]
            public void ShouldRenderIndexes()
            {
                _result.Indexes.ShouldNotBeNull();
                _result.Indexes.ShouldNotBeEmpty();
            }

            [Fact]
            public void ShouldRenderDictionarySelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Fact]
            public void ShouldRenderDictionaryIndexLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Index);
            }

            [Fact]
            public void ShouldRenderDictionarySearchLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Search);
            }
        }

        public class WhenRendereingForOwner
        {
            private readonly DictionaryView _result;

            private readonly Dictionary _dictionary = new Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = Languages.Chinese,
                IsPublic = false,
                UserId = Guid.NewGuid()
            };

            public WhenRendereingForOwner()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, 0);
            }

            [Fact]
            public void ShouldRenderDictionaryUpdateLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Update);
            }

            [Fact]
            public void ShouldRenderDictionaryDeleteLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Delete);
            }

            [Fact]
            public void ShouldRenderDictionaryCreateWordLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.CreateWord);
            }

            [Fact]
            public void ShouldRenderDictionaryCreateDownloadLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.CreateDownload);
            }
        }
    }
}