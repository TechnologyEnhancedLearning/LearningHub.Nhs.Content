namespace LearningHub.Nhs.Content
{
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Content.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Rewrite;
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
        /// Defines the contentSegment.
        /// </summary>
        internal readonly string contentSegment = $"/content/";

        /// <summary>
        /// Defines the adapterSegment.
        /// </summary>
        internal readonly string adapterSegment = $"/jsadapter12_aspnet/jsadapter12_asp/oracle_scorm_adapter_js.html?starting_url=";

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentRewriteRule"/> class.
        /// </summary>
        /// <param name="scormContentRewriteService">The scormContentRewriteService<see cref="IScormContentRewriteService"/>.</param>
        public ScormContentRewriteRule(IScormContentRewriteService scormContentRewriteService)
        {
            this.scormContentRewriteService = scormContentRewriteService;
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

            if (!requestPath.Contains(contentSegment))
                return;

            var pathSegments = requestPath.Split('/');
            
            if (pathSegments.Length < resourceSegmentPosition)
                return;

            //Index 2 resourcename
            var resourceSegment = pathSegments[2];

            string hostSegment = requestUrl.Substring(0, requestUrl.IndexOf(contentSegment));
            var resourceUrl = $"{hostSegment}{contentSegment}{resourceSegment}";
            ScormResourceDetail scormResourceDetail = scormContentRewriteService.GetScormResourceDetailAsync(resourceUrl).Result;

            if (scormResourceDetail == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            var rewrittenUrl = requestPath.Replace(resourceSegment, scormResourceDetail.ContentFolderName);
            if (pathSegments.Length > resourceSegmentPosition)
            {
                request.Path = $"{rewrittenUrl}";
            }
            else if (pathSegments.Length == resourceSegmentPosition)
            {
                var redirectedLocation = $"{hostSegment}{adapterSegment}{resourceUrl}/{scormResourceDetail.ManifestUrl}";
                response.StatusCode = StatusCodes.Status302Found;
                response.Headers[HeaderNames.Location] = redirectedLocation;
                context.Result = RuleResult.EndResponse;
            }

            return;
        }
    }
}
