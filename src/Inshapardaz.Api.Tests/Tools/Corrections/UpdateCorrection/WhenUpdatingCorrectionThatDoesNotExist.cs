using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.UpdateCorrection
{
    [TestFixture]
    public class WhenUpdatingCorrectionThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private CorrectionAssert _assert;
        private CorrectionView _correction;
        private readonly string _language = RandomData.Locale;
        private readonly string _profile = RandomData.Words(1);
        public WhenUpdatingCorrectionThatDoesNotExist()
            :base(Role.Admin)
        {

        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _correction = new CorrectionView
            {
                IncorrectText = RandomData.Words(2),
                CorrectText = RandomData.Words(2),
                CompleteWord = true
            };

            _response = await Client.PutObject($"/tools/{_language}/corrections/{_profile}/{RandomData.Number}", _correction);
            _assert = CorrectionAssert.WithResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnCreated()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveCorrectObjectReturned()
        {
            _assert.ShouldBeSameAs(new CorrectionDto
            {
                Language = _language,
                Profile = _profile,
                IncorrectText = _correction.IncorrectText,
                CorrectText = _correction.CorrectText,
                CompleteWord = _correction.CompleteWord
            });
        }


        [Test]
        public void ShouldHaveCorrectObjectSaved()
        {
            _assert.ShouldMatchSavedCorrection(DatabaseConnection, new CorrectionDto
            {
                Id = _assert.View.Id,
                Language = _language,
                Profile = _profile,
                IncorrectText = _correction.IncorrectText,
                CorrectText = _correction.CorrectText,
                CompleteWord = _correction.CompleteWord
            });
        }
    }
}
