namespace LearningHub.Nhs.Content
{
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Content.Service;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Defines the <see cref="Startup" />.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The ConfigureServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IScormContentRewriteService, ScormContentRewriteService>();
        }

        /// <summary>
        /// The Configure.
        /// </summary>
        /// <param name="app">The app<see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">The env<see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var defaultOptions = new DefaultFilesOptions();
            defaultOptions.DefaultFileNames.Clear();
            defaultOptions.DefaultFileNames.Add("index.html");
            defaultOptions.DefaultFileNames.Add("index_lms.html");
            app.UseDefaultFiles(defaultOptions);

            var scormContentRequestHandler = app.ApplicationServices.GetService<IScormContentRewriteService>();

            var rewriteOptions = new RewriteOptions()
                 .Add(new ScormContentRewriteRule(scormContentRequestHandler));

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
    }
}
