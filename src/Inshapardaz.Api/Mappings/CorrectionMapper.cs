using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Views;

namespace Inshapardaz.Api.Mappings
{
    public static class CorrectionMapper
    {
        public static CorrectionView Map(this CorrectionModel source)
            => new CorrectionView
            {
                Id = source.Id,
                Language = source.Language,
                Profile = source.Profile,
                IncorrectText = source.IncorrectText,
                CorrectText = source.CorrectText,
                CompleteWord = source.CompleteWord
            };

        public static CorrectionModel Map(this CorrectionView source)
            => new CorrectionModel
            {
                Id = source.Id,
                Language = source.Language,
                Profile = source.Profile,
                IncorrectText = source.IncorrectText,
                CorrectText = source.CorrectText,
                CompleteWord = source.CompleteWord
            };

        public static CorrectionSimpleView MapSimple(this CorrectionModel source)
            => new CorrectionSimpleView
            {
                IncorrectText = source.IncorrectText,
                CorrectText = source.CorrectText,
                CompleteWord = source.CompleteWord
            };

    }
}
