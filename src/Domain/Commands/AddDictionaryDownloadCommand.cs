namespace Inshapardaz.Domain.Commands
{
    public class AddDictionaryDownloadCommand : Command
    {
        public int DitionarayId { get; set; }

        public string DownloadType { get; set; }

        public string JobId { get; set; }
    }
}
