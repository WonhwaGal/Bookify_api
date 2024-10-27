using Quartz;
using System.Diagnostics;

namespace Bookify.Jobs
{
    public class SampleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine($"{DateTime.Now} >>> Sample Job ...");
            return Task.CompletedTask;
        }
    }
}
