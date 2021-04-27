// <copyright file="Startup.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content
{
    using System.Net.Http;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Content.Configuration;
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Content.Service;
    using LearningHub.Nhs.Content.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="Startup" />.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The hosting environment.
        /// </summary>
        private readonly IWebHostEnvironment env;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        /// <param name="env">The env<see cref="IWebHostEnvironment"/>.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.env = env;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the Configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// The Configure.
        /// </summary>
        /// <param name="app">The app<see cref="IApplicationBuilder"/>.</param>
        public void Configure(IApplicationBuilder app)
        {
            if (this.env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var defaultOptions = new DefaultFilesOptions();
            defaultOptions.DefaultFileNames.Clear();
            defaultOptions.DefaultFileNames.Add("index.html");
            defaultOptions.DefaultFileNames.Add("index_lms.html");
            app.UseDefaultFiles(defaultOptions);

            var scormContentRequestHandler = app.ApplicationServices.GetService<IScormContentRewriteService>();
            var settings = app.ApplicationServices.GetService<IOptions<Settings>>();

            var rewriteOptions = new RewriteOptions()
                 .Add(new ScormContentRewriteRule(scormContentRequestHandler, settings));

            app.UseRewriter(rewriteOptions);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Learning Hub Content Server");
                });
            });
        }

        /// <summary>
        /// The ConfigureServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Settings>(this.Configuration.GetSection("Settings"));

            // Register an ILearningHubHttpClient.
            if (this.env.IsDevelopment())
            {
                services.AddHttpClient<ILearningHubHttpClient, LearningHubHttpClient>()
                    .ConfigurePrimaryHttpMessageHandler(
                        () => new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                          HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        });
            }
            else
            {
                services.AddHttpClient<ILearningHubHttpClient, LearningHubHttpClient>();
            }

            services.AddSingleton<IScormContentRewriteService, ScormContentRewriteService>();

            // Set up redis caching.
            services.AddDistributedCache(this.Configuration);
        }
    }
}
