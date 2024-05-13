using System;

namespace Inshapardaz.Domain.Models;

public class RefreshTokenModel
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string RevokedByIp { get; set; }
    public string ReplacedByToken { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
}
