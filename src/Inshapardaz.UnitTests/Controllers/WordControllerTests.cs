using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.UnitTests.Fakes;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using paramore.brighter.commandprocessor;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class WhenGettingWordsForDictionary : WordControllerTestContext
    {
        public WhenGettingWordsForDictionary()
        {
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());
            _fakeQueryProcessor.SetupResultFor<WordQuery, Page<Word>>(new Page<Word>());
            _result = _controller.Get(12, 1).Result;
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

    public class WhenGettingWordsForPrivateDictionaryOfOthers : WordControllerTestContext
    {
        public WhenGettingWordsForPrivateDictionaryOfOthers()
        {
            _fakeUserHelper.WithUserId("1");
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary
            {
                UserId = "2",
                IsPublic = false
            });

            _result = _controller.Get(12, 1).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(_result);
        }
    }

    public class WhenGettingWordsForPublicDictionaryOfOthers : WordControllerTestContext
    {
        public WhenGettingWordsForPublicDictionaryOfOthers()
        {
            _fakeUserHelper.WithUserId("1");
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary
            {
                UserId = "2",
                IsPublic = true
            });

            _fakeQueryProcessor.SetupResultFor<WordQuery, Page<Word>>(new Page<Word>());
            _result = _controller.Get(12, 1).Result;
        }

        [Fact]
        public void ShouldReturnOk()
        {
            Assert.IsType<OkObjectResult>(_result);
        }
    }

    public class WhenGettingWordsForDictionaryThatDoesNotExist : WordControllerTestContext
    {
        public WhenGettingWordsForDictionaryThatDoesNotExist()
        {
            _fakeUserHelper.WithUserId("1");
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(null);
            _result = _controller.Get(12, 1).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(_result);
        }
    }

    public class WhenGettingWordById : WordControllerTestContext
    {
        public WhenGettingWordById()
        {
            _fakeUserHelper.AsContributor();
            _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
            _result = _controller.Get(34).Result;
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

    public class WhenGettingNonExistingWordById : WordControllerTestContext
    {
        public WhenGettingNonExistingWordById()
        {
            _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(null);
            _result = _controller.Get(34).Result;
        }

        [Fact]
        public void ShouldReturnNotFoundResult()
        {
            Assert.IsType<NotFoundResult>(_result);
        }
    }

    public class WhenGettingWordBelongingToPrivateDictionaryWithNoAccess : WordControllerTestContext
    {
        public WhenGettingWordBelongingToPrivateDictionaryWithNoAccess()
        {
            _fakeUserHelper.WithUserId("32");
            _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(null);
            _result = _controller.Get(34).Result;
        }

        [Fact]
        public void ShouldQueryForWordWithCorrectUser()
        {
            _fakeQueryProcessor.AssertQueryExecuted<WordByIdQuery>(q => q.UserId == "32");
        }
    }

    public class WhenPostingForWord : WordControllerTestContext
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
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());
            _fakeWordRenderer.WithLink("self", new System.Uri("http://link.test/123"));
            _fakeUserHelper.WithUserId("45");
            _result = _controller.Post(DictionaryId, _wordView).Result;
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
    
    public class WhenPostingInDictionaryWithNoWriteAccess : WordControllerTestContext
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

            _result = _controller.Post(DictionaryId, _wordView).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(_result);
        }
    }

    public class WhenUpdatingAWord : WordControllerTestContext
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
            _result = _controller.Put(wordId, wordView).Result;
        }

        [Fact]
        public void ShouldReturnOk()
        {
            Assert.IsType<NoContentResult>(_result);
        }
    }

    public class WhenUpdatingingNonExisitngWord : WordControllerTestContext
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

            _result = _controller.Put(wordId, wordView).Result;
        }

        [Fact]
        public void ShouldReturnNotFoundResult()
        {
            Assert.IsType<NotFoundResult>(_result);
        }
    }
    
    public class WhenDeleteingAWord : WordControllerTestContext
    {
        public WhenDeleteingAWord()
        {
            _fakeUserHelper.WithUserId("43");
            _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
            _result = _controller.Delete(34).Result;
        }

        [Fact]
        public void ShouldReturnNoContent()
        {
            Assert.IsType<NoContentResult>(_result);
        }
    }

    public class WhenDeleteingNonExisitngWord : WordControllerTestContext
    {
        public WhenDeleteingNonExisitngWord()
        {
            _fakeUserHelper.WithUserId("43");

            _result = _controller.Delete(34).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(_result);
        }
    }

    public class WhenDeleteingAWordFromDictionaryWithNoWriteAccess : WordControllerTestContext
    {
        public WhenDeleteingAWordFromDictionaryWithNoWriteAccess()
        {
            _fakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
            _result = _controller.Delete(34).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(_result);
        }
    }

    public class WhenUpdatingAWordInDictionaryWithNoWriteAccess : WordControllerTestContext
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

            _result = _controller.Put(wordId, wordView).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(_result);
        }
    }

    public abstract class WordControllerTestContext
    {
        protected readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
        protected FakeWordRenderer _fakeWordRenderer;
        protected FakeQueryProcessor _fakeQueryProcessor;
        protected IActionResult _result;
        protected WordController _controller;
        protected FakeUserHelper _fakeUserHelper;

        protected WordControllerTestContext()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

            _mockCommandProcessor = new Mock<IAmACommandProcessor>();
            _fakeQueryProcessor = new FakeQueryProcessor();
            _fakeWordRenderer = new FakeWordRenderer();
            _fakeUserHelper = new FakeUserHelper();

            _controller = new WordController(_fakeWordRenderer, _mockCommandProcessor.Object, _fakeQueryProcessor,
                _fakeUserHelper, new FakePageRenderer<Word, WordView>());
        }
    }
}