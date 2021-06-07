// <copyright file="ScormContentRewriteRule.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content
{
    using LearningHub.Nhs.Content.Configuration;
    using LearningHub.Nhs.Content.Extensions;
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Content.Models;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Resource;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ScormContentRewriteRule" />.
    /// </summary>
    public class ScormContentRewriteRule : IRule
    {
        /// <summary>
        /// Defines a string which is prefixed to all cache keys used by the LH Content Server.........
        /// </summary>
        private const string KeyPrefix = "ContentServer";

        /// <summary>
        /// Defines the scormContentRewriteService.
        /// </summary>
        private readonly IScormContentRewriteService scormContentRewriteService;

        /// <summary>
        /// The settings......
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Defines the sourceSystems.
        /// </summary>
        private List<MigrationSourceViewModel> sourceSystems = null;

        /// <summary>
        /// Defines the logger.
        /// </summary>
        private readonly ILogger<ScormContentRewriteRule> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentRewriteRule"/> class.
        /// </summary>
        /// <param name="scormContentRewriteService">The scormContentRewriteService<see cref="IScormContentRewriteService" />.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">logger.</param>
        public ScormContentRewriteRule(IScormContentRewriteService scormContentRewriteService,
            IOptions<Settings> settings, ILogger<ScormContentRewriteRule> logger)
        {
            this.scormContentRewriteService = scormContentRewriteService;
            this.settings = settings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// The LoadSourceSystemsAsync.
        /// </summary>
        private void LoadSourceSystemsAsync()
        {
            sourceSystems ??= this.scormContentRewriteService
                .GetMigrationSourcesAsync($"{KeyPrefix}-Migration-Sources").Result;
            this.logger.LogInformation($"source systems {sourceSystems.Count}");
        }

        /// <summary>
        /// The ApplyRule.
        /// </summary>
        /// <param name="context">The context<see cref="RewriteContext" />.</param>
        public void ApplyRule(RewriteContext context)
        {
            if (context.HttpContext.Request.Path.Value.Contains("/nlog/"))
                return;
            try
            {
                var displayUrl = context.HttpContext.Request.GetDisplayUrl();

                if (this.sourceSystems == null)
                {
                    this.LoadSourceSystemsAsync();
                }
                var migrationSource = sourceSystems.GetMigrationSource(displayUrl);

                if (migrationSource == null)
                    return;

                _ = this.HandleRequestsAsync(context, migrationSource);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, exception.Message);
            }
        }

        /// <summary>
        /// The HandleExternalSourceRequests.
        /// </summary>
        /// <param name="context">The context<see cref="RewriteContext" />.</param>
        /// <param name="sourceSystem">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task HandleRequestsAsync(RewriteContext context, MigrationSourceViewModel sourceSystem)
        {
            // example https://www.elearningrepository.nhs.uk/sites/default/files/lms/5742/Lesson_1/default.html
            // example https://cs1.e-learningforhealthcare.org.uk/content/ACM_001/
            // example https://content.learninghub.nhs.uk/content/07BD413E-5758-43C9-9117-F7A7F469452E/

            var requestPath = context.HttpContext.Request.Path.Value;
            var fullResourceUrl = context.HttpContext.Request.GetDisplayUrl();

            var pathSegments = requestPath.Split('/');

            if (pathSegments.Length < sourceSystem.ResourceIdentifierPosition)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            var resourceExternalReference = pathSegments[sourceSystem.ResourceIdentifierPosition - 1];
            var match = Regex.Match(resourceExternalReference, sourceSystem.ResourceRegEx, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                this.logger.LogInformation($"{requestPath} : resourceExternalReference :{resourceExternalReference}: Resource Identifier Invalid Regex Format");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var cacheKey = $"{KeyPrefix}-{sourceSystem.Description}-{resourceExternalReference}";

            ScormContentServerViewModel scormContentDetail = null;
            switch (sourceSystem.SourceType())
            {
                case SourceType.LearningHub:
                    scormContentDetail = await scormContentRewriteService.GetScormContentDetailsByExternalReferenceAsync(resourceExternalReference, cacheKey);
                    break;
                case SourceType.eLR:
                    scormContentDetail = await scormContentRewriteService.GetScormContentDetailsByExternalUrlAsync(fullResourceUrl, cacheKey);
                    break;
                case SourceType.eWIN:
                default:
                    this.logger.LogInformation("SourceType : Not Supported");
                    break;
            }

            if (scormContentDetail == null)
            {
                this.logger.LogInformation($"{requestPath} : resourceExternalReference :{resourceExternalReference}: scormContentDetail NOT FOUND");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var rewrittenUrlStringBuilder = new StringBuilder();
            if (pathSegments.Length == sourceSystem.ResourceIdentifierPosition)
            {
                var hostSegment = fullResourceUrl[..fullResourceUrl.IndexOf(sourceSystem.ResourcePath)];
                rewrittenUrlStringBuilder.Append($"{hostSegment}{requestPath}/{scormContentDetail.ManifestUrl}");

                context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
                context.HttpContext.Response.Headers[HeaderNames.Location] = rewrittenUrlStringBuilder.ToString();

                this.logger.LogInformation($"Manifest file included {rewrittenUrlStringBuilder}");
                context.Result = RuleResult.EndResponse;
                return;
            }

            rewrittenUrlStringBuilder
                .Append(requestPath)
                .Replace(sourceSystem.ResourcePath, $"{this.settings.LearningHubContentVirtualPath}/")
                .Replace(resourceExternalReference, scormContentDetail.InternalResourceIdentifier);
            context.HttpContext.Request.Path = rewrittenUrlStringBuilder.ToString();
            if (requestPath.Contains("htm"))
            {
                this.logger.LogInformation($"Source System :{sourceSystem.Description} ---- Request Path:{requestPath} ---- Rewritten Path:{rewrittenUrlStringBuilder}");
            }
        }
    }
}
