using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Dictionaries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.UpdateDictionary
{
    public class WhenUpdatingDictionaryThatDoesNotExist : FunctionTest
    {
        private CreatedResult _response;
        private DictionaryView _payload;
        private DictionaryEditView _dictionary;
        private Ports.Database.Entities.Dictionaries.Dictionary _actual;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataHelper = Container.GetService<DictionaryDataHelper>();
            var handler = Container.GetService<Functions.Dictionaries.UpdateDictionary>();
            _dictionary = new DictionaryEditView
            {
                Name = Random.Name,
                IsPublic = Random.Bool,
                LanguageId = (int)new Faker().PickRandom<Languages>()
            };

            var request = new RequestBuilder()
                                            .WithJsonBody(_dictionary)
                                            .Build();

            _response = (CreatedResult)await handler.Run(request, Random.Number, AuthenticationBuilder.AdminClaim, CancellationToken.None);
            _payload = (DictionaryView)_response.Value;
            _actual = dataHelper.GetDictionaryByid(_payload.Id);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.Should().BeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _response.Location.Should().MatchRegex("/api/dictionaries/[0-9]+");
        }

        [Test]
        public void ShouldHaveCreatedTheDictionary()
        {
            _actual.Name.Should().Be(_dictionary.Name);
            _actual.IsPublic.Should().Be(_dictionary.IsPublic);
            _actual.Language.Should().Be((Languages)_dictionary.LanguageId);
        }

        [Test]
        public void ShouldHaveCorrectBodyInResponse()
        {
            _payload.Name.Should().Be(_dictionary.Name);
            _payload.IsPublic.Should().Be(_dictionary.IsPublic);
            _payload.Language.Should().Be((Languages)_dictionary.LanguageId);
            _payload.SelfLink().ShouldGet($"/api/dictionaries/{_payload.Id}");
            _payload.UpdateLink().ShouldPut($"/api/dictionaries/{_payload.Id}");
            _payload.DeleteLink().ShouldDelete($"/api/dictionaries/{_payload.Id}");
            _payload.Link(RelTypes.CreateWord).ShouldPost($"/api/dictionaries/{_payload.Id}/words");
            _payload.Link(RelTypes.CreateDownload).ShouldPost($"/api/dictionaries/{_payload.Id}/download");
        }
    }
}
