using Bookify.Extensions;
using Bookify.Health;
using Bookify.Identity.Services;
using Bookify.Models.Services;
using Bookify.Models.Services.Impl;
using Bookify.Options;
using Bookify.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace Bookify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHealthChecks()
                .AddCheck<CustomSqlHealthCheck>("custom-sql")
                .AddCheck("sample-check", () =>
                {
                    return HealthCheckResult.Healthy("Sample check is healthy");
                })
                .AddSqlServer(builder.Configuration.GetConnectionString("Bookify-Database")!, name: "bookify-sql")
                .AddSqlServer(builder.Configuration.GetConnectionString("Bookify-Identity-Database")!, name: "bookify-identity-sql")
                .AddRedis(builder.Configuration.GetConnectionString("Cache")!, name: "bookify-redis-cache")
                .AddUrlGroup(new Uri(builder.Configuration["ExternalServices:EmailClient:HealthCheck"]!), HttpMethod.Get, name: "email-check")
                .AddUrlGroup(new Uri(builder.Configuration["ExternalServices:SmsClient:HealthCheck"]!), HttpMethod.Get, name: "sms-check");

            builder.Services.AddHealthChecksUI(setup =>
            {
                setup.AddHealthCheckEndpoint("default", "http://localhost/health");
                setup.SetEvaluationTimeInSeconds(20);
            })
                .AddInMemoryStorage();

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
                logging.CombineLogs = true;
            });

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddRepositories();

            builder.Services.AddScoped<PricingService>();
            builder.Services.AddScoped<ApartmentSettingService>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            builder.Services.AddHttpClient<IDefaultEmailClient, DefaultEmailClient>();
            builder.Services.AddHttpClient<IDefaultSmsClient, DefaultSmsClient>();

            builder.Services.AddOptions(builder.Configuration);
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddAdentity(builder.Configuration);
            builder.Services.AddCaching(builder.Configuration);
            builder.Services.AddJobs();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example : \" Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();

                //support for DateOnly in Swagger
                options.UseDateOnlyTimeOnlyStringConverters();

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Bookify API",
                    Description = "API взаимодействия с сервисом бронирования аппартаментов",
                    Contact = new OpenApiContact
                    {
                        Name = "Контактная информация",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Сведения о лицензии",
                        Url = new Uri("https://example.com/license")
                    }
                });

                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "BookifyAPI.xml");
                options.IncludeXmlComments(xmlPath);
            });

            var app = builder.Build();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.ApplyMigrations();
                //app.SeedData();
                app.SeedIdentityData();
            }

            app.UseHttpLogging();
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RequestContextLoggingMiddleware>();

            app.MapControllers();

            app.MapHealthChecksUI(setup =>
            {
                setup.UIPath = "/hc-ui";
            });

            app.Run();
        }
    }
}
