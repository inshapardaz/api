using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.AddIssue
{
    [TestFixture]
    public class WhenAddingIssueAsUnauthorizedUser
        : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();

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
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
