using System.Text.Json.Serialization;

namespace Inshapardaz.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IssueSortByType
{
    IssueDate,
    VolumeNumber,
    VolumeNumberAndIssueNumber
}
