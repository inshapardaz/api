using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly IRenderResponseFromObject<JobStatus, JobStatusModel> _jobsStatusRenderer;

        public JobsController(IRenderResponseFromObject<JobStatus, JobStatusModel> jobsStatusRenderer)
        {
            _jobsStatusRenderer = jobsStatusRenderer;
        }

        [HttpGet("/api/job/status", Name = "JobStatus")]
        public async Task<IActionResult> DownloadStatus(string id)
        {
            IStorageConnection connection = JobStorage.Current.GetConnection();
            var jobData = connection.GetJobData(id);

            if (jobData == null)
            {
                return NotFound();
            }

            var result = _jobsStatusRenderer.Render(new JobStatus
            {
                State = jobData.State,
                JobId = id
            });
            return Ok(result);
        }
    }
}