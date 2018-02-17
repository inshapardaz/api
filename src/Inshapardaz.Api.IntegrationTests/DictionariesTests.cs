using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.DataHelper;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests
{
    public class DictionariesTests
    {
        [TestFixture]
        public class WhenGettingDictionariesAnonymously : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionariesView _view;
            private DictionaryDataHelpers _dictionaryDataHelper;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionaryDataHelper.CreateDictionary(new Dictionary {Id = -1, IsPublic = true, Name = "Test1"});
                _dictionaryDataHelper.CreateDictionary(new Dictionary {Id = -2, IsPublic = true, Name = "Test2"});
                _dictionaryDataHelper.CreateDictionary(new Dictionary {Id = -3, IsPublic = false, Name = "Test3", UserId = Guid.NewGuid()});
                _dictionaryDataHelper.RefreshIndex();

                _response = await GetClient().GetAsync("/api/dictionaries");
                _view = JsonConvert.DeserializeObject<DictionariesView>(await _response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(-1);
                _dictionaryDataHelper.DeleteDictionary(-2);
                _dictionaryDataHelper.DeleteDictionary(-3);
            }

            [Test]
            public void ShouldReturn200()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }

            [Test]
            public void ShouldHaveCorrectResponseBody()
            {
                Assert.That(_view, Is.Not.Null);
            }

            [Test]
            public void ShouldHaveSelfLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Self && l.Href != null);
            }

            [Test]
            public void ShouldNotHaveCreateLink()
            {
                _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Create && l.Href != null);
            }

            [Test]
            public void ShouldReturnPublicDictionaries()
            {
                _view.Items.Count().ShouldBe(2);
                _view.Items.ShouldAllBe(d => d.IsPublic);
            }

            [Test]
            public void ShouldNotReturnPrivateDictionaries()
            {
                _view.Items.ShouldNotContain(d => d.IsPublic == false);
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenGettingDictionariesAsLoggedInUser : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionariesView _view;
            private DictionaryDataHelpers _dictionaryDataHelper;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var user1 = Guid.NewGuid();
                var user2 = Guid.NewGuid();
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionaryDataHelper.CreateDictionary(new Dictionary {Id = -1, IsPublic = true, Name = "Test1", UserId = user1});
                _dictionaryDataHelper.CreateDictionary(new Dictionary {Id = -2, IsPublic = false, Name = "Test2", UserId = user1});
                _dictionaryDataHelper.CreateDictionary(new Dictionary {Id = -3, IsPublic = false, Name = "Test3", UserId = user2});
                _dictionaryDataHelper.CreateDictionary(new Dictionary {Id = -4, IsPublic = true, Name = "Test4", UserId = user2});
                _dictionaryDataHelper.RefreshIndex();


                _response = await GetAuthenticatedClient(user1).GetAsync("/api/dictionaries");
                _view = JsonConvert.DeserializeObject<DictionariesView>(await _response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(-1);
                _dictionaryDataHelper.DeleteDictionary(-2);
                _dictionaryDataHelper.DeleteDictionary(-3);
                _dictionaryDataHelper.DeleteDictionary(-4);
            }

            [Test]
            public void ShouldReturn200()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }

            [Test]
            public void ShouldHaveCorrectResponseBody()
            {
                _view.ShouldNotBeNull();
            }

            [Test]
            public void ShouldHaveSelfLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Self && l.Href != null);
            }

            [Test]
            public void ShouldHaveCreateLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Create && l.Href != null);
            }

            [Test]
            public void ShouldReturnUsersPublicDictionary()
            {
                _view.Items.ShouldContain(d => d.Id == -1);
            }

            [Test]
            public void ShouldReturnUsersPrivateDictionary()
            {
                _view.Items.ShouldContain(d => d.Id == -2);
            }

            [Test]
            public void ShouldNotReturnOtherUsersPrivateDictionary()
            {
                _view.Items.ShouldNotContain(d => d.Id == -3);
            }

            [Test]
            public void ShouldReturnOtherUsersPublicDictionary()
            {
                _view.Items.ShouldContain(d => d.Id == -4);
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenGettingADictionaryAsAnonymousUser : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryView _view;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = true,
                    Name = "Test1",
                    UserId = Guid.NewGuid(),
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    } 
                };
                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _response = await GetClient().GetAsync("/api/dictionaries/-1");
                _view = JsonConvert.DeserializeObject<DictionaryView>(await _response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(-1);
            }

            [Test]
            public void ShouldReturn200()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }

            [Test]
            public void ShouldHaveCorrectResponseBody()
            {
                Assert.That(_view, Is.Not.Null);
            }

            [Test]
            public void ShouldHaveSelfLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Self && l.Href != null);
            }

            [Test]
            public void ShouldNotHaveUpdateLink()
            {
                _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Update && l.Href != null);
            }

            [Test]
            public void ShouldNotHaveDeleteLink()
            {
                _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Delete && l.Href != null);
            }

            [Test]
            public void ShouldNotHaveCreateWordLink()
            {
                _view.Links.ShouldNotContain(l => l.Rel == RelTypes.CreateWord && l.Href != null);
            }

            [Test]
            public void ShouldHaveIndexLinks()
            {
                _view.Indexes.ShouldNotBeEmpty();
            }

            [Test]
            public void ShouldHaveDownloadLinks()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Download && l.Href != null && l.Type == MimeTypes.SqlLite);
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Download && l.Href != null && l.Type == MimeTypes.Csv);
            }

            [Test]
            public void ShouldReturnCorrectDictionaryMetadata()
            {
                _view.Id.ShouldBe(_dictionary.Id);
                _view.Name.ShouldBe(_dictionary.Name);
                _view.Language.ShouldBe(_dictionary.Language);
                _view.IsPublic.ShouldBe(_dictionary.IsPublic);
                _view.UserId.ShouldBe(_dictionary.UserId);
                _view.WordCount.ShouldBe(0);
                _view.Indexes.ShouldNotBeEmpty();
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenGettingAPrivateDictionaryAsAnonymousUser : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryView _view;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    UserId = Guid.NewGuid(),
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite }
                    }
                };
                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _response = await GetClient().GetAsync("/api/dictionaries/-1");
                _view = JsonConvert.DeserializeObject<DictionaryView>(await _response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(-1);
            }

            [Test]
            public void ShouldReturnUnAuthorised()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            }

            [Test]
            public void ShouldNotHaveDataInResponseBody()
            {
                Assert.That(_view, Is.Null);
            }
            
            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenGettingOwnPrivateDictionary : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryView _view;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                var userId = Guid.NewGuid();
                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    UserId = userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };
                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _response = await GetAuthenticatedClient(userId).GetAsync("/api/dictionaries/-1");
                _view = JsonConvert.DeserializeObject<DictionaryView>(await _response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(-1);
            }

            [Test]
            public void ShouldReturn200()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }

            [Test]
            public void ShouldHaveCorrectResponseBody()
            {
                Assert.That(_view, Is.Not.Null);
            }

            [Test]
            public void ShouldHaveSelfLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Self && l.Href != null);
            }

            [Test]
            public void ShouldHaveUpdateLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Update && l.Href != null);
            }

            [Test]
            public void ShouldHaveDeleteLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Delete && l.Href != null);
            }

            [Test]
            public void ShouldHaveCreateDownloadLink()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.CreateDownload && l.Href != null);
            }

            [Test]
            public void ShouldHaveIndexLinks()
            {
                _view.Indexes.ShouldNotBeEmpty();
            }

            [Test]
            public void ShouldHaveDownloadLinks()
            {
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Download && l.Href != null && l.Type == MimeTypes.SqlLite);
                _view.Links.ShouldContain(l => l.Rel == RelTypes.Download && l.Href != null && l.Type == MimeTypes.Csv);
            }

            [Test]
            public void ShouldReturnCorrectDictionaryMetadata()
            {
                _view.Id.ShouldBe(_dictionary.Id);
                _view.Name.ShouldBe(_dictionary.Name);
                _view.Language.ShouldBe(_dictionary.Language);
                _view.IsPublic.ShouldBe(_dictionary.IsPublic);
                _view.UserId.ShouldBe(_dictionary.UserId);
                _view.WordCount.ShouldBe(0);
                _view.Indexes.ShouldNotBeEmpty();
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenGettingOtherUsersPrivateDictionary : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryView _view;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                var userId = Guid.NewGuid();
                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    UserId = userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };
                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _response = await GetAuthenticatedClient(Guid.NewGuid()).GetAsync("/api/dictionaries/-1");
                _view = JsonConvert.DeserializeObject<DictionaryView>(await _response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(-1);
            }

            [Test]
            public void ShouldReturnUnauthorised()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            }

            [Test]
            public void ShouldHaveEmptyResponseBody()
            {
                Assert.That(_view, Is.Null);
            }
            
            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenCreatingADictionaryAsAnonymousUser : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                var userId = Guid.NewGuid();
                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    UserId = userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };

                _response = await GetClient().PostJson("/api/dictionaries", _dictionary);
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(-1);
            }

            [Test]
            public void ShouldReturnUnauthorised()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            }


            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenCreatingADictionary : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryView _view;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;
            private readonly Guid  _userId = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };

                _response = await GetAuthenticatedClient(_userId).PostJson("/api/dictionaries", _dictionary);
                _view = JsonConvert.DeserializeObject<DictionaryView>(await _response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(_view.Id);
            }

            [Test]
            public void ShouldReturnCreated()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Created);
            }

            [Test]
            public void ShouldReturnNewItemLink()
            {
                _response.Headers.Location.ShouldNotBeNull();
            }

            [Test]
            public void ShouldReturnCreatedDictionary()
            {
                _view.ShouldNotBeNull();
            }

            [Test]
            public void ShouldCreateCorrectDictionary()
            {
                _view.Name.ShouldBe(_dictionary.Name);
                _view.Language.ShouldBe(_dictionary.Language);
                _view.IsPublic.ShouldBe(_dictionary.IsPublic);
                _view.UserId.ShouldBe(_userId);
                _view.WordCount.ShouldBe(0);
                _view.Indexes.ShouldNotBeEmpty();
            }


            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenUpdatingADictionary : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private Dictionary _createdDictionary;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;
            private readonly Guid _userId = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };

                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                _response = await GetAuthenticatedClient(_userId).PutJson($"/api/dictionaries/{_dictionary.Id}", _dictionary);

                _dictionaryDataHelper.RefreshIndex();
                _createdDictionary = _dictionaryDataHelper.GetDictionary(_dictionary.Id);
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnCreated()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            }
            
            [Test]
            public void ShouldCreateCorrectDictionary()
            {
                _createdDictionary.Name.ShouldBe(_dictionary.Name);
                _createdDictionary.Language.ShouldBe(_dictionary.Language);
                _createdDictionary.IsPublic.ShouldBe(_dictionary.IsPublic);
                _createdDictionary.UserId.ShouldBe(_userId);
            }


            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenUpdatingDictionaryOfOtherUser : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;
            private readonly Guid _userId1 = Guid.NewGuid();
            private readonly Guid _userId2 = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId2,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };

                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                _response = await GetAuthenticatedClient(_userId1).PutJson($"/api/dictionaries/{_dictionary.Id}", _dictionary);
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnUnauthorised()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            }

            
            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenUpdatingDictionaryAsAnonymousUser : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;
            private readonly Guid _userId = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };

                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                _response = await GetClient().PutJson($"/api/dictionaries/{_dictionary.Id}", _dictionary);
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnUnauthorised()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            }
            
            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenDeletingADictionary : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private Dictionary _existedDictionary;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;
            private readonly Guid _userId = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };

                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                _response = await GetAuthenticatedClient(_userId).DeleteAsync($"/api/dictionaries/{_dictionary.Id}");

                _dictionaryDataHelper.RefreshIndex();
                _existedDictionary = _dictionaryDataHelper.GetDictionary(_dictionary.Id);
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnNoContent()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            }

            [Test]
            public void ShouldRemoveDictionary()
            {
                _existedDictionary.ShouldBeNull();
            }


            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenDeletingOtherUsersDictionary : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;
            private readonly Guid _userId1 = Guid.NewGuid();
            private readonly Guid _userId2 = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId1,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload {Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite},
                        new DictionaryDownload {Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv}
                    }
                };

                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                _response = await GetAuthenticatedClient(_userId2).DeleteAsync($"/api/dictionaries/{_dictionary.Id}");
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnUnauthorised()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            }
            
            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }

        [TestFixture]
        public class WhenDeletingDictionaryAsAnonymousUser : IntegrationTestBase
        {
            private HttpResponseMessage _response;
            private DictionaryDataHelpers _dictionaryDataHelper;
            private Dictionary _dictionary;
            private readonly Guid _userId = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

                _dictionary = new Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload {Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite},
                        new DictionaryDownload {Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv}
                    }
                };

                _dictionaryDataHelper.CreateDictionary(_dictionary);
                _dictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                _response = await GetClient().DeleteAsync($"/api/dictionaries/{_dictionary.Id}");
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                _dictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnUnauthorised()
            {
                _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                if (isDisposing)
                {
                    _response?.Dispose();
                }
            }
        }
    }
}
