using Amazon.Auth.AccessControlPolicy;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inshapardaz.Api.Views
{
    public class LibraryView : ViewWithLinks
    {
        public int Id { get; set; }
        public string OwnerEmail { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public bool SupportsPeriodicals { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public bool Public { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DatabaseConnection { get; set; }
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FileStoreType { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FileStoreSource { get; set; }
    }
}
