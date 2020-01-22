using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Dictionaries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.GetDictionaries
{
    [TestFixture]
    public class WhenGettingDictionariesAndThereAreNoDictionaries : FunctionTest
    {
        private OkObjectResult _response;
        private ListView<DictionaryView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var handler = Container.GetService<Functions.Dictionaries.GetDictionaries>();
            _response = (OkObjectResult)await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as ListView<DictionaryView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink().ShouldGet("/api/dictionaries");
        }

        [Test]
        public void ShouldReturnNoDictionaries()
        {
            _view.Items.Should().BeEmpty();
        }
    }
}
