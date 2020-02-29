using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Dictionaries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.AddDictionary
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenAddingDictionaryWithPermissions : FunctionTest
    {
        private CreatedResult _response;
        private DictionaryDataBuilder _builder;
        private DictionaryView _dictionary;
        private DictionaryView _payload;
        private Ports.Database.Entities.Dictionaries.Dictionary _actual;
        private readonly ClaimsPrincipal _claim;

        public WhenAddingDictionaryWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<DictionaryDataBuilder>();
            var dataHelper = Container.GetService<DictionaryDataHelper>();
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

            _response = (CreatedResult)await handler.Run(request, _claim, CancellationToken.None);
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
            _response.Location.Should().MatchRegex($"/api/dictionaries/{_payload.Id}");
        }

        [Test]
        public void ShouldHaveCreatedTheDictionary()
        {
            _actual.Name.Should().Be(_dictionary.Name);
            _actual.IsPublic.Should().Be(_dictionary.IsPublic);
            _actual.Language.Should().Be((Languages)_dictionary.LanguageId);
            _actual.UserId.Should().Be(_claim.GetUserId());
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
