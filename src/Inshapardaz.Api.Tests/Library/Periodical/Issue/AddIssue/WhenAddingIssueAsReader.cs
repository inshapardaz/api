using Bogus;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.AddIssue
{
    [TestFixture]
    public class WhenAddingIssueAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingIssueAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();

            var issue = new IssueView {
                VolumeNumber = new Faker().Random.Number(1, 100),
                IssueNumber = new Faker().Random.Number(1, 100),
                IssueDate = new Faker().Date.Past()
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{periodical.Id}/issues", issue);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
