using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.DeleteCorrection
{
    [TestFixture]
    public class WhenDeletingCorrectionWithIncorrectLanguage() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;
        private CorrectionAssert _assert;
        private CorrectionDto _correction;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _correction = CorrectionBuilder.Build();

            _response = await Client.DeleteAsync($"/tools/{_correction.Language}-2/corrections/{_correction.Profile}/{_correction.Id}");
            _assert = Services.GetService<CorrectionAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveDeletedAuthor() => _assert.ShouldNotHaveDeletedCorrection(_correction.Id);
    }
}
