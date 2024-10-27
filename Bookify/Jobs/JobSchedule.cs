namespace Bookify.Jobs
{
    //stores the schedule for the task
    public class JobSchedule(Type jobType, string expression)
    {
        public string Expression { get; set; } = expression;
        public Type JobType { get; set; } = jobType;
    }
}
