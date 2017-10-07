using System.Collections.Generic;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public class JobStatusRenderer : IRenderResponseFromObject<JobStatus, JobStatusModel>
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