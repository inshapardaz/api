using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;

namespace Inshapardaz.Api.Mappings
{
    public static class UserPageSummaryMapper
    {
        public static UserPageSummaryView Map(this UserPageSummaryItem source)
            => new UserPageSummaryView
            {
                Status = source.Status.ToDescription(),
                Count = source.Count
            };

        public static UserPageSummaryItem Map(this UserPageSummaryView source)
            => new UserPageSummaryItem
            {
                Status = source.Status.ToEnum<EditingStatus>(EditingStatus.All),
                Count = source.Count
            };
    }
}
