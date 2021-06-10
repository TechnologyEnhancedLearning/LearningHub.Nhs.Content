// <copyright file="ScormContentRewriteRule.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

using System.Linq;

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
            this.LoadSourceSystems();
        }

        /// <summary>
        /// The LoadSourceSystemsAsync.
        /// </summary>
        private void LoadSourceSystems()
        {
            try
            {
                if (this.sourceSystems != null)
                    return;

                this.sourceSystems = this.scormContentRewriteService
                    .GetMigrationSourcesAsync($"Migration-Sources").Result;

                this.logger.LogTrace($"Source systems Loaded # Count :{sourceSystems.Count}");
            }
            catch (Exception e)
            {
                this.logger.LogError(e, e.Message);
            }
        }

        /// <summary>
        /// The ApplyRule.
        /// </summary>
        /// <param name="context">The context<see cref="RewriteContext" />.</param>
        public void ApplyRule(RewriteContext context)
        {
            try
            {
                var displayUrl = context.HttpContext.Request.GetDisplayUrl();

                var sbRequestHeaders = new StringBuilder();

                sbRequestHeaders.Append($"Request Url # {context.HttpContext.Request.Path}");

                foreach (var header in context.HttpContext.Request.Headers)
                {
                    sbRequestHeaders.AppendLine($"{header.Key} # {header.Value}");
                }

                logger.LogTrace(sbRequestHeaders.ToString());

                this.LoadSourceSystems();

                if (sourceSystems == null)
                {
                    this.logger.LogWarning("SOURCE SYSTEMS NOT LOADED");
                    return;
                }

                var migrationSource = sourceSystems?.GetMigrationSource(displayUrl);

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
            var originalRequestPath = context.HttpContext.Request.Path.Value;
            string requestPath;
            var fullRequestUrl = context.HttpContext.Request.GetDisplayUrl();
            
            const string startUrlParam = "starting_url";
            var startingUrl = string.Empty;
            
            
            if (fullRequestUrl.Contains(startUrlParam, StringComparison.InvariantCultureIgnoreCase))
            {
                startingUrl = context.HttpContext.Request.Query[startUrlParam];
                var index = startingUrl.IndexOf(sourceSystem.ResourcePath,
                    StringComparison.InvariantCultureIgnoreCase);
                requestPath = startingUrl.Substring(index, startingUrl.Length - index);
            }
            else
            {
                startingUrl = fullRequestUrl;
                requestPath = originalRequestPath;
            }
            
            var pathSegments = requestPath.Split('/');
            if (string.IsNullOrEmpty(pathSegments.Last()))
            {
                pathSegments = pathSegments.Take(pathSegments.Length-1).ToArray();
            }
            if (pathSegments.Length < sourceSystem.ResourceIdentifierPosition)
            {
                this.logger.LogWarning($" fullResourceUrl {startingUrl} # Request Path {requestPath} # INVALID PATH SEGMENTS pathSegmentsLength: {pathSegments.Length} # Expected PathSegments: {sourceSystem.ResourceIdentifierPosition}");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var resourceExternalReference = pathSegments[sourceSystem.ResourceIdentifierPosition - 1];
            var match = Regex.Match(resourceExternalReference, sourceSystem.ResourceRegEx, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                this.logger.LogInformation($"{startingUrl} : resourceExternalReference :{resourceExternalReference}: Resource Identifier Invalid Regex Format");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var cacheKey = $"{sourceSystem.Description}_{resourceExternalReference}";

            ScormContentServerViewModel scormContentDetail = null;
            switch (sourceSystem.SourceType())
            {
                case SourceType.LearningHub:
                    scormContentDetail = await scormContentRewriteService.GetScormContentDetailsByExternalReferenceAsync(resourceExternalReference, cacheKey);
                    break;
                case SourceType.eLR:
                    this.logger.LogWarning($"Calling Backend to fetch scorm resource detail {startingUrl}");
                    scormContentDetail = await scormContentRewriteService.GetScormContentDetailsByExternalUrlAsync(startingUrl, cacheKey);
                    break;
                case SourceType.eWIN:
                default:
                    this.logger.LogWarning("SourceType : Not Supported");
                    break;
            }

            if (scormContentDetail == null)
            {
                this.logger.LogWarning($"Original Request Path:{startingUrl} # : resourceExternalReference :{resourceExternalReference}: scormContentDetail NOT FOUND");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var rewrittenUrlStringBuilder = new StringBuilder();
            if (pathSegments.Length == sourceSystem.ResourceIdentifierPosition)
            {
                var hostSegment = startingUrl[..startingUrl.IndexOf(sourceSystem.ResourcePath)];
                rewrittenUrlStringBuilder.Append($"{hostSegment}{requestPath}/{scormContentDetail.ManifestUrl}");

                context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
                context.HttpContext.Response.Headers[HeaderNames.Location] = rewrittenUrlStringBuilder.ToString();

                this.logger.LogInformation($"Original Request Path:{startingUrl} # Manifest file included {rewrittenUrlStringBuilder}");
                context.Result = RuleResult.EndResponse;
                return;
            }

            if (pathSegments.Length == sourceSystem.ResourceIdentifierPosition)
                scormContentDetail.InternalResourceIdentifier = ($"{scormContentDetail.InternalResourceIdentifier}/{scormContentDetail.ManifestUrl}");

            rewrittenUrlStringBuilder
                .Append(requestPath)
                .Replace(sourceSystem.ResourcePath, $"{this.settings.LearningHubContentVirtualPath}/")
                .Replace(resourceExternalReference, scormContentDetail.InternalResourceIdentifier);
            context.HttpContext.Request.Path = rewrittenUrlStringBuilder.ToString();
            
            match = Regex.Match(pathSegments.Last(), @"^(?i:index|default).*\.(htm|html)$");
            
            if (match.Success)
            {
                this.logger.LogInformation(
                    $"Source System :{sourceSystem.Description} # Original Request Path:{startingUrl} # Rewritten Path:{rewrittenUrlStringBuilder}");
            }
            else
            {
                this.logger.LogTrace(
                    $"Source System :{sourceSystem.Description} # Original Request Path:{startingUrl} # Rewritten Path:{rewrittenUrlStringBuilder}");
            }
        }
    }
}
