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

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.GetDictionaries
{
    public class WhenGettingDictionariesAsUnauthorised : FunctionTest
    {
        private OkObjectResult _response;
        private ListView<DictionaryView> _view;
        private IEnumerable<Ports.Database.Entities.Dictionaries.Dictionary> _saved;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var builder = Container.GetService<DictionaryDataBuilder>();
            _saved = builder.WithWords(3)
                            .AsPrivate()
                            .Build(4);

            var handler = Container.GetService<Functions.Dictionaries.GetDictionaries>();
            _response = (OkObjectResult)await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as ListView<DictionaryView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.StatusCode.Should().BeOk();
        }

        [Test]
        public void ShouldReturnNoDictionaries()
        {
            _view.Items.Should().BeEmpty();
        }
    }
}
