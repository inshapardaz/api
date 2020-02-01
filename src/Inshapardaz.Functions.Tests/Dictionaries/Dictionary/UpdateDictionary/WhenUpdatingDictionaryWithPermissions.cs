using System.Net;
using System.Security.Claims;
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

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.UpdateDictionary
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUpdatingDictionaryWithPermissions : FunctionTest
    {
        private OkObjectResult _response;
        private DictionaryDataBuilder _builder;
        private DictionaryEditView _dictionary;
        private Ports.Database.Entities.Dictionaries.Dictionary _actual;
        private readonly ClaimsPrincipal _claim;

        public WhenUpdatingDictionaryWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<DictionaryDataBuilder>();
            var dataHelper = Container.GetService<DictionaryDataHelper>();
            var handler = Container.GetService<Functions.Dictionaries.UpdateDictionary>();
            var dictionary = _builder.Build();
            _dictionary = new DictionaryEditView
            {
                Name = Random.Name,
                IsPublic = Random.Bool,
                LanguageId = (int)new Faker().PickRandom<Languages>()
            };

            var request = new RequestBuilder()
                                            .WithJsonBody(_dictionary)
                                            .Build();

            _response = (OkObjectResult)await handler.Run(request, dictionary.Id, _claim, CancellationToken.None);
            _actual = dataHelper.GetDictionaryByid(dictionary.Id);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.StatusCode.Should().BeOk();
        }

        [Test]
        public void ShouldHaveUpdatedTheDictionary()
        {
            _actual.Name.Should().Be(_dictionary.Name);
            _actual.IsPublic.Should().Be(_dictionary.IsPublic);
            _actual.Language.Should().Be((Languages)_dictionary.LanguageId);
        }
    }
}
