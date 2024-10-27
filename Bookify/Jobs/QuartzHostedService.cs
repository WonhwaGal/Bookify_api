using Quartz;
using Quartz.Spi;

namespace Bookify.Jobs
{
    public class QuartzHostedService(
        IJobFactory jobFactory,
        ISchedulerFactory schedulerFactory,
        IEnumerable<JobSchedule> jobSchedules) : IHostedService
    {
        public IScheduler Scheduler { get; set; }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = jobFactory;

            foreach(var schedule in jobSchedules)
            {
                await Scheduler.ScheduleJob(CreateJobDetail(schedule), CreateTrigger(schedule), cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(Scheduler != null)
            {
                await Scheduler.Shutdown(cancellationToken);
            }
        }

        private static IJobDetail CreateJobDetail(JobSchedule jobSchedule)  // cointains job type
        {
            IJobDetail jobDetail = JobBuilder
                .Create(jobSchedule.JobType)
                .Build();
            return jobDetail;
        }

        private static ITrigger CreateTrigger(JobSchedule jobSchedule) //contains job's launch time
        {
            ITrigger jobTrigger = TriggerBuilder
                .Create()
                .WithCronSchedule(jobSchedule.Expression)
                .Build();
            return jobTrigger;
        }
    }
}
