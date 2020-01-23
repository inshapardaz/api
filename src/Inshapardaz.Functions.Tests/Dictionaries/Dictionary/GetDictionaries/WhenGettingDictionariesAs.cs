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
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    [TestFixture(AuthenticationLevel.Reader)]
    [TestFixture(AuthenticationLevel.Unauthorized)]
    public class WhenGettingDictionariesAs : FunctionTest
    {
        private readonly AuthenticationLevel _authenticationLevel;
        private OkObjectResult _response;
        private ListView<DictionaryView> _view;
        private IEnumerable<Ports.Database.Entities.Dictionaries.Dictionary> _saved;

        public WhenGettingDictionariesAs(AuthenticationLevel authenticationLevel)
        {
            _authenticationLevel = authenticationLevel;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var builder = Container.GetService<DictionaryDataBuilder>();
            _saved = builder.WithWords(3).Build(4);

            var handler = Container.GetService<Functions.Dictionaries.GetDictionaries>();
            var principal = AuthenticationBuilder.CreateClaim(_authenticationLevel);
            _response = (OkObjectResult)await handler.Run(request, NullLogger.Instance, principal, CancellationToken.None);

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
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink().ShouldGet("/api/dictionaries");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            if (_authenticationLevel.CanEdit())
            {
                _view.CreateLink().ShouldPost("/api/dictionaries");
            }
            else
            {
                _view.CreateLink().Should().BeNull();
            }
        }

        [Test]
        public void ShouldReturnCorrectDictionaries()
        {
            _view.Items.Should().HaveSameCount(_saved);

            foreach (var expected in _saved)
            {
                var actual = _view.Items.SingleOrDefault(d => d.Name == expected.Name);

                actual.Should().NotBeNull();
                actual.IsPublic.Should().Be(expected.IsPublic);
                actual.Language.Should().Be(expected.Language);
                actual.WordCount.Should().Be(expected.Word.Count());

                actual.SelfLink().ShouldGet($"/api/dictionaries/{actual.Id}");

                if (_authenticationLevel.CanEdit())
                {
                    actual.UpdateLink().ShouldPut($"/api/dictionaries/{actual.Id}");
                    actual.DeleteLink().ShouldDelete($"/api/dictionaries/{actual.Id}");
                }
                else
                {
                    actual.UpdateLink().Should().BeNull();
                    actual.DeleteLink().Should().BeNull();
                }
            }
        }
    }
}
