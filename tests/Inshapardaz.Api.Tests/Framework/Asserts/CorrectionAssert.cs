using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    internal class CorrectionAssert
    {
        private CorrectionView _correction;
        public HttpResponseMessage _response;
        public CorrectionView View => _correction;

        public readonly ICorrectionTestRepository _correctionRepository;

        public CorrectionAssert(ICorrectionTestRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }


        public CorrectionAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _correction = response.GetContent<CorrectionView>().Result;
            return this;
        }

        public CorrectionAssert ForView(CorrectionView view)
        {
            _correction = view;
            return this;
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

        internal CorrectionAssert ShouldHaveDeletedCorrection(long correctionId)
        {
            var correction = _correctionRepository.GetCorrectionById(correctionId);
            correction.Should().BeNull();
            return this;
        }

        internal CorrectionAssert ShouldNotHaveDeletedCorrection(long correctionId)
        {
            var correction = _correctionRepository.GetCorrectionById(correctionId);
            correction.Should().NotBeNull();
            return this;
        }

        public CorrectionAssert ShouldNotHaveDeleteLink()
        {
            _correction.DeleteLink().Should().BeNull();

            return this;
        }

        public CorrectionAssert ShouldHaveSavedCorrection()
        {
            var dbCorrection = _correctionRepository.GetCorrectionById(_correction.Id);
            dbCorrection.Should().NotBeNull();
            _correction.Language.Should().Be(dbCorrection.Language);
            _correction.Profile.Should().Be(dbCorrection.Profile);
            _correction.IncorrectText.Should().Be(dbCorrection.IncorrectText);
            _correction.CorrectText.Should().Be(dbCorrection.CorrectText);
            _correction.CompleteWord.Should().Be(dbCorrection.CompleteWord);
            return this;
        }

        public CorrectionAssert ShouldMatchSavedCorrection(CorrectionDto correction)
        {
            var dbCorrection = _correctionRepository.GetCorrectionById(correction.Id);
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
}
