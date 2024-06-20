using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.DeleteCorrection
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenDeletingCorrectionAsNonAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private CorrectionAssert _assert;
        private CorrectionDto _correction;

        public WhenDeletingCorrectionAsNonAdmin(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _correction = CorrectionBuilder.Build();

            _response = await Client.DeleteAsync($"/tools/{_correction.Language}/corrections/{_correction.Profile}/{_correction.Id}");
            _assert = Services.GetService<CorrectionAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnForbidden()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldHaveDeletedAuthor()
        {
            _assert.ShouldNotHaveDeletedCorrection(_correction.Id);
        }
    }
}
