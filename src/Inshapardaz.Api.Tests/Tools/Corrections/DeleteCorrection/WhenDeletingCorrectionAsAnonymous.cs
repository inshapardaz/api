using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.DeleteCorrection
{
    [TestFixture]
    public class WhenDeletingCorrectionAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;

        private CorrectionDto _correction;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _correction = CorrectionBuilder.Build();

            _response = await Client.DeleteAsync($"/tools/{_correction.Language}/corrections/{_correction.Profile}/{_correction.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldHaveDeletedAuthor()
        {
            CorrectionAssert.ShouldNotHaveDeletedCorrection(_correction.Id, DatabaseConnection);
        }
    }
}
