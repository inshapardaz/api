using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.GetDictionaryById
{
    [TestFixture]
    public class WhenGettingPrivateDictionaryAsDifferentUser : FunctionTest
    {
        private IActionResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var builder = Container.GetService<DictionaryDataBuilder>();
            var dictionary = builder.WithWords(3)
                                    .AsPrivate()
                                    .ForUser(Guid.NewGuid())
                                    .Build(4).PickRandom();

            var handler = Container.GetService<Functions.Dictionaries.GetDictionaryById>();
            _response = await handler.Run(request, dictionary.Id, NullLogger.Instance, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            _response.Should().BeForbidden();
        }
    }
}
