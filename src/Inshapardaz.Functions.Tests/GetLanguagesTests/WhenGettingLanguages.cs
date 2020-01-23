using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.GetLanguagesTests
{
    [TestFixture]
    public class WhenGettingLanguages : FunctionTest
    {
        private OkObjectResult _response;
        private List<KeyValuePair<string, int>> _view;
        
        [OneTimeSetUp]
        public void Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var handler = Container.GetService<GetLanguages>();
            _response = (OkObjectResult) handler.Run(request, NullLogger.Instance);

            _view = _response.Value as List<KeyValuePair<string, int>>;
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldReturnSomeLanguages()
        {
            Assert.IsNotEmpty(_view, "Should return some languages.");
        }
    }
}
