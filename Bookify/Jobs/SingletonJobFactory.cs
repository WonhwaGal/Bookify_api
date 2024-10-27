using Quartz;
using Quartz.Spi;

namespace Bookify.Jobs
{
    public class SingletonJobFactory(IServiceProvider serviceProvider) : IJobFactory
    {
        // required service type is passed in the bundle 
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)serviceProvider.GetRequiredService(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            //
        }
    }
}