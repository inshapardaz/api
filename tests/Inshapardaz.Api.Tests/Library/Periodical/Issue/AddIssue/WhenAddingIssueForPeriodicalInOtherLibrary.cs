using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.AddIssue
{
    [TestFixture]
    public class WhenAddingIssueForPeriodicalInOtherLibrary
        : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryDataBuilder _libBuilder;

        public WhenAddingIssueForPeriodicalInOtherLibrary()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _libBuilder = Services.GetService<LibraryDataBuilder>();

            var library2 = _libBuilder.Build();
            var periodical = PeriodicalBuilder.WithLibrary(library2.Id).Build();

            var issue = new IssueView
            {
                VolumeNumber = new Faker().Random.Number(1, 100),
                IssueNumber = new Faker().Random.Number(1, 100),
                IssueDate = new Faker().Date.Past()
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{periodical.Id}/issues", issue);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _libBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
