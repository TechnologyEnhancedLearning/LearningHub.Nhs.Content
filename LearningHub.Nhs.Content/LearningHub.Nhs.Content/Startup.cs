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
    using LearningHub.Nhs.WebUI.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using System.Net.Http;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Logging;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using Microsoft.AspNetCore.StaticFiles;
    using System.IO;
    using System.Linq;

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
    /// <param name="environment">The env<see cref="IWebHostEnvironment"/>.</param>
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
        app.UseDeveloperExceptionPage();
        //app.UseExceptionHandler("/Error");
      }
      else
      {
        app.UseExceptionHandler("/Error");
      }

      var defaultOptions = new DefaultFilesOptions();
      defaultOptions.DefaultFileNames.Clear();
      app.UseDefaultFiles(defaultOptions);

      var contentRequestHandler = app.ApplicationServices.GetService<IContentRewriteService>();
      var settings = app.ApplicationServices.GetService<IOptions<Settings>>();
      var logger = app.ApplicationServices.GetService<ILogger<ResourceContentRewriteRule>>();

      var rewriteOptions = new RewriteOptions()
          .Add(new ResourceContentRewriteRule(contentRequestHandler, settings, logger));
      app.UseRewriter(rewriteOptions);

      if (!Convert.ToBoolean(settings.Value.EnableAzureFileStorageRESTAPIaccess))
      {
        app.UseFileServer(new FileServerOptions
        {
          FileProvider = new PhysicalFileProvider(settings.Value.LearningHubContentPhysicalPath),
          RequestPath = settings.Value.LearningHubContentVirtualPath,
          EnableDirectoryBrowsing = false,
        });
      }

      app.UseRouting();
      app.UseCors(AllowOrigins);

      var provider = new FileExtensionContentTypeProvider();
      var fileMappings = Configuration.GetSection("Settings:FileTypeMappings")
                          .GetChildren()
                          .ToDictionary(x => x.Key, x => x.Value);

      foreach (var fileMapping in fileMappings)
      {
        provider.Mappings[fileMapping.Key] = fileMapping.Value;
      }

      if (!Convert.ToBoolean(settings.Value.EnableAzureFileStorageRESTAPIaccess))
      {
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
          FileProvider = new PhysicalFileProvider(settings.Value.LearningHubContentPhysicalPath),
          RequestPath = settings.Value.LearningHubContentVirtualPath,
          ContentTypeProvider = provider,
        });
      }

      app.UseEndpoints(endpoints =>
          {
            endpoints.MapRazorPages();
            endpoints.MapPost("/remove-cache/{key}", async context =>
              {
                var cacheService = context.RequestServices.GetService<ICacheService>();
                await cacheService.RemoveAsync(context.Request.RouteValues["key"].ToString());
                await context.Response.WriteAsync("Cache removed");
              });
            endpoints.MapControllerRoute(
              name: "default",
              pattern: "{controller=lhcontent}/{action=Index}");
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

      services.AddSingleton<IContentRewriteService, ContentRewriteService>();

      if (Convert.ToBoolean(this.Configuration.GetSection("settings")["EnableAzureFileStorageRESTAPIaccess"]))
      {
        services.AddMvc();

        services.AddScoped<IFileService, FileService>();
      }
    }
  }
}
