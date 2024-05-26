using System;

namespace Inshapardaz.Domain.Models;

public class AccountModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool AcceptTerms { get; set; }
    public string VerificationToken { get; set; }
    public DateTime? Verified { get; set; }
    public bool IsSuperAdmin { get; set; }
    public string InvitationCode { get; internal set; }
    public DateTime? InvitationCodeExpiry { get; set; }
    public string PasswordHash { get; internal set; }
    public string ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    public DateTime? PasswordReset { get; set; }
    public bool IsVerified => Verified != null;

    public Role Role { get; set; }
}
