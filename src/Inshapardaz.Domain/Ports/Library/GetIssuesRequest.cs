using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetIssuesRequest :  RequestBase
    {
        public GetIssuesRequest(int periodicalId, int pageNumber, int pageSize)
        {
            PeriodicalId = periodicalId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PeriodicalId { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Page<Issue> Result { get; set; }
    }

    public class GetIssuesRequestHandler : RequestHandlerAsync<GetIssuesRequest>
    {
        private readonly IIssueRepository _issueRepository;

        public GetIssuesRequestHandler(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public override async Task<GetIssuesRequest> HandleAsync(GetIssuesRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _issueRepository.GetIssues(command.PeriodicalId, command.PageNumber, command.PageSize, cancellationToken);
           
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

