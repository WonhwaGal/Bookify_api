
using Bookify.Infrastructure.Services;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bookify.Health
{
    public class CustomSqlHealthCheck(ISqlConnectionFactory sqlConnectionFactory) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = sqlConnectionFactory.CreateConnection();
                await connection.ExecuteAsync("SELECT 1");

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
