using System;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly IRenderJobStatus _jobsStatusRenderer;

        public JobsController(IRenderJobStatus jobsStatusRenderer)
        {
            _jobsStatusRenderer = jobsStatusRenderer;
        }

        [HttpGet("/api/job/status", Name = "JobStatus")]
        public Task<IActionResult> DownloadStatus(string id)
        {
            throw new NotImplementedException();
        }
    }
}