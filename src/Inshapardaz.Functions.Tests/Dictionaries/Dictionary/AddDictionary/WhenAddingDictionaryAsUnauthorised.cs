using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.DataBuilders;
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
        private DictionaryDataBuilder _builder;
        private DictionaryView _dictionary;
        private Ports.Database.Entities.Dictionaries.Dictionary _actual;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<DictionaryDataBuilder>();
            var handler = Container.GetService<Functions.Dictionaries.AddDictionary>();
            _dictionary = new DictionaryView
            {
                Name = Random.Name,
                IsPublic = Random.Bool,
                LanguageId = (int)new Faker().PickRandom<Languages>()
            };

            var request = new RequestBuilder()
                                            .WithJsonBody(_dictionary)
                                            .Build();

            _response = (UnauthorizedResult)await handler.Run(request, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.Should().BeUnauthorized();
        }
    }
}
