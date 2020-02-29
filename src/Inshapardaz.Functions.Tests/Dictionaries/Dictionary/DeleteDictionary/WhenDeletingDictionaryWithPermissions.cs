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

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.DeleteDictionary
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingDictionaryWithPermissions : FunctionTest
    {
        private OkResult _response;
        private DictionaryDataBuilder _builder;
        private DictionaryEditView _dictionary;
        private Ports.Database.Entities.Dictionaries.Dictionary _actual;
        private readonly ClaimsPrincipal _claim;

        public WhenDeletingDictionaryWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<DictionaryDataBuilder>();
            var dataHelper = Container.GetService<DictionaryDataHelper>();
            var handler = Container.GetService<Functions.Dictionaries.DeleteDictionary>();
            var dictionary = _builder.Build();
            _dictionary = new DictionaryEditView
            {
                Name = Random.Name,
                IsPublic = Random.Bool,
                LanguageId = (int)new Faker().PickRandom<Languages>()
            };

            var request = new RequestBuilder().Build();

            _response = (OkResult)await handler.Run(request, dictionary.Id, _claim, CancellationToken.None);
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
            _response.Should().BeOk();
        }

        [Test]
        public void ShouldHaveDeletedTheDictionary()
        {
            _actual.Should().BeNull();
        }
    }
}
