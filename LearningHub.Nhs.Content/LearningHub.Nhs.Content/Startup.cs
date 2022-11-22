// <copyright file="Startup.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>


using System;

namespace LearningHub.Nhs.Content
{
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
    using System.Net.Http;
    using System.IO;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Logging;
    using System.Text;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;

    /// <summary>
    /// Defines the <see cref="Startup" />.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The hosting environment..
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Defines the AllowOrigins.
        /// </summary>
        private readonly string AllowOrigins = "allowOrigins";

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        /// <param name="env">The env<see cref="IWebHostEnvironment"/>.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.environment = environment;
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
            if (this.environment.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseExceptionHandler("/Error");               
            }

            var defaultOptions = new DefaultFilesOptions();
            defaultOptions.DefaultFileNames.Clear();
            app.UseDefaultFiles(defaultOptions);

            var scormContentRequestHandler = app.ApplicationServices.GetService<IScormContentRewriteService>();
            var settings = app.ApplicationServices.GetService<IOptions<Settings>>();
            var logger = app.ApplicationServices.GetService<ILogger<ScormContentRewriteRule>>();

            var rewriteOptions = new RewriteOptions()
                .Add(new ScormContentRewriteRule(scormContentRequestHandler, settings, logger));
            app.UseRewriter(rewriteOptions);

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(settings.Value.LearningHubContentPhysicalPath),
                RequestPath = settings.Value.LearningHubContentVirtualPath,
                EnableDirectoryBrowsing = false
            });

            app.UseRouting();
            app.UseCors(AllowOrigins);
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapPost("/remove-cache/{key}", async context =>
                {
                    var cacheService = context.RequestServices.GetService<ICacheService>();
                    await cacheService.RemoveAsync(context.Request.RouteValues["key"].ToString());
                    await context.Response.WriteAsync("Cache removed");
                });

                endpoints.MapPost("/remove-cache/{key}", async context =>
                {
                    var cacheService = context.RequestServices.GetService<ICacheService>();
                    await cacheService.RemoveAsync(context.Request.RouteValues["key"].ToString());
                    await context.Response.WriteAsync("Cache removed");
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
            services.AddRazorPages();
            // Register an ILearningHubHttpClient.
            if (this.environment.IsDevelopment())
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

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowOrigins, builder =>
                {
                    builder.AllowAnyOrigin();
                });
            });

            var environment = this.Configuration.GetValue<EnvironmentEnum>("Environment");
            var envPrefix = environment.GetAbbreviation();
            if (environment == EnvironmentEnum.Local)
            {
                envPrefix += $"_{Environment.MachineName}";
            }

            // Set up redis caching.
            services.AddDistributedCache(opt =>
            {
                opt.RedisConnectionString = this.Configuration.GetConnectionString("Redis");
                opt.KeyPrefix = $"{envPrefix}_ContentServer";
            });

            services.AddSingleton<IScormContentRewriteService, ScormContentRewriteService>();
        }
    }
}
