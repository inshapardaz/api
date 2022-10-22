using Inshapardaz.Api.Tests;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tools.Corrections.AddCorrection
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingCorrectionAsNonAdmin : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingCorrectionAsNonAdmin(Role role)
            :base(role)
        { }
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var correction = CorrectionBuilder.BuildCorrection();

            _response = await Client.PostObject($"/tools/en/corrections/test", correction);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldBeForbidden()
        {
            _response.ShouldBeForbidden();
        }
    }
}
