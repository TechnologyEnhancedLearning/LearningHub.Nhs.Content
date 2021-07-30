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
    using System.IO;
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
        private readonly Configuration.Settings settings;

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
            IOptions<Configuration.Settings> settings, ILogger<ScormContentRewriteRule> logger)
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
                var hostName = context.HttpContext.Request.Host.Host.ToString();
                var requestPath = context.HttpContext.Request.Path;
                
                if(requestPath == @"/")
                    return;

                if (requestPath.Value.StartsWith(@"/css/") ||
                    requestPath.Value.StartsWith(@"/lib/") ||
                    requestPath.Value.StartsWith(@"/fonts/") ||
                    requestPath.Value.StartsWith(@"/js/")){
                    return; }

                this.LoadSourceSystems();

                if (sourceSystems == null)
                {
                    this.logger.LogWarning("SOURCE SYSTEMS NOT LOADED");
                    return;
                }

                var migrationSource = sourceSystems?.GetMigrationSource(hostName, requestPath);

                if (migrationSource == null)
                {
                    return;
                }

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
            var requestPath = context.HttpContext.Request.Path.Value?.TrimEnd('/');

            var startingUrl = context.HttpContext.Request.GetDisplayUrl();

            var uriBuilder = new UriBuilder(startingUrl);
            
            var pathSegments = requestPath.Split('/');
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
                    scormContentDetail = scormContentRewriteService.GetScormContentDetailsByExternalReferenceAsync(resourceExternalReference, cacheKey).Result;
                    break;
                case SourceType.eLR:
                    var resourceUri = $"{sourceSystem.ResourcePath}{resourceExternalReference}/";
                    this.logger.LogTrace($"resourceUri '{resourceUri}' Calling Backend Api");
                    scormContentDetail = scormContentRewriteService.GetScormContentDetailsByExternalUrlAsync(resourceUri, cacheKey).Result;
                    break;
                case SourceType.eWIN:
                default:
                    this.logger.LogWarning("SourceType : Not Supported");
                    break;
            }

            if (scormContentDetail == null)
            {
                this.logger.LogWarning($"Original Request Path:{startingUrl} # : resourceExternalReference :{resourceExternalReference}: scormContentDetail NOT FOUND");
                context.Result = RuleResult.SkipRemainingRules;
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.HttpContext.Request.Path = "/NotFound";
                return;
            }
            if (scormContentDetail.EsrLinkType != Nhs.Models.Enums.EsrLinkType.EveryOne)
            {
                context.Result = RuleResult.SkipRemainingRules;
                context.HttpContext.Items.Add("ScormContentDetail", scormContentDetail);
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.HttpContext.Request.Path = "/Forbidden";
                return;
            }
            if (!scormContentDetail.IsActive || scormContentDetail.VersionStatus != Nhs.Models.Enums.VersionStatusEnum.Published)
            {
                context.Result = RuleResult.SkipRemainingRules;
                context.HttpContext.Items.Add("ScormContentDetail", scormContentDetail);
                context.HttpContext.Response.StatusCode = StatusCodes.Status410Gone;
                context.HttpContext.Request.Path = "/NotPublished";
                return;
            }

            if (pathSegments.Length == sourceSystem.ResourceIdentifierPosition)
            {
                uriBuilder.Path = uriBuilder.Path.EndsWith("/") ? $"{uriBuilder.Path}{scormContentDetail.ManifestUrl}" : $"{uriBuilder.Path}/{scormContentDetail.ManifestUrl}";

                if (context.HttpContext.Request.Headers.TryGetValue("X-FORWARDED-PROTO", out var uriScheme))
                {
                    uriBuilder.Scheme = uriScheme.ToString().ToLower() == "https" ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
                    uriBuilder.Port = -1; // default port for scheme
                }

                var rewrittenUrl = uriBuilder.Uri.ToString();

                context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
                context.HttpContext.Response.Headers[HeaderNames.Location] = rewrittenUrl;

                this.logger.LogInformation($"Original Request Path:{startingUrl} # Manifest file included {rewrittenUrl}");
                context.Result = RuleResult.EndResponse;
                return;
            }

            var rewrittenUrlStringBuilder = new StringBuilder();
            rewrittenUrlStringBuilder
                .Append(requestPath)
                .Replace(sourceSystem.ResourcePath, $"{this.settings.LearningHubContentVirtualPath}/")
                .Replace(resourceExternalReference, scormContentDetail.InternalResourceIdentifier);
            context.HttpContext.Request.Path = rewrittenUrlStringBuilder.ToString();

            match = Regex.Match(pathSegments.Last(), @"^.*\.(htm|html)$");

            if (match.Success)
            {
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                context.HttpContext.Response.Headers.Add("Expires", "-1");

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
