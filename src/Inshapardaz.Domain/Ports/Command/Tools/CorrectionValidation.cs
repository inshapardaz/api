using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Ports.Command.Tools;

internal static class CorrectionValidation
{
    public static void ValidateProfile(string profile)
    {
        if (!CorrectionProfiles.IsValid(profile))
        {
            throw new BadRequestException($"Unknown correction list '{profile}'. Use one of: {string.Join(", ", CorrectionProfiles.All)}.");
        }
    }

    public static void Validate(CorrectionModel correction)
    {
        ValidateProfile(correction.Profile);
        if (string.IsNullOrEmpty(correction.IncorrectText) || string.IsNullOrEmpty(correction.CorrectText))
        {
            throw new BadRequestException("Both the incorrect and the correct text are required.");
        }
    }
}
