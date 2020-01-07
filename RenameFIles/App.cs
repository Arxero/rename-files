using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenameFiles
{
    public class App : IHostedService
    {
        private readonly IOptions<AppConfig> config;
        private readonly ILogger<App> logger;

        public App(IOptions<AppConfig> config, ILogger<App> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public AppConfig Config => config.Value;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var videoFileNames = new List<string>();

            foreach (string file in Directory.EnumerateFiles(Config.WorkingFolder, "*.mp4"))
            {
                var fileName = Path.GetFileName(file);
                videoFileNames.Add(fileName);
                logger.LogInformation($"Reading file {fileName}");
                var destFile = Path.Combine(Config.WorkingFolder, "renamed file.mp4");
                File.Move(file, destFile);
            }
            var a = 1;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping app...");
            return Task.CompletedTask;
        }
    }
}
