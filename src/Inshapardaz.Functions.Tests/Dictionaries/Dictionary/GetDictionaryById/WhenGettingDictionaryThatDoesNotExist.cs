using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Dictionaries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.GetDictionaryById
{
    [TestFixture]
    public class WhenGettingDictionaryThatDoesNotExist : FunctionTest
    {
        private StatusCodeResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var handler = Container.GetService<Functions.Dictionaries.GetDictionaryById>();
            _response = (StatusCodeResult)await handler.Run(request, int.MinValue, NullLogger.Instance, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.StatusCode.Should().BeNotFound();
        }
    }
}
