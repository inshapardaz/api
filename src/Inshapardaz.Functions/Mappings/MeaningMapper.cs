using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Functions.Views.Dictionaries;

namespace Inshapardaz.Functions.Mappings
{
    public static class MeaningMapper
    {
        public static MeaningView Map(this MeaningModel source)
            => new MeaningView
            {
                Id = source.Id,
                Context = source.Context,
                Value = source.Value,
                Example = source.Example,
            };

        public static MeaningModel Map(this MeaningView source)
            => new MeaningModel
            {
                Id = source.Id,
                Context = source.Context,
                Value = source.Value,
                Example = source.Example
            };
    }
}
