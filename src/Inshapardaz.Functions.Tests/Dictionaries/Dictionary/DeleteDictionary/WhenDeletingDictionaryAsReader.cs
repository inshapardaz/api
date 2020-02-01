using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.DeleteDictionary
{
    [TestFixture]
    public class WhenDeletingDictionaryAsReader : FunctionTest
    {
        private ForbidResult _response;
        private DictionaryDataBuilder _builder;
        private Ports.Database.Entities.Dictionaries.Dictionary _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<DictionaryDataBuilder>();
            var handler = Container.GetService<Functions.Dictionaries.DeleteDictionary>();
            _dictionary = _builder.Build();

            var request = new RequestBuilder().Build();

            _response = (ForbidResult)await handler.Run(request, _dictionary.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
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
