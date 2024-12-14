using Bookify.Models.Services.Impl;
using Bookify.Models.Services;
using System.Runtime.CompilerServices;
using Bookify.Options;
using Microsoft.AspNetCore.Mvc;
using Bookify.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Bookify.Infrastructure.Services;
using Bookify.Infrastructure.Services.Impl;
using Dapper;
using Bookify.Infrastructure.Data;
using Bookify.Identity;
using Bookify.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Bookify.Services;
using Bookify.Services.Impl;
using Bookify.Jobs;
using Quartz.Spi;
using Quartz;
using Quartz.Impl;

namespace Bookify.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // will live as long as controller lives
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IApartmentRepository, ApartmentRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            return services;
        }

        public static IServiceCollection AddJobs(this IServiceCollection services)
        {
            services.AddSingleton(new JobSchedule(typeof(SampleJob), "0/10 * * ? * * *"));  // every 10 sec
            services.AddSingleton(new JobSchedule(typeof(BookingsCountJob), "0/30 * * ? * * *"));  // every 30 sec
            services.AddSingleton(new JobSchedule(typeof(RandomReviewJob), "0/45 * * ? * * *"));  // every 45 sec

            services.AddSingleton<SampleJob>();
            services.AddSingleton<BookingsCountJob>();
            services.AddSingleton<RandomReviewJob>();

            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddHostedService<QuartzHostedService>();

            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            services.Configure<EmailClientOptions>(settings =>
               configuration.GetSection("ExternalServices:EmailClient").Bind(settings));

            services.Configure<SmsClientOptions>(settings =>
               configuration.GetSection("ExternalServices:SmsClient").Bind(settings));

            return services;
        }

        public static IServiceCollection AddAdentity(this IServiceCollection services, IConfiguration configuration)
        {
            //Identity
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddSignInManager()
                .AddRoles<Role>();

            //Jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                };
            });

            return services;
        }

        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Cache") ??
                throw new ArgumentNullException(nameof(configuration));

            services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
            services.AddSingleton<ICacheService, CacheService>();
            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Bookify-Database") ?? 
                throw new ArgumentNullException(nameof(configuration));

            var connectionStringIdentity = configuration.GetConnectionString("Bookify-Identity-Database") ??
                throw new ArgumentNullException(nameof(configuration));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseSqlServer(connectionStringIdentity);
            });

            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(connectionString));

            //поддержка типа DateOnly в Dapper
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
            return services;
        }
    }
}
