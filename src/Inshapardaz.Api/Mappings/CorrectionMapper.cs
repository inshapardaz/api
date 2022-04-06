using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Views;

namespace Inshapardaz.Api.Mappings
{
    public static class CorrectionMapper
    {
        public static CorrectionView Map(this CorrectionModel source)
            => new CorrectionView
            {
                Language = source.Language,
                Profile = source.Profile,
                IncorrectText = source.IncorrectText,
                CorrectText = source.CorrectText
            };

        public static CorrectionModel Map(this CorrectionView source)
            => new CorrectionModel
            {
                Language = source.Language,
                Profile = source.Profile,
                IncorrectText = source.IncorrectText,
                CorrectText = source.CorrectText
            };
    }
}
