using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace RenameFiles
{
    class Program
    {
        public static void Main(string[] args)
        {

            var logpath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.txt");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logpath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                var environment = env.EnvironmentName.ToLower();

                config.AddJsonFile("appsettings.json", optional: false)
                   .AddJsonFile($"appsettings.{environment}.json", optional: true)
                   .AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }

            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<AppConfig>(hostContext.Configuration.GetSection("AppConfig"));
                services.AddOptions();
                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
                services.AddHostedService<App>();
            })
            .ConfigureLogging((hostContext, configLogging) =>
            {
                // configLogging.AddConsole();

            })
            .UseConsoleLifetime();
 
    }
}
