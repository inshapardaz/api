using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Model
{
    public class DictionaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public Languages Language { get; set; }
        public string UserId { get; set; }
    }
}