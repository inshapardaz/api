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
    public class WhenAddingDictionaryAsReader : FunctionTest
    {
        private ForbidResult _response;
        private DictionaryEditView _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Dictionaries.AddDictionary>();
            _dictionary = new DictionaryEditView
            {
                Name = Random.Name,
                IsPublic = Random.Bool,
                LanguageId = (int)new Faker().PickRandom<Languages>()
            };

            _response = (ForbidResult)await handler.Run(_dictionary.ToRequest(), AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.Should().BeForbidden();
        }
    }
}
