﻿using System.Net.Http;
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
    public class WhenUpdatingCorrectionAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private CorrectionAssert _assert;
        private CorrectionDto _correction;
        private CorrectionView _update;

        public WhenUpdatingCorrectionAdmin()
            :base(Role.Admin)
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
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedCorrection()
        {
            _assert.ShouldMatchSavedCorrection(DatabaseConnection, new CorrectionDto
            {
                Id = _correction.Id,
                Language = _correction.Language,
                Profile = _correction.Profile,
                IncorrectText = _update.IncorrectText,
                CorrectText = _update.CorrectText,
                CompleteWord = _update.CompleteWord
            });
        }
    }
}
