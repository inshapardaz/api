using System;
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
    public class WhenGettingPrivateDictionaryAsAnonymous : FunctionTest
    {
        private StatusCodeResult _response;

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
            _response = (StatusCodeResult)await handler.Run(request, dictionary.Id, NullLogger.Instance, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            _response.StatusCode.Should().BeUnauthorized();
        }
    }
}
