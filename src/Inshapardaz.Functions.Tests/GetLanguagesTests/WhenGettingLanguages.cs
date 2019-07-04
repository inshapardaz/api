using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.GetLanguagesTests
{
    [TestFixture]
    public class WhenGettingLanguages : FunctionTest
    {
        OkObjectResult _response;
        List<KeyValuePair<string, int>> _view;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var handler = Container.GetService<GetLanguages>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance);

            _view = _response.Value as List<KeyValuePair<string, int>>;
        }

        [Test]
        public void ShouldHaveOkResult()
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