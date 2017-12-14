using System;
using System.Collections.Generic;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.UnitTests.Renderers
{
    public class DictionaryRendererTests
    {
        [TestFixture]
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
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, wordCount, new DictionaryDownload[0]);
            }

            [Test]
            public void ShouldRenderDictionary()
            {
                _result.ShouldNotBeNull();
            }

            [Test]
            public void ShouldRenderDictionaryId()
            {
                _result.Id.ShouldBe(_dictionary.Id);
            }

            [Test]
            public void ShouldRenderDictionaryName()
            {
                _result.Name.ShouldBe(_dictionary.Name);
            }

            [Test]
            public void ShouldRenderDictionaryLanguage()
            {
                _result.Language.ShouldBe(_dictionary.Language);
            }

            [Test]
            public void ShouldRenderWordCount()
            {
                _result.WordCount.ShouldBe(wordCount);
            }

            [Test]
            public void ShouldRenderPublicFlag()
            {
                _result.IsPublic.ShouldBe(_dictionary.IsPublic);
            }

            [Test]
            public void ShouldRenderDictionaryLinks()
            {
                _result.Links.ShouldNotBeNull();
                _result.Links.ShouldNotBeEmpty();
            }

            [Test]
            public void ShouldRenderIndexes()
            {
                _result.Indexes.ShouldNotBeNull();
                _result.Indexes.ShouldNotBeEmpty();
            }

            [Test]
            public void ShouldRenderDictionarySelfLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Self);
            }

            [Test]
            public void ShouldRenderDictionaryIndexLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Index);
            }

            [Test]
            public void ShouldRenderDictionarySearchLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Search);
            }
        }

        [TestFixture]
        public class WhenRendereingPublicDictionaryWithDownloadAnonymously
        {
            private readonly DictionaryView _result;
            private int wordCount = 23;

            private readonly Dictionary _dictionary = new Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = Languages.French,
                IsPublic = true,
                UserId = Guid.NewGuid()
            };

            private readonly List<DictionaryDownload> _downloads = new List<DictionaryDownload>
            {
                new DictionaryDownload
                {
                    MimeType = MimeTypes.SqlLite
                }
            };

            public WhenRendereingPublicDictionaryWithDownloadAnonymously()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, wordCount, _downloads);
            }

            [Test]
            public void ShouldRenderDownloadLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Download);
                _result.Links.ShouldContain(l => l.Type == MimeTypes.SqlLite);
            }
        }

        [TestFixture]
        public class WhenRendereingPrivateDictionaryWithDownloadAnonymously
        {
            private readonly DictionaryView _result;
            private int wordCount = 23;

            private readonly Dictionary _dictionary = new Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = Languages.French,
                IsPublic = false,
                UserId = Guid.NewGuid(),
                Downloads = new List<DictionaryDownload>{ new DictionaryDownload
                {
                    MimeType = MimeTypes.SqlLite
                }}
            };

            public WhenRendereingPrivateDictionaryWithDownloadAnonymously()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, wordCount, new DictionaryDownload[0]);
            }

            [Test]
            public void ShouldNotRenderDownloadLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Download);
            }
        }

        [TestFixture]
        public class WhenRendereingPrivateDictionaryWithDownloadAsContributor
        {
            private readonly DictionaryView _result;
            private int wordCount = 23;

            private readonly Dictionary _dictionary = new Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = Languages.French,
                IsPublic = false,
                UserId = Guid.NewGuid(),
            };

            private readonly List<DictionaryDownload> _downloads = new List<DictionaryDownload>
            {
                new DictionaryDownload
                {
                    MimeType = MimeTypes.SqlLite
                },
                new DictionaryDownload
                {
                    MimeType = "text/plain"
                }

            };

            public WhenRendereingPrivateDictionaryWithDownloadAsContributor()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, wordCount, _downloads);
            }

            [Test]
            public void ShouldRenderDownloadLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Download);
                _result.Links.ShouldContain(l => l.Type == MimeTypes.SqlLite);
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Download);
                _result.Links.ShouldContain(l => l.Type == "text/plain");
            }
        }

        [TestFixture]
        public class WhenRendereingPrivateDictionaryWithDownloadAsReader
        {
            private readonly DictionaryView _result;
            private int wordCount = 23;

            private readonly Dictionary _dictionary = new Dictionary
            {
                Id = 1,
                Name = "Test",
                Language = Languages.French,
                IsPublic = false,
                UserId = Guid.NewGuid(),
                Downloads = new List<DictionaryDownload>{ new DictionaryDownload
                {
                    MimeType = MimeTypes.SqlLite
                }}
            };

            public WhenRendereingPrivateDictionaryWithDownloadAsReader()
            {
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsReader();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, wordCount, new DictionaryDownload[0]);
            }

            [Test]
            public void ShouldNotRenderDownloadLink()
            {
                _result.Links.ShouldNotContain(l => l.Rel == RelTypes.Download);
            }
        }

        [TestFixture]
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
                var fakeLinkRenderer = new FakeLinkRenderer();
                var fakeUserHelper = new FakeUserHelper().AsContributor();
                var renderer = new DictionaryRenderer(fakeLinkRenderer, fakeUserHelper);

                _result = renderer.Render(_dictionary, 0, new DictionaryDownload[0]);
            }

            [Test]
            public void ShouldRenderDictionaryUpdateLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Update);
            }

            [Test]
            public void ShouldRenderDictionaryDeleteLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.Delete);
            }

            [Test]
            public void ShouldRenderDictionaryCreateWordLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.CreateWord);
            }

            [Test]
            public void ShouldRenderDictionaryCreateDownloadLink()
            {
                _result.Links.ShouldContain(l => l.Rel == RelTypes.CreateDownload);
            }
        }
    }
}