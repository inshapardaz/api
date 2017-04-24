using AutoMapper;
using Inshapardaz.Configuration;
using Inshapardaz.Controllers;
using Inshapardaz.Model;
using Inshapardaz.UnitTests.Fakes;
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
                _fakeWordRenderer = new FakeWordRenderer();
                _fakeWordRenderer.WithLink("self", new System.Uri("http://link.test/123"));

                var controller = new WordController(_fakeWordRenderer, _mockCommandProcessor.Object, _fakeQueryProcessor);
                               
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
    }
}
