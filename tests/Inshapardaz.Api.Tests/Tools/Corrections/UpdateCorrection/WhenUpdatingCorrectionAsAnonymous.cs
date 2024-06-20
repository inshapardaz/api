using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.UpdateCorrection
{
    [TestFixture]
    public class WhenUpdatingCorrectionAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;
        private CorrectionAssert _assert;
        private CorrectionDto _correction;
        private CorrectionView _update;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _correction = CorrectionBuilder.Build();
            _update = new CorrectionView
            {
                IncorrectText = _correction.IncorrectText + "2",
                CorrectText = _correction.CorrectText + "2",
                CompleteWord = !_correction.CompleteWord
            };

            _response = await Client.PutObject($"/tools/{_correction.Language}/corrections/{_correction.Profile}/{_correction.Id}", _update);
            _assert = Services.GetService<CorrectionAssert>().ForResponse(_response);
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
        public void ShouldHaveNotUpdatedCorrection()
        {
            _assert.ShouldMatchSavedCorrection(_correction);
        }
    }
}
