// <copyright file="ScormContentRewriteRule.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using LearningHub.Nhs.Content.Configuration;
    using LearningHub.Nhs.Content.Extensions;
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Content.Models;
    using LearningHub.Nhs.Models.Entities.Migration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;

    /// <summary>
    ///     Defines the <see cref="ScormContentRewriteRule" />.
    /// </summary>
    public class ScormContentRewriteRule : IRule
    {
        /// <summary>
        ///     Defines the AdapterSegment.
        /// </summary>
        private readonly string AdapterSegment =
            "/JSAdapter12_aspnet/JSAdapter12_asp/Oracle_SCORM_Adapter_JS.html?starting_url=";

        /// <summary>
        ///     Defines the CONTENT_SEGMENT.
        /// </summary>
        private readonly string ContentSegment = "/content/";

        /// <summary>
        /// Defines a string which is prefixed to all cache keys used by the LH Content Server......
        /// </summary>
        private const string KeyPrefix = "ContentServer";

        /// <summary>
        ///     Defines the scormContentRewriteService.
        /// </summary>
        private readonly IScormContentRewriteService scormContentRewriteService;

        /// <summary>
        ///     The settings...
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        ///     Defines the sourceSystems.
        /// </summary>
        private readonly List<MigrationSourceViewModel> sourceSystems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScormContentRewriteRule" /> class.
        /// </summary>
        /// <param name="scormContentRewriteService">The scormContentRewriteService<see cref="IScormContentRewriteService" />.</param>
        /// <param name="settings">The settings.</param>
        public ScormContentRewriteRule(IScormContentRewriteService scormContentRewriteService,
            IOptions<Settings> settings)
        {
            this.scormContentRewriteService = scormContentRewriteService;
            this.settings = settings.Value;
            sourceSystems = this.scormContentRewriteService.GetMigrationSourcesAsync($"{KeyPrefix}-Migration-Sources").Result;
        }

        /// <summary>
        ///     The ApplyRule.
        /// </summary>
        /// <param name="context">The context<see cref="RewriteContext" />.</param>
        public void ApplyRule(RewriteContext context)
        {
            var displayUrl = context.HttpContext.Request.GetDisplayUrl();

            var migrationSource = sourceSystems.GetMigrationSource(displayUrl);

            if(migrationSource == null)
                return;

            switch (migrationSource.SourceType())
            {
                case SourceType.LearningHub:
                    HandleLearningHubRequests(context, migrationSource);
                    break;
                case SourceType.eLR:
                    HandleExternalRequests(context, migrationSource);
                    break;
                case SourceType.eWIN:
                    Debug.WriteLine($"SourceType : {migrationSource.SourceType()} Not Supported");
                    break;
                default:
                    Debug.WriteLine("SourceType : Not Supported");
                    break;
            }
        }

        /// <summary>
        /// The HandleLearningHubRequests.
        /// </summary>
        /// <param name="context">The context<see cref="RewriteContext" />.</param>
        /// <param name="migrationSource"></param>
        private void HandleLearningHubRequests(RewriteContext context,
            MigrationSourceViewModel migrationSource)
        {
            // example https://cs1.e-learningforhealthcare.org.uk/content/ACM_001/
            // example https://content.learninghub.nhs.uk/content/07BD413E-5758-43C9-9117-F7A7F469452E/

            var requestPath = context.HttpContext.Request.Path.Value;
            var displayUrl = context.HttpContext.Request.GetDisplayUrl();

            if (!requestPath.Contains(migrationSource.ResourcePath))
                return;

            var pathSegments = requestPath.Split('/');

            if (pathSegments.Length < migrationSource.ResourceIdentifierPosition)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            var resourceExternalReference = pathSegments[migrationSource.ResourceIdentifierPosition - 1];

            var cacheKey = $"{KeyPrefix}{migrationSource.ResourcePath.Replace("/","-")}{resourceExternalReference}";

            var scormResourceDetail = scormContentRewriteService
                .GetScormContentDetailsByExternalReferenceAsync(resourceExternalReference, cacheKey).Result;
            if (scormResourceDetail == null)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var hostSegment = displayUrl.Substring(0, displayUrl.IndexOf(migrationSource.ResourcePath));
            var resourceUrl = $"{hostSegment}{ContentSegment}{scormResourceDetail.InternalResourceIdentifier}";
            
            var rewrittenUrl = $"{hostSegment}{AdapterSegment}{resourceUrl}/{scormResourceDetail.ManifestUrl}";
            context.HttpContext.Response.Headers[HeaderNames.Location] = rewrittenUrl;
            context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
            //context.HttpContext.Request.Path = $"{requestPath.Replace(resourceExternalReference, scormResourceDetail.InternalResourceIdentifier)}";
            context.Result = RuleResult.EndResponse;

            Debug.WriteLine($"Source :{migrationSource.Description} ---- Request Path:{requestPath} ---- Rewritten Path:{rewrittenUrl}");
        }

        /// <summary>
        ///     The HandleExternalSourceRequests.
        /// </summary>
        /// <param name="context">The context<see cref="RewriteContext" />.</param>
        /// <param name="migrationSource"></param>
        private void HandleExternalRequests(RewriteContext context, MigrationSourceViewModel migrationSource)
        {
            // example https://www.elearningrepository.nhs.uk/sites/default/files/lms/5742/Lesson_1/default.html
            
            var requestPath = context.HttpContext.Request.Path.Value;
            var displayUrl = context.HttpContext.Request.GetDisplayUrl();

            Debug.WriteLine($"Source :{migrationSource.Description} ---- Request Path:{requestPath}");

            var pathSegments = requestPath.Split('/');

            if (pathSegments.Length < migrationSource.ResourceIdentifierPosition)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            var resourceExternalReference = pathSegments[migrationSource.ResourceIdentifierPosition - 1];

            var cacheKey = $"{KeyPrefix}{migrationSource.ResourcePath.Replace("/", "-")}{resourceExternalReference}";

            var scormResourceDetail =
                scormContentRewriteService.GetScormContentDetailsByExternalUrlAsync(displayUrl, cacheKey).Result;

            if (scormResourceDetail == null)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var hostSegment = displayUrl.Substring(0, displayUrl.IndexOf(migrationSource.ResourcePath));
            var resourceUrl = $"{hostSegment}{ContentSegment}{scormResourceDetail.InternalResourceIdentifier}";

            var rewrittenUrl =
                $"{hostSegment}{AdapterSegment}{resourceUrl}/{scormResourceDetail.ManifestUrl}";
            context.HttpContext.Response.Headers[HeaderNames.Location] = rewrittenUrl;
            context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
            context.Result = RuleResult.EndResponse;
            Debug.WriteLine($"Source :{migrationSource.Description} ---- Request Path:{requestPath} ---- Rewritten Path:{rewrittenUrl}");
        }
    }
}