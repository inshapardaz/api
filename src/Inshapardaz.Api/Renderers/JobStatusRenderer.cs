using System.Collections.Generic;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderJobStatus
    {
        JobStatusModel Render(JobStatus source);
    }

    public class JobStatusRenderer : IRenderJobStatus
    {
        private readonly IRenderLink _linkRenderer;

        public JobStatusRenderer(IRenderLink linkRenderer)
        {
            _linkRenderer = linkRenderer;
        }

        public JobStatusModel Render(JobStatus source)
        {
            return new JobStatusModel
            {
                Status = source.State,
                Links = new List<LinkView>
                {
                    _linkRenderer.Render("JobStatus", "self", new {source.JobId})
                }
            };
        }
    }
}