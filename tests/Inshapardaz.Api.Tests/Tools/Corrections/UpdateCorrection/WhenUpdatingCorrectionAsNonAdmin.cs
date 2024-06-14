using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.UpdateCorrection
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenUpdatingCorrectionAsNonAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private CorrectionAssert _assert;
        private CorrectionDto _correction;
        private CorrectionView _update;

        public WhenUpdatingCorrectionAsNonAdmin(Role role)
            :base(role)
        {

        }

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
            _assert = CorrectionAssert.WithResponse(_response);
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
        public void ShouldHaveNotUpdatedCorrection()
        {
            _assert.ShouldMatchSavedCorrection(DatabaseConnection, _correction);
        }
    }
}
