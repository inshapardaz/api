namespace Inshapardaz.Domain.Adapters.Configuration;

public record Security
{
    public string Secret { get; init; }
    public string RegisterPagePath { get; init; }
    public string ResetPasswordPagePath { get; init; }
    public int AccessTokenTTLInMinutes { get; init; } = 60;
    public double ResetTokenTTLInDays { get; init; } = 1;
    public int RefreshTokenTTLInDays { get; init; } = 30;
}
