// <copyright file="Program.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace LearningHub.Nhs.Content
{
    /// <summary>
    ///     Defines the <see cref="Program" />.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     The Main.
        /// </summary>
        /// <param name="args">The args<see cref="string[]" />.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        ///     The CreateHostBuilder.
        /// </summary>
        /// <param name="args">The args<see cref="string[]" />.</param>
        /// <returns>The <see cref="IHostBuilder" />.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    UpdateNLogConfig(config.Build(), hostingContext.HostingEnvironment);
                })
                .ConfigureLogging((host, logging) =>
                {
                    logging.ClearProviders()
                        .AddConfiguration(host.Configuration.GetSection("Logging"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNLog();
        }
        private static void UpdateNLogConfig(IConfiguration configuration, IHostEnvironment env)
        {
            var storageConString = configuration.GetValue<string>("ConnectionStrings:LearningHubContentLogStorage");
            var storageTableName = configuration.GetValue<string>("Settings:LearningHubContentLogTable");
            GlobalDiagnosticsContext.Set("StorageConnectionString", storageConString);
            GlobalDiagnosticsContext.Set("StorageTableName", storageTableName);
            LogManager.Configuration = LogManager.LoadConfiguration("nlog.config").Configuration;
        }
    }
}