// <copyright file="ScormContentRewriteService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Models.Resource;
    using Newtonsoft.Json;

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
        /// Defines the scormContentRegister.
        /// </summary>
        private Dictionary<string, ScormContentServerViewModel> scormContentRegister;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentRewriteService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient.</param>
        public ScormContentRewriteService(ILearningHubHttpClient learningHubHttpClient)
        {
            this.learningHubHttpClient = learningHubHttpClient;

            this.scormContentRegister = new Dictionary<string, ScormContentServerViewModel>
            {
                ////{ "https://localhost:44373/content/EFM_2017_01", new ScormContentServerViewModel { ContentFilePath = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                ////{ "https://localhost:44373/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                ////{ "https://localhost:44373/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
                ////{ "http://localhost/LearningHub.Nhs.Content/content/EFM_2017_01", new ScormResourceDetail { ContentFolderName = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                ////{ "http://localhost/LearningHub.Nhs.Content/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                ////{ "http://localhost/LearningHub.Nhs.Content/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
                ////{ "https://dev-test-learninghub-nhs-content.azurewebsites.net/content/EFM_2017_01", new ScormResourceDetail { ContentFolderName = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                ////{ "https://dev-test-learninghub-nhs-content.azurewebsites.net/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                ////{ "https://dev-test-learninghub-nhs-content.azurewebsites.net/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
                ////{ "https://learninghubnhscontent.azurewebsites.net/content/EFM_2017_01", new ScormResourceDetail { ContentFolderName = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                ////{ "https://learninghubnhscontent.azurewebsites.net/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                ////{ "https://learninghubnhscontent.azurewebsites.net/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
            };
        }

        /// <summary>
        /// The GetScormResourceDetail.
        /// </summary>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        public async Task<ScormContentServerViewModel> GetScormResourceDetailAsync(string requestUrl)
        {
            var scormResourceDetail = this.scormContentRegister.SingleOrDefault(scr => requestUrl.StartsWith(scr.Key)).Value;

            // If not found in cache get from WebAPI.
            if (scormResourceDetail == null)
            {
                scormResourceDetail = await this.GetScormContentDetailsFromApiAsync(requestUrl);

                if (scormResourceDetail != null)
                {
                    this.scormContentRegister.Add(scormResourceDetail.ExternalUrl, scormResourceDetail);
                }
            }

            return scormResourceDetail;
        }

        /// <summary>
        /// The GetScormContentDetailsAsync.
        /// </summary>
        /// <param name="externalUrl">The externalUrl.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<ScormContentServerViewModel> GetScormContentDetailsFromApiAsync(string externalUrl)
        {
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
