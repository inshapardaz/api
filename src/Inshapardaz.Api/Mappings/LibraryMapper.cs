using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views;

namespace Inshapardaz.Api.Mappings
{
    public static class LibraryMapper
    {
        public static LibraryView Map(this LibraryModel source)
            => source == null ? null : new LibraryView
            {
                Name = source.Name,
                OwnerEmail = source.OwnerEmail,
                Language = source.Language,
                SupportsPeriodicals = source.SupportsPeriodicals,
                PrimaryColor = source.PrimaryColor,
                SecondaryColor = source.SecondaryColor,
                Public = source.Public,
                DatabaseConnection = null,
                FileStoreSource = source.FileStoreSource,
                FileStoreType = source.FileStoreType
            };

        public static LibraryModel Map(this LibraryView source)
            => source == null ? null : new LibraryModel
            {
                Name = source.Name,
                OwnerEmail = source.OwnerEmail,
                Language = source.Language,
                SupportsPeriodicals = source.SupportsPeriodicals,
                PrimaryColor = source.PrimaryColor,
                SecondaryColor = source.SecondaryColor,
                Public = source.Public,
                DatabaseConnection = source.DatabaseConnection,
                FileStoreSource = source.FileStoreSource,
                FileStoreType = source.FileStoreType
            };
    }
}
