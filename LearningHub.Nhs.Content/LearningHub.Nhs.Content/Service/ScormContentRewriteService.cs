// <copyright file="ScormContentRewriteService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Service
{
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Content.Configuration;
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ScormContentRewriteService" />.
    /// </summary>
    public class ScormContentRewriteService : IScormContentRewriteService
    {
        /// <summary>
        /// Defines the learningHubHttpClient.
        /// </summary>
        private ILearningHubHttpClient learningHubHttpClient;

        /// <summary>
        /// Defines the Redis cacheService.......
        /// </summary>
        private ICacheService cacheService;

        /// <summary>
        /// The settings....
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentRewriteService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="settings">The settings<see cref="IOptions{Settings}"/>.</param>
        public ScormContentRewriteService(ILearningHubHttpClient learningHubHttpClient, ICacheService cacheService, IOptions<Settings> settings)
        {
            this.learningHubHttpClient = learningHubHttpClient;
            this.cacheService = cacheService;
            this.settings = settings.Value;
        }

        /// <summary>
        /// The LogScormResourceReferenceEventAsync.
        /// </summary>
        /// <param name="scormResourceReferenceEvent">The scormResourceReferenceEvent<see cref="ScormResourceReferenceEvent"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task LogScormResourceReferenceEventAsync(ScormResourceReferenceEventViewModel scormResourceReferenceEvent)
        {
            var client = await this.learningHubHttpClient.GetClientAsync();
            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(scormResourceReferenceEvent), Encoding.UTF8, "application/json");
            var request = $"ScormContentServer/LogScormResourceReferenceEvent";
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

             if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }

        /// <summary>
        /// The GetMigrationSourcesAsync.
        /// </summary>
        /// <param name="cacheKey">.</param>
        /// <returns>The <see cref="Task{List{MigrationSourceViewModel}}"/>.</returns>
        public async Task<List<MigrationSourceViewModel>> GetMigrationSourcesAsync(string cacheKey)
        {
            var migrationSources = this.cacheService.GetAsync<List<MigrationSourceViewModel>>(cacheKey).Result;

            if (migrationSources != null)
                return migrationSources;

            migrationSources = await this.ApiGetMigrationSourceAsync();
            // Also include learning hub by default as this is not part of a migration
            migrationSources?.Add(new MigrationSourceViewModel
            {
                HostName = this.settings.LearningHubContentServerUrl,
                Description = "LearningHub",
                ResourcePath = "/content/",
                ResourceIdentifierPosition = 3,
                ResourceRegEx = "[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?"
            });
            await this.cacheService.SetAsync(cacheKey, migrationSources);
            return migrationSources;
        }

        /// <summary>
        /// The GetScormContentDetailsByExternalUrlAsync.
        /// </summary>
        /// <param name="resourceExternalUrl">The resourceExternalUrl<see cref="string"/>.</param>
        /// <param name="cacheKey">.</param>
        /// <returns>The <see cref="Task{ScormContentServerViewModel}"/>.</returns>
        public async Task<ScormContentServerViewModel> GetScormContentDetailsByExternalUrlAsync(string resourceExternalUrl, string cacheKey)
        {
            var contentServerResponse = this.cacheService.GetAsync<ScormContentServerViewModel>(cacheKey).Result;
            if (contentServerResponse != null)
            {
                return contentServerResponse;
            }

            contentServerResponse = await this.ApiGetScormContentDetailsByExternalUrlAsync(resourceExternalUrl);
            if (contentServerResponse != null)
            {
                await this.cacheService.SetAsync(cacheKey, contentServerResponse);
            }
            return contentServerResponse;
        }

        /// <summary>
        /// The GetScormContentDetailsByExternalReferenceAsync.
        /// </summary>
        /// <param name="resourceExternalReference">.</param>
        /// <param name="cacheKey">The externalReference<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ScormContentServerViewModel}"/>.</returns>
        public async Task<ScormContentServerViewModel> GetScormContentDetailsByExternalReferenceAsync(
            string resourceExternalReference, string cacheKey)
        {
            var contentServerResponse = this.cacheService.GetAsync<ScormContentServerViewModel>(cacheKey).Result;
            if (contentServerResponse != null)
            {
                return contentServerResponse;
            }

            contentServerResponse = await this.ApiGetScormContentDetailsByExternalReferenceAsync(resourceExternalReference);
            if (contentServerResponse != null)
            {
                await this.cacheService.SetAsync(cacheKey, contentServerResponse);
            }
            return contentServerResponse;
        }

        /// <summary>
        /// The ApiGetMigrationSourceAsync.
        /// </summary>
        /// <returns>The <see cref="Task{List{MigrationSourceViewModel}}"/>.</returns>
        private async Task<List<MigrationSourceViewModel>> ApiGetMigrationSourceAsync()
        {
            var client = await this.learningHubHttpClient.GetClientAsync();
            List<MigrationSourceViewModel> migrationSources = null;
            var request = $"migration/get-migration-sources";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                migrationSources = JsonConvert.DeserializeObject<List<MigrationSourceViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return migrationSources;
        }

        /// <summary>
        /// The ApiGetScormContentDetailsByExternalUrlAsync.
        /// </summary>
        /// <param name="resourceExternalUrl">The resourceExternalUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ScormContentServerViewModel}"/>.</returns>
        private async Task<ScormContentServerViewModel> ApiGetScormContentDetailsByExternalUrlAsync(string resourceExternalUrl)
        {
            ScormContentServerViewModel viewmodel = null;

            var client = await this.learningHubHttpClient.GetClientAsync();

            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(resourceExternalUrl), Encoding.UTF8, "application/json");
            var request = $"ScormContentServer/GetScormContentDetailsByExternalUrl";
            var response = await client.PostAsync(request, content).ConfigureAwait(false);

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

        /// <summary>
        /// The ApiGetScormContentDetailsByExternalReferenceAsync.
        /// </summary>
        /// <param name="externalReference">The externalReference<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ScormContentServerViewModel}"/>.</returns>
        private async Task<ScormContentServerViewModel> ApiGetScormContentDetailsByExternalReferenceAsync(string externalReference)
        {
            ScormContentServerViewModel viewmodel = null;

            var client = await this.learningHubHttpClient.GetClientAsync();

            var request = $"ScormContentServer/GetScormContentDetailsByExternalReference/{externalReference}";
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
