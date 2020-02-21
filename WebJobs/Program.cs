using Core.Extensions;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Providers.File;
using Providers.Queue;
using Providers.Sms;
using Services;
using System;
using System.IO;

namespace WebJobs
{
    internal class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static void Main()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var projectConfiguration = new ProjectConfiguration(config,
                environment,
                environment == "localhost",
                environment == "development",
                environment == "staging",
                environment == "production");

            var builder = new HostBuilder();
            _ = builder
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddAzureStorage();
                    b.AddServiceBus();
                    b.AddTimers();
                })
                .ConfigureLogging(t =>
                {
                    t.AddConsole();
                })
                .ConfigureServices((Action<IServiceCollection>)(services =>
                {
                    services.AddSingleton(projectConfiguration);
                    services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(projectConfiguration.DefaultConnection));

                    services.AddScoped((Func<IServiceProvider, IFileProvider>)(_ => new AzureFileProvider(projectConfiguration.StorageConnection)));
                    services.AddScoped((Func<IServiceProvider, IQueueProvider>)(_ => new AzureServiceBusQueueProvider(projectConfiguration.AzureWebJobsServiceBus)));
                    services.AddTransient<UserService, UserService>();
                    services.AddTransient<KeyValuePairService, KeyValuePairService>();
                    services.AddTransient<ISmsProvider, SlackSmsProvider>();
                    services.AddTransient<Services.SmsService, Services.SmsService>();
                }));

            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }
    }
}