using Inshapardaz.Api.Tests;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tools.Corrections.AddCorrection
{
    [TestFixture]
    public class WhenAddingCorrectionAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var correction = CorrectionBuilder.BuildCorrection();

            _response = await Client.PostObject($"/tools/en/corrections/test", correction);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }
}
