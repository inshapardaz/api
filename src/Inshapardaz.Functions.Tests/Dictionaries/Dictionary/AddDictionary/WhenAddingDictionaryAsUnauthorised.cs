using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Dictionaries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.AddDictionary
{
    [TestFixture]
    public class WhenAddingDictionaryAsUnauthorised : FunctionTest
    {
        private UnauthorizedResult _response;
        private DictionaryView _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Dictionaries.AddDictionary>();
            _dictionary = new DictionaryView
            {
                Name = Random.Name,
                IsPublic = Random.Bool,
                LanguageId = (int)new Faker().PickRandom<Languages>()
            };

            _response = (UnauthorizedResult)await handler.Run(_dictionary.ToRequest(), AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisezResult()
        {
            _response.Should().BeUnauthorized();
        }
    }
}
