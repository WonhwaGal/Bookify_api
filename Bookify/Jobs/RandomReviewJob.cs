using Bookify.Infrastructure;
using Quartz;
using System.Diagnostics;

namespace Bookify.Jobs
{
    public class RandomReviewJob(IServiceScopeFactory serviceScopeFactory) : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var randomNumber = new Random().Next(0, dbContext.Reviews.Count());

            var reviews = dbContext.Reviews.ToList();
            var randomReview = reviews[randomNumber];

            var reviewApartment = dbContext.Apartments.FirstOrDefault(ap => ap.Id == randomReview.ApartmentId);
            if (reviewApartment != null)
                Debug.WriteLine($"{DateTime.Now} >>> " +
                    $"Random review for appartment {reviewApartment.Name}: \"{randomReview.Comment}\"");
            else
                Debug.WriteLine($"{DateTime.Now} >>> Requested review apartment is null");

            return Task.CompletedTask;
        }
    }
}
