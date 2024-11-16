namespace Inshapardaz.Domain.Adapters.Configuration;

public record Settings
{
    public Email Email { get; init; }

    public Security Security { get; init; }

    public Database Database { get; init; }

    public Storage Storage { get; init; }

    public string[] AllowedOrigins { get; init; }

    public string FrontEndUrl { get; init; }

    public int DefaultLibraryId { get; init; }

    public bool SaveDownloadsToStorage { get; init; } = false;
    
    public string? Domain { get; set; }
}
