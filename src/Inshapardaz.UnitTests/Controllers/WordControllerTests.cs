using AutoMapper;
using Inshapardaz.Configuration;
using Inshapardaz.Controllers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Model;
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
        public class WhenPostingForWord
        {
            private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
            private FakeWordRenderer _fakeWordRenderer;
            private FakeQueryProcessor _fakeQueryProcessor;
            private IActionResult _result;

            private int _dictionaryId = 56;
            private WordView _wordView = new WordView
            {
                Title = "a",
                TitleWithMovements = "a^A",
                Pronunciation = "~a",
                Description = "Some description"
            };

            public WhenPostingForWord()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                _mockCommandProcessor = new Mock<IAmACommandProcessor>();
                _fakeQueryProcessor = new FakeQueryProcessor();
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, WordView>(_wordView);
                _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary());
                _fakeWordRenderer = new FakeWordRenderer();
                _fakeWordRenderer.WithLink("self", new System.Uri("http://link.test/123"));

                var controller = new WordController(_fakeWordRenderer, _mockCommandProcessor.Object, _fakeQueryProcessor, new FakeUserHelper());
                               
                _result = controller.Post(_dictionaryId, _wordView);
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

        public class WhenPostingForWordWithNoTitle
        {
            private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
            private FakeWordRenderer _fakeWordRenderer;
            private FakeQueryProcessor _fakeQueryProcessor;
            private IActionResult _result;

            private int _dictionaryId = 56;
            private WordView _wordView = new WordView
            {
                Title = "",
                TitleWithMovements = "a^A",
                Pronunciation = "~a",
                Description = "Some description"
            };

            public WhenPostingForWordWithNoTitle()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                _mockCommandProcessor = new Mock<IAmACommandProcessor>();
                _fakeQueryProcessor = new FakeQueryProcessor();
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, WordView>(_wordView);
                _fakeWordRenderer = new FakeWordRenderer();

                var controller = new WordController(_fakeWordRenderer, _mockCommandProcessor.Object, _fakeQueryProcessor, new FakeUserHelper());

                _result = controller.Post(_dictionaryId, _wordView);
            }

            [Fact]
            public void ShouldReturnBadRequest()
            {
                Assert.IsType<BadRequestResult>(_result);
            }
        }

        public class WhenPostingForOtherUserPrivatesDictionary
        {
            private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
            private FakeWordRenderer _fakeWordRenderer;
            private FakeQueryProcessor _fakeQueryProcessor;
            private IActionResult _result;

            private int _dictionaryId = 56;
            private WordView _wordView = new WordView
            {
                Title = "a",
                TitleWithMovements = "a^A",
                Pronunciation = "~a",
                Description = "Some description"
            };

            public WhenPostingForOtherUserPrivatesDictionary()
            {
                Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

                _mockCommandProcessor = new Mock<IAmACommandProcessor>();
                _fakeQueryProcessor = new FakeQueryProcessor();
                _fakeQueryProcessor.SetupResultFor<WordByIdQuery, WordView>(null);
                _fakeWordRenderer = new FakeWordRenderer();
                _fakeWordRenderer.WithLink("self", new System.Uri("http://link.test/123"));
                var fakeUserHelper = new FakeUserHelper().WithUserId("21");

                var controller = new WordController(_fakeWordRenderer, _mockCommandProcessor.Object, _fakeQueryProcessor, fakeUserHelper);

                _result = controller.Post(_dictionaryId, _wordView);
            }

            [Fact]
            public void ShouldReturnUnauthorised()
            {
                Assert.IsType<UnauthorizedResult>(_result);
            }
        }
    }
}
