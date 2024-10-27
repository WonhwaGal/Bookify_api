using Bookify.Infrastructure;
using Bookify.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Diagnostics;

namespace Bookify.Jobs
{
    public class BookingsCountJob(IServiceScopeFactory serviceScopeFactory) : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Debug.WriteLine($"{DateTime.Now} >>> Total number of bookings: {dbContext.Bookings.Count()}");
            return Task.CompletedTask;
        }
    }
}
