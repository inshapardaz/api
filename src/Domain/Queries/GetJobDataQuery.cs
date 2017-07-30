using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetJobStateQuery : IQuery<string>
    {
        public string JobId { get; set; }
    }
}