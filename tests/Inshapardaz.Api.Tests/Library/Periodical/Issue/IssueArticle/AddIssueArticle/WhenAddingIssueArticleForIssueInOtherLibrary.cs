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

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.AddIssueArticle
{
    [TestFixture]
    public class WhenAddingIssueArticleForIssueInOtherLibrary
        : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryDataBuilder _libBuilder;

        public WhenAddingIssueArticleForIssueInOtherLibrary()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _libBuilder = Services.GetService<LibraryDataBuilder>();

            var library2 = _libBuilder.Build();
            var issue = IssueBuilder.WithLibrary(library2.Id).Build();

            var article = new IssueArticleView { Title = new Faker().Random.String(), SequenceNumber = 1 };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles", article);
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
