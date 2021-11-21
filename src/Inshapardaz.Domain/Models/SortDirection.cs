using System.Text.Json.Serialization;

namespace Inshapardaz.Domain.Models.Library
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortDirection
    {
        Ascending,
        Descending
    }
}
