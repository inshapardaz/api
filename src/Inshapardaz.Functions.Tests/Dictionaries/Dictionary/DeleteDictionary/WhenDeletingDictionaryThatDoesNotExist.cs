using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.DeleteDictionary
{
    [TestFixture]
    public class WhenDeletingDictionaryThatDoesNotExist : FunctionTest
    {
        private OkResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Dictionaries.DeleteDictionary>();
            var request = new RequestBuilder().Build();
            _response = (OkResult)await handler.Run(request, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.Should().BeOk();
        }
    }
}
