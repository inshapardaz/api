using System.Text.Json.Serialization;

namespace Inshapardaz.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    Admin,
    LibraryAdmin,
    Writer,
    Reader
}
