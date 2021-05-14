// <copyright file="ScormContentRewriteService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Service
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Content.Interfaces;    
    using LearningHub.Nhs.Models.Resource;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ScormContentRewriteService" />.
    /// </summary>
    public class ScormContentRewriteService : IScormContentRewriteService
    {
        /// <summary>
        /// Defines a string which is prefixed to all cache keys used by the LH Content Server.
        /// </summary>
        private const string KeyPrefix = "ContentServer-";

        /// <summary>
        /// Defines the learningHubHttpClient.
        /// </summary>
        private ILearningHubHttpClient learningHubHttpClient;

        /// <summary>
        /// Defines the Redis cacheService.
        /// </summary>
        private ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentRewriteService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient.</param>
        /// <param name="cacheService">The cacheService.</param>
        public ScormContentRewriteService(ILearningHubHttpClient learningHubHttpClient, ICacheService cacheService)
        {
            this.learningHubHttpClient = learningHubHttpClient;
            this.cacheService = cacheService;
        }

        /// <summary>
        /// The GetScormResourceDetail.
        /// </summary>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        public async Task<ScormContentServerViewModel> GetScormResourceDetailAsync(string requestUrl)
        {
            var scormResourceDetail = await this.cacheService.GetOrCreateAsync($"{KeyPrefix}{requestUrl}", () => this.GetScormContentDetailsFromApiAsync(requestUrl).Result);

            return scormResourceDetail;
        }

        /// <summary>
        /// The GetScormContentDetailsAsync.
        /// </summary>
        /// <param name="externalUrl">The externalUrl.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<ScormContentServerViewModel> GetScormContentDetailsFromApiAsync(string externalUrl)
        {
            // TEMP:
            return new ScormContentServerViewModel
            {
                ContentFilePath = "4bdc56b4-2148-4383-a9d7-3ea79b9bf2ee",
                ExternalUrl = "https://localhost:44737/content/4bdc56b4-2148-4383-a9d7-3ea79b9bf2ee",
                ManifestUrl = "index_lms.html"
            };

            ScormContentServerViewModel viewmodel = null;

            var client = await this.learningHubHttpClient.GetClientAsync();

            var encodedUrl = HttpUtility.UrlEncode(externalUrl);
            var request = $"ScormContentServer/GetScormContentDetailsByExternalUrl/{encodedUrl}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<ScormContentServerViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
