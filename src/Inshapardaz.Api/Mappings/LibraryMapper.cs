using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Mappings;

public static class LibraryMapper
{
    public static LibraryView Map(this LibraryModel source)
        => source == null ? null : new LibraryView
        {
            Name = source.Name,
            OwnerEmail = source.OwnerEmail,
            Description = source.Description,
            Language = source.Language,
            SupportsPeriodicals = source.SupportsPeriodicals,
            PrimaryColor = source.PrimaryColor,
            SecondaryColor = source.SecondaryColor,
            Public = source.Public,
            DatabaseConnection = source.DatabaseConnection,
            FileStoreSource = source.FileStoreSource,
            FileStoreType = source.FileStoreType.ToDescription()
        };

    public static LibraryModel Map(this LibraryView source)
        => source == null ? null : new LibraryModel
        {
            Name = source.Name,
            Description = source.Description,
            OwnerEmail = source.OwnerEmail,
            Language = source.Language,
            SupportsPeriodicals = source.SupportsPeriodicals,
            PrimaryColor = source.PrimaryColor,
            SecondaryColor = source.SecondaryColor,
            Public = source.Public,
            DatabaseConnection = source.DatabaseConnection,
            FileStoreSource = source.FileStoreSource,
            FileStoreType = source.FileStoreType.ToEnum<FileStoreTypes>(FileStoreTypes.Unknown)
        };
}
