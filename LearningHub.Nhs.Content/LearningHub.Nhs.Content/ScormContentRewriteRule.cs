// <copyright file="ScormContentRewriteRule.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content
{
    using LearningHub.Nhs.Content.Configuration;
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Content.Models;
    ////using LearningHub.Nhs.Models.Resource;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;

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
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Defines the contentSegment.
        /// </summary>
        private readonly string contentSegment = $"/content/";

        /// <summary>
        /// Defines the adapterSegment.
        /// </summary>
        private readonly string adapterSegment = $"/jsadapter12_aspnet/jsadapter12_asp/oracle_scorm_adapter_js.html?starting_url=";

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentRewriteRule"/> class.
        /// </summary>
        /// <param name="scormContentRewriteService">The scormContentRewriteService<see cref="IScormContentRewriteService"/>.</param>
        /// <param name="settings">The settings.</param>
        public ScormContentRewriteRule(IScormContentRewriteService scormContentRewriteService, IOptions<Settings> settings)
        {
            this.scormContentRewriteService = scormContentRewriteService;
            this.settings = settings.Value;
        }

        /// <summary>
        /// The ApplyRule.
        /// </summary>
        /// <param name="context">The context<see cref="RewriteContext"/>.</param>
        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;
            var requestPath = request.Path.Value;
            var requestUrl = request.GetDisplayUrl();
            const int resourceSegmentPosition = 3;

            // TEMP:
            if (requestUrl.EndsWith("test.html"))
            {
                return;
            }

            // Work out if the request is for an LH external URL or an historic one.
            if (requestUrl.StartsWith(this.settings.LearningHubContentServerUrl))
            {
                // LH external URL - Code is direct from POC.
                if (!requestPath.Contains(this.contentSegment))
                {
                    return;
                }

                var pathSegments = requestPath.Split('/');

                if (pathSegments.Length < resourceSegmentPosition)
                {
                    return;
                }

                // Index 2 resourcename
                var resourceSegment = pathSegments[2];

                string hostSegment = requestUrl.Substring(0, requestUrl.IndexOf(this.contentSegment));
                var resourceUrl = $"{hostSegment}{this.contentSegment}{resourceSegment}";
                ScormContentServerViewModel scormResourceDetail = this.scormContentRewriteService.GetScormResourceDetailAsync(resourceUrl).Result;

                if (scormResourceDetail == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                var rewrittenUrl = requestPath.Replace(resourceSegment, scormResourceDetail.ContentFilePath);
                if (pathSegments.Length > resourceSegmentPosition)
                {
                    request.Path = $"{rewrittenUrl}";
                }
                else if (pathSegments.Length == resourceSegmentPosition)
                {
                    var redirectedLocation = $"{hostSegment}{this.adapterSegment}{resourceUrl}/{scormResourceDetail.ManifestUrl}";
                    response.StatusCode = StatusCodes.Status302Found;
                    response.Headers[HeaderNames.Location] = redirectedLocation;
                    context.Result = RuleResult.EndResponse;
                }
            }
            else
            {
                // Historic external URL - new code.
                ScormContentServerViewModel scormResourceDetail = this.scormContentRewriteService.GetScormResourceDetailAsync(requestUrl).Result;

                if (scormResourceDetail == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                var rewrittenUrl = requestUrl.Replace(scormResourceDetail.ExternalUrl, $"{this.settings.LearningHubContentServerUrl}/content/{scormResourceDetail.ContentFilePath}/");
                request.Path = $"{rewrittenUrl}";
            }

            return;
        }
    }
}
