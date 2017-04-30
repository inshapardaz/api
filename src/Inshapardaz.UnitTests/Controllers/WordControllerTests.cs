using AutoMapper;
using Inshapardaz.Configuration;
using Inshapardaz.Controllers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Model;
using Inshapardaz.Renderers;
using Inshapardaz.UnitTests.Fakes;
using Inshapardaz.UnitTests.Fakes.Helpers;
using Inshapardaz.UnitTests.Fakes.Renderers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using paramore.brighter.commandprocessor;
using Xunit;

namespace Inshapardaz.UnitTests.Controllers
{
    public class WordControllerTests
    {
        public class WhenGettingWordsForDictionary : TestContext
        {
            public WhenGettingWordsForDictionary()
            {
                _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary());
                _fakeQueryProcessor.SetupResultFor<WordQuery, Page<Word>>(new Page<Word>());
                _result = _controller.Get(12, 1);
            }

            [Fact]
            public void ShouldReturnOkResult()
            {
                Assert.IsType<OkObjectResult>(_result);
            }

            [Fact]
            public void ShouldReturnThePagedData()
            {
                var result = _result as OkObjectResult;
                
                Assert.NotNull(result.Value);
                Assert.IsType<PageView<WordView>>(result.Value);
            }
        }

        public class WhenGettingWordsForPrivateDictionaryOfOthers : TestContext
        {
            public WhenGettingWordsForPrivateDictionaryOfOthers()
            {
                _fakeUserHelper.WithUserId("1");
                _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery,Dictionary>(new Dictionary
                {
                    UserId = "2",
                    IsPublic = false
                });

                _result = _controller.Get(12, 1);
            }

            [Fact]
            public void ShouldReturnUnauthorised()
            {
                Assert.IsType<UnauthorizedResult>(_result);
            }
        }

        public class WhenGettingWordsForPublicDictionaryOfOthers : TestContext
        {
            public WhenGettingWordsForPublicDictionaryOfOthers()
            {
                _fakeUserHelper.WithUserId("1");
                _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary
                {
                    UserId = "2",
                    IsPublic = true
                });

                _fakeQueryProcessor.SetupResultFor<WordQuery, Page<Word>>(new Page<Word>());
                _result = _controller.Get(12, 1);
            }

            [Fact]
            public void ShouldReturnOk()
            {
                Assert.IsType<OkObjectResult>(_result);
            }
        }

        public class WhenGettingWordsForDictionaryThatDoesNotExist : TestContext
        {
            public WhenGettingWordsForDictionaryThatDoesNotExist()
            {
                _fakeUserHelper.WithUserId("1");
                _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(null);
                _result = _controller.Get(12, 1);
            }

            [Fact]
            public void ShouldReturnNotFound()
            {
                Assert.IsType<NotFoundResult>(_result);
            }
        }

        public class WhenGettingWordById : TestContext
        {
            public WhenGettingWordById()
            {
                _fakeUserHelper.AsContributor();
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
                _result = _controller.Get(34);
            }

            [Fact]
            public void ShouldReturnOkResult()
            {
                Assert.IsType<OkObjectResult>(_result);
            }

            [Fact]
            public void ShoudldQueryWordRequested()
            {
                var response = _result as OkObjectResult;

                Assert.NotNull(response.Value);
                Assert.IsType<WordView>(response.Value);
            }
        }

        public class WhenGettingNonExistingWordById : TestContext
        {
            public WhenGettingNonExistingWordById()
            {
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(null);
                _result = _controller.Get(34);
            }

            [Fact]
            public void ShouldReturnNotFoundResult()
            {
                Assert.IsType<NotFoundResult>(_result);
            }
        }

        public class WhenGettingWordBelongingToPrivateDictionaryWithNoAccess : TestContext
        {
            public WhenGettingWordBelongingToPrivateDictionaryWithNoAccess()
            {
                _fakeUserHelper.WithUserId("32");
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(null);
                _result = _controller.Get(34);
            }

            [Fact]
            public void ShouldQueryForWordWithCorrectUser()
            {
                _fakeQueryProcessor.AssertQueryExecuted<WordByIdQuery>(q => q.UserId == "32");
            }
        }

        public class WhenPostingForWord : TestContext
        {
            private const int DictionaryId = 56;
            private readonly WordView _wordView = new WordView
            {
                Title = "a",
                TitleWithMovements = "a^A",
                Pronunciation = "~a",
                Description = "Some description"
            };

            public WhenPostingForWord()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, WordView>(_wordView);
                _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary());
                _fakeWordRenderer.WithLink("self", new System.Uri("http://link.test/123"));
                _fakeUserHelper.WithUserId("45");
                _result = _controller.Post(DictionaryId, _wordView);
            }

            [Fact]
            public void ShouldReturnCreatedResult()
            {
                Assert.IsType<CreatedResult>(_result);
            }

            [Fact]
            public void ShouldReturnNewlyCreatedWordLink()
            {
                var response = _result as CreatedResult;

                Assert.NotNull(response.Location);
            }

            [Fact]
            public void ShouldReturnCreatedWord()
            {
                var response = _result as CreatedResult;

                Assert.NotNull(response.Value);
                Assert.IsType<WordView>(response.Value);
            }
        }

        public class WhenPostingForWordWithNoTitle : TestContext
        {
            private const int DictionaryId = 56;
            private readonly WordView _wordView = new WordView
            {
                Title = "",
                TitleWithMovements = "a^A",
                Pronunciation = "~a",
                Description = "Some description"
            };

            public WhenPostingForWordWithNoTitle()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, WordView>(_wordView);

                _result = _controller.Post(DictionaryId, _wordView);
            }

            [Fact]
            public void ShouldReturnBadRequest()
            {
                Assert.IsType<BadRequestResult>(_result);
            }
        }

        public class WhenPostingInDictionaryWithNoWriteAccess : TestContext
        {
            private const int DictionaryId = 56;
            private readonly WordView _wordView = new WordView
            {
                Title = "a",
                TitleWithMovements = "a^A",
                Pronunciation = "~a",
                Description = "Some description"
            };

            public WhenPostingInDictionaryWithNoWriteAccess()
            {
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, WordView>(null);
                _fakeWordRenderer.WithLink("self", new System.Uri("http://link.test/123"));
                _fakeUserHelper.WithUserId("21");

                _result = _controller.Post(DictionaryId, _wordView);
            }

            [Fact]
            public void ShouldReturnUnauthorised()
            {
                Assert.IsType<UnauthorizedResult>(_result);
            }
        }

        public class WhenUpdatingAWord : TestContext
        {
            public WhenUpdatingAWord()
            {
                int wordId = 434;
                WordView wordView = new WordView
                {
                    Id = wordId,
                    Title = "a",
                    TitleWithMovements = "a^A",
                    Pronunciation = "~a",
                    Description = "Some description"
                };

                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
                _fakeUserHelper.WithUserId("34");
                _result = _controller.Put(wordId, wordView);
            }

            [Fact]
            public void ShouldReturnOk()
            {
                Assert.IsType<NoContentResult>(_result);
            }
        }

        public class WhenUpdatingingNonExisitngWord : TestContext
        {
            public WhenUpdatingingNonExisitngWord()
            {
                int wordId = 434;
                WordView wordView = new WordView
                {
                    Id = wordId,
                    Title = "a",
                    TitleWithMovements = "a^A",
                    Pronunciation = "~a",
                    Description = "Some description"
                };

                _fakeUserHelper.WithUserId("23");

                _result = _controller.Put(wordId, wordView);
            }

            [Fact]
            public void ShouldReturnNotFoundResult()
            {
                Assert.IsType<NotFoundResult>(_result);
            }
        }

        public class WhenUpdatingAWordWithNoTitle : TestContext
        {
            public WhenUpdatingAWordWithNoTitle()
            {
                int wordId = 434;
                WordView wordView = new WordView
                {
                    Id = wordId,
                    Title = " ",
                    TitleWithMovements = "a^A",
                    Pronunciation = "~a",
                    Description = "Some description"
                };

                _result = _controller.Put(wordId, wordView);
            }

            [Fact]
            public void ShouldReturnBadRequest()
            {
                Assert.IsType<BadRequestResult>(_result);
            }
        }

        public class WhenUpdatingAWordInDictionaryWithNoWriteAccess : TestContext
        {
            public WhenUpdatingAWordInDictionaryWithNoWriteAccess()
            {
                int wordId = 434;
                WordView wordView = new WordView
                {
                    Id = wordId,
                    Title = "a",
                    TitleWithMovements = "a^A",
                    Pronunciation = "~a",
                    Description = "Some description"
                };

                _fakeUserHelper.WithUserId("2");

                _result = _controller.Put(wordId, wordView);
            }

            [Fact]
            public void ShouldReturnNotFound()
            {
                Assert.IsType<NotFoundResult>(_result);
            }
        }

        public class WhenDeleteingAWord : TestContext
        {
            public WhenDeleteingAWord()
            {
                _fakeUserHelper.WithUserId("43");
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
                _result = _controller.Delete(34);
            }

            [Fact]
            public void ShouldReturnNoContent()
            {
                Assert.IsType<NoContentResult>(_result);
            }
        }

        public class WhenDeleteingNonExisitngWord : TestContext
        {
            public WhenDeleteingNonExisitngWord()
            {
                _fakeUserHelper.WithUserId("43");

                _result = _controller.Delete(34);
            }
            [Fact]
            public void ShouldReturnNotFound()
            {
                Assert.IsType<NotFoundResult>(_result);
            }
        }

        public class WhenDeleteingAWordFromDictionaryWithNoWriteAccess : TestContext
        {
            public WhenDeleteingAWordFromDictionaryWithNoWriteAccess()
            {
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
                _result = _controller.Delete(34);
            }
            [Fact]
            public void ShouldReturnUnauthorised()
            {
                Assert.IsType<UnauthorizedResult>(_result);
            }
        }

        public abstract class TestContext
        {
            protected readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
            protected FakeWordRenderer _fakeWordRenderer;
            protected FakeQueryProcessor _fakeQueryProcessor;
            protected IActionResult _result;
            protected WordController _controller;
            protected FakeUserHelper _fakeUserHelper;
            private WordIndexPageRenderer _wordIndexPageRenderer;

            protected TestContext()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                _mockCommandProcessor = new Mock<IAmACommandProcessor>();
                _fakeQueryProcessor = new FakeQueryProcessor();
                _fakeWordRenderer = new FakeWordRenderer();
                _fakeUserHelper = new FakeUserHelper();

                _controller = new WordController(_fakeWordRenderer, _mockCommandProcessor.Object, _fakeQueryProcessor, _fakeUserHelper, new FakePageRenderer<Word, WordView>());
            }
        }
    }
}
