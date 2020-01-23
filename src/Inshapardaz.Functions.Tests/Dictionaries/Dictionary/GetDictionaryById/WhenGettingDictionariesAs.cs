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

namespace Inshapardaz.Functions.Tests.Dictionaries.Dictionary.GetDictionaryById
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    [TestFixture(AuthenticationLevel.Reader)]
    [TestFixture(AuthenticationLevel.Unauthorized)]
    public class WhenGettingDictionaryByIdAs : FunctionTest
    {
        private readonly AuthenticationLevel _authenticationLevel;
        private OkObjectResult _response;
        private DictionaryView _view;
        private Ports.Database.Entities.Dictionaries.Dictionary _saved;

        public WhenGettingDictionaryByIdAs(AuthenticationLevel authenticationLevel)
        {
            _authenticationLevel = authenticationLevel;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var builder = Container.GetService<DictionaryDataBuilder>();
            _saved = builder.WithWords(3).Build(4).PickRandom();

            var handler = Container.GetService<Functions.Dictionaries.GetDictionaryById>();
            var principal = AuthenticationBuilder.CreateClaim(_authenticationLevel);
            _response = (OkObjectResult)await handler.Run(request, _saved.Id, NullLogger.Instance, principal, CancellationToken.None);

            _view = _response.Value as DictionaryView;
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
            _view.SelfLink().ShouldGet($"/api/dictionaries/{_saved.Id}");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            if (_authenticationLevel.CanEdit())
            {
                _view.CreateWordLink().ShouldPost($"/api/dictionaries/{_saved.Id}/words");
            }
            else
            {
                _view.CreateWordLink().Should().BeNull();
            }
        }

        [Test]
        public void ShouldReturnCorrectDictionaries()
        {
            var expected = new DictionaryView
            {
                Id = _saved.Id,
                Name = _saved.Name,
                Language = _saved.Language,
                LanguageId = (int)_saved.Language,
                IsPublic = _saved.IsPublic,
                WordCount = _saved.Word.Count()
            };

            _view.Should().BeEquivalentTo(expected, opt => opt.Excluding(d => d.Links));

            _view.SelfLink().ShouldGet($"/api/dictionaries/{_saved.Id}");
            if (_authenticationLevel.CanEdit())
            {
                _view.UpdateLink().ShouldPut($"/api/dictionaries/{_saved.Id}");
                _view.DeleteLink().ShouldDelete($"/api/dictionaries/{_saved.Id}");
            }
            else
            {
                _view.UpdateLink().Should().BeNull();
                _view.DeleteLink().Should().BeNull();
            }
        }
    }
}
