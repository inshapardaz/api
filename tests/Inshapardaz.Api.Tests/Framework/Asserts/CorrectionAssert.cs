using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using System.Data;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    internal class CorrectionAssert
    {
        private CorrectionView _correction;

        public HttpResponseMessage _response;

        public CorrectionView View => _correction;
        private CorrectionAssert(HttpResponseMessage response)
        {
            _response = response;
            _correction = response.GetContent<CorrectionView>().Result;
        }

        private CorrectionAssert(CorrectionView view)
        {
            _correction = view;
        }

        public static CorrectionAssert WithResponse(HttpResponseMessage response)
        {
            return new CorrectionAssert(response);
        }

        internal CorrectionAssert ShouldBeSameAs(CorrectionDto expected)
        {
            _correction.Should().NotBeNull();
            _correction.Language.Should().Be(expected.Language);
            _correction.Profile.Should().Be(expected.Profile);
            _correction.IncorrectText.Should().Be(expected.IncorrectText);
            _correction.CorrectText.Should().Be(expected.CorrectText);
            _correction.CompleteWord.Should().Be(expected.CompleteWord);
            return this;
        }

        internal CorrectionAssert ShouldBeSameAs(CorrectionView expected)
        {
            _correction.Should().NotBeNull();
            _correction.Language.Should().Be(expected.Language);
            _correction.Profile.Should().Be(expected.Profile);
            _correction.IncorrectText.Should().Be(expected.IncorrectText);
            _correction.CorrectText.Should().Be(expected.CorrectText);
            _correction.CompleteWord.Should().Be(expected.CompleteWord);
            return this;
        }

        internal static CorrectionAssert FromObject(CorrectionView correction)
        {
            return new CorrectionAssert(correction);
        }

        internal static void ShouldHaveDeletedCorrection(long correctionId, IDbConnection dbConnection)
        {
            var correction = dbConnection.GetCorrectionById(correctionId);
            correction.Should().BeNull();
        }

        internal static void ShouldNotHaveDeletedCorrection(long correctionId, IDbConnection dbConnection)
        {
            var correction = dbConnection.GetCorrectionById(correctionId);
            correction.Should().NotBeNull();
        }

        public CorrectionAssert ShouldNotHaveDeleteLink()
        {
            _correction.DeleteLink().Should().BeNull();

            return this;
        }

        public CorrectionAssert ShouldHaveSavedCorrection(IDbConnection dbConnection)
        {
            var dbCorrection = dbConnection.GetCorrectionById(_correction.Id);
            dbCorrection.Should().NotBeNull();
            _correction.Language.Should().Be(dbCorrection.Language);
            _correction.Profile.Should().Be(dbCorrection.Profile);
            _correction.IncorrectText.Should().Be(dbCorrection.IncorrectText);
            _correction.CorrectText.Should().Be(dbCorrection.CorrectText);
            _correction.CompleteWord.Should().Be(dbCorrection.CompleteWord);
            return this;
        }

        public CorrectionAssert ShouldMatchSavedCorrection(IDbConnection dbConnection, CorrectionDto correction)
        {
            var dbCorrection = dbConnection.GetCorrectionById(correction.Id);
            dbCorrection.Should().NotBeNull();
            correction.Language.Should().Be(dbCorrection.Language);
            correction.Profile.Should().Be(dbCorrection.Profile);
            correction.IncorrectText.Should().Be(dbCorrection.IncorrectText);
            correction.CorrectText.Should().Be(dbCorrection.CorrectText);
            correction.CompleteWord.Should().Be(dbCorrection.CompleteWord);
            return this;
        }

        public CorrectionAssert ShouldHaveCorrectSeriesRetunred(CorrectionDto correction)
        {
            _correction.Language.Should().Be(correction.Language);
            _correction.Profile.Should().Be(correction.Profile);
            _correction.IncorrectText.Should().Be(correction.IncorrectText);
            _correction.CorrectText.Should().Be(correction.CorrectText);
            _correction.CompleteWord.Should().Be(correction.CompleteWord);
            return this;
        }
    }

    internal static class CorrectionAssertionExtensions
    {
        internal static CorrectionAssert ShouldMatch(this CorrectionView view, CorrectionDto dto)
        {
            return CorrectionAssert.FromObject(view)
                               .ShouldBeSameAs(dto);
        }
    }
}
