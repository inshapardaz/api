using System.Collections.Generic;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;

namespace Inshapardaz.Api.UnitTests.Fakes
{
    public class FakeBackgroundJobClient : IBackgroundJobClient
    {
        private string _jobId = "test_job_id";
        private readonly List<Job> _jobsQueued = new List<Job>();

        public string Create(Job job, IState state)
        {
            _jobsQueued.Add(job);
            return _jobId;
        }

        public bool ChangeState(string jobId, IState state, string expectedState)
        {
            throw new System.NotImplementedException();
        }

        public FakeBackgroundJobClient WithJobId(string jobId)
        {
            _jobId = jobId;
            return this;
        }

        public bool WasJobQueued(Job job)
        {
            return _jobsQueued.Contains(job);
        }
    }
}