using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class DeleteDictionaryCommand : Command
    {
        public string UserId { get; set; }

        public int DictionaryId { get; set; }
    }
}
