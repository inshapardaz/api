using Inshapardaz.Api.Tests;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tools.Corrections.AddCorrection
{
    [TestFixture]
    public class WhenAddingCorrectionAsAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private CorrectionAssert _assert;
        private CorrectionView _correction;


        public WhenAddingCorrectionAsAdmin()
            :base(Role.Admin)
        { }
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            _correction = CorrectionBuilder.BuildCorrection();

            _response = await Client.PostObject($"/tools/{_correction.Language}/corrections/{_correction.Profile}", _correction);
            _assert = Services.GetService<CorrectionAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldBeCreated()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldReturnCorrectObject()
        {
            _assert.ShouldBeSameAs(_correction);
        }

        [Test]
        public void ShouldHaveSavedCorrection()
        {
            _assert.ShouldHaveSavedCorrection();
        }
    }
}
