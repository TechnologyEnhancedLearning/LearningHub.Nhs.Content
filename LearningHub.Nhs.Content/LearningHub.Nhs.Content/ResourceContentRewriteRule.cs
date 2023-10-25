// <copyright file="ResourceContentRewriteRule.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

using System.Linq;

namespace LearningHub.Nhs.Content
{
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
    /// Defines the <see cref="ResourceContentRewriteRule" />.
    /// </summary>
    public class ResourceContentRewriteRule : IRule
    {
        /// <summary>
        /// Defines the ContentRewriteService.
        /// </summary>
        private readonly IContentRewriteService contentRewriteService;

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
        private readonly ILogger<ResourceContentRewriteRule> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceContentRewriteRule"/> class.
        /// </summary>
        /// <param name="contentRewriteService">The contentRewriteService<see cref="IContentRewriteService" />.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">logger.</param>
        public ResourceContentRewriteRule(IContentRewriteService contentRewriteService,
            IOptions<Configuration.Settings> settings, ILogger<ResourceContentRewriteRule> logger)
        {
            this.contentRewriteService = contentRewriteService;
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

                this.sourceSystems = this.contentRewriteService
                    .GetMigrationSourcesAsync($"Migration-Sources").Result;
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
            context.HttpContext.Request.Headers.TryGetValue("Referer", out var httpRefererer);
            context.HttpContext.Request.Headers.TryGetValue("X-Original-For", out var ipAddress);
            
            var logEvent = new ResourceReferenceEventViewModel
            {
                Url = startingUrl,
                HttpRefererer = httpRefererer,
                IPAddress = ipAddress
            };

            var uriBuilder = new UriBuilder(startingUrl);
            
            var pathSegments = requestPath.Split('/');
            if (pathSegments.Length < sourceSystem.ResourceIdentifierPosition)
            {
                this.logger.LogWarning($"FullResourceUrl {startingUrl} # Request Path {requestPath} # INVALID PATH SEGMENTS pathSegmentsLength: {pathSegments.Length} # Expected PathSegments: {sourceSystem.ResourceIdentifierPosition}");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var resourceExternalReference = pathSegments[sourceSystem.ResourceIdentifierPosition - 1];
            var match = Regex.Match(resourceExternalReference, sourceSystem.ResourceRegEx, RegexOptions.IgnoreCase);

            var cacheKey = $"{sourceSystem.Description}_{resourceExternalReference}";
            ContentServerViewModel contentDetail = null;

            if (match.Success)
            {
                switch (sourceSystem.SourceType())
                {
                    case SourceType.LearningHub:
                        contentDetail = contentRewriteService.GetContentDetailsByExternalReferenceAsync(resourceExternalReference, cacheKey).Result;
                        break;
                    case SourceType.eLR:
                        var resourceUri = $"{sourceSystem.ResourcePath}{resourceExternalReference}/";
                        contentDetail = contentRewriteService.GetContentDetailsByExternalUrlAsync(resourceUri, cacheKey).Result;
                        break;
                    case SourceType.eWIN:
                    default:
                        this.logger.LogWarning("SourceType : Not Supported");
                        break;
                }
            }
            
            if (contentDetail == null)
            {
                this.logger.LogWarning($"Original Request Path:{startingUrl} # : resourceExternalReference :{resourceExternalReference}: contentDetail NOT FOUND");
                context.Result = RuleResult.SkipRemainingRules;
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                context.HttpContext.Response.Headers.Add("Expires", "-1");
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.HttpContext.Request.Path = "/NotFound";
                logEvent.ResourceReferenceEventType = Nhs.Models.Enums.ResourceReferenceEventTypeEnum.Status404NotFound;
                await this.contentRewriteService.LogResourceReferenceEventAsync(logEvent);
                return;
            }
            
            if (contentDetail.VersionStatus != Nhs.Models.Enums.VersionStatusEnum.Published)
            {
                context.Result = RuleResult.SkipRemainingRules;
                context.HttpContext.Items.Add("ContentDetail", contentDetail);             
                context.HttpContext.Response.StatusCode = StatusCodes.Status410Gone;
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                context.HttpContext.Response.Headers.Add("Expires", "-1");
                context.HttpContext.Request.Path = "/NotPublished";
                logEvent.ResourceReferenceId = contentDetail.ResourceReferenceId;
                logEvent.ResourceReferenceEventType = Nhs.Models.Enums.ResourceReferenceEventTypeEnum.Status410Gone;
                await this.contentRewriteService.LogResourceReferenceEventAsync(logEvent);
                return;
            }

            if (!contentDetail.IsActive || contentDetail.EsrLinkType == Nhs.Models.Enums.EsrLinkType.NotAvailable)
            {
                context.Result = RuleResult.SkipRemainingRules;
                context.HttpContext.Items.Add("ContentDetail", contentDetail);
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                context.HttpContext.Response.Headers.Add("Expires", "-1");
                context.HttpContext.Request.Path = "/Forbidden";
                logEvent.ResourceReferenceId = contentDetail.ResourceReferenceId;
                logEvent.ResourceReferenceEventType = Nhs.Models.Enums.ResourceReferenceEventTypeEnum.Status403Forbidden;
                await this.contentRewriteService.LogResourceReferenceEventAsync(logEvent);
                return;
            }

            if (pathSegments.Length == sourceSystem.ResourceIdentifierPosition)
            {
                uriBuilder.Path = uriBuilder.Path.EndsWith("/") ? $"{uriBuilder.Path}{contentDetail.DefaultUrl}" : $"{uriBuilder.Path}/{contentDetail.DefaultUrl}";

                if (context.HttpContext.Request.Headers.TryGetValue("X-FORWARDED-PROTO", out var uriScheme))
                {
                    uriBuilder.Scheme = uriScheme.ToString().ToLower() == "https" ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
                    uriBuilder.Port = -1; // default port for scheme
                }

                var rewrittenUrl = uriBuilder.Uri.ToString();

                context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
                context.HttpContext.Response.Headers[HeaderNames.Location] = rewrittenUrl;

                this.logger.LogInformation($"Original Request Path:{startingUrl} # Default file included {rewrittenUrl}");
                context.Result = RuleResult.EndResponse;
                return;
            }

            var rewrittenUrlStringBuilder = new StringBuilder();
            rewrittenUrlStringBuilder
                .Append(requestPath)
                .Replace(sourceSystem.ResourcePath, $"{this.settings.LearningHubContentVirtualPath}/")
                .Replace(resourceExternalReference, contentDetail.InternalResourceIdentifier);
            context.HttpContext.Request.Path = rewrittenUrlStringBuilder.ToString();

            match = Regex.Match(pathSegments.Last(), @"^.*\.(htm|html)$");

            if (match.Success || contentDetail.DefaultUrl == pathSegments.Last())
            {
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                context.HttpContext.Response.Headers.Add("Expires", "-1");

                logEvent.ResourceReferenceId = contentDetail.ResourceReferenceId;
                logEvent.ResourceReferenceEventType = Nhs.Models.Enums.ResourceReferenceEventTypeEnum.Status200OK;
                await this.contentRewriteService.LogResourceReferenceEventAsync(logEvent);

                this.logger.LogInformation(
                    $"Source System :{sourceSystem.Description} # Original Request Path:{startingUrl} # Rewritten Path:{rewrittenUrlStringBuilder}");
            }
            else
            {
                context.HttpContext.Response.Headers.Add("Expires", DateTime.Now.AddHours(1).ToUniversalTime().ToString("r"));               
                this.logger.LogTrace(
                    $"Source System :{sourceSystem.Description} # Original Request Path:{startingUrl} # Rewritten Path:{rewrittenUrlStringBuilder}");
            }
        }
    }
}
