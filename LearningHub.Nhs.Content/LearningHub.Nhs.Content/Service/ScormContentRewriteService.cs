namespace LearningHub.Nhs.Content.Service
{
    using LearningHub.Nhs.Content.Interfaces;
    using LearningHub.Nhs.Content.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ScormContentRewriteService" />.
    /// </summary>
    public class ScormContentRewriteService : IScormContentRewriteService
    {
        /// <summary>
        /// Defines the scormContentRegister.
        /// </summary>
        internal Dictionary<string, ScormResourceDetail> scormContentRegister;

        /// <summary>
        /// Defines the scormContentCacheRegister.
        /// </summary>
        internal Dictionary<string, ScormResourceDetail> scormContentCacheRegister;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentRewriteService"/> class.
        /// </summary>
        public ScormContentRewriteService()
        {
            //:TODO Replace with actual database
            scormContentRegister = new Dictionary<string, ScormResourceDetail>
            {
                { "https://localhost:44373/content/EFM_2017_01", new ScormResourceDetail { ContentFolderName = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                { "https://localhost:44373/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                { "https://localhost:44373/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
                { "http://localhost/LearningHub.Nhs.Content/content/EFM_2017_01", new ScormResourceDetail { ContentFolderName = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                { "http://localhost/LearningHub.Nhs.Content/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                { "http://localhost/LearningHub.Nhs.Content/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
                { "https://dev-test-learninghub-nhs-content.azurewebsites.net/content/EFM_2017_01", new ScormResourceDetail { ContentFolderName = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                { "https://dev-test-learninghub-nhs-content.azurewebsites.net/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                { "https://dev-test-learninghub-nhs-content.azurewebsites.net/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
                { "https://learninghubnhscontent.azurewebsites.net/content/EFM_2017_01", new ScormResourceDetail { ContentFolderName = "5f1a449b-0d34-4bf4-904d-b69c04bb712b", ManifestUrl = "d/index.html" } },
                { "https://learninghubnhscontent.azurewebsites.net/content/ACU_03_001", new ScormResourceDetail { ContentFolderName = "689d9f29-ce4d-4e65-b551-fa8fcc8086a1", ManifestUrl = "d/ELFH_Scenario/32/session.html" } },
                { "https://learninghubnhscontent.azurewebsites.net/content/MLD_03_003", new ScormResourceDetail { ContentFolderName = "6cdae485-679d-4285-82ab-6beff6ab9950", ManifestUrl = "index_lms.html" } },
            };
            scormContentCacheRegister = new Dictionary<string, ScormResourceDetail>();
        }

        /// <summary>
        /// The GetScormResourceDetail.
        /// </summary>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormResourceDetail"/>.</returns>
        public async Task<ScormResourceDetail> GetScormResourceDetailAsync(string requestUrl)
        {
            return this.GetCachedScormRescourceDetails(requestUrl);
        }

        /// <summary>
        /// The GetScormRescourceDetails.
        /// </summary>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormResourceDetail"/>.</returns>
        private ScormResourceDetail GetScormRescourceDetails(string requestUrl)
        {
            //:TODO Replace with actual database/Api call
            var scormResourceDetail = scormContentRegister.SingleOrDefault(scr => scr.Key == requestUrl).Value;

            if (scormResourceDetail != null && !scormContentCacheRegister.ContainsKey(requestUrl))
            {
                this.scormContentCacheRegister.Add(requestUrl, scormResourceDetail);
            }
            return scormResourceDetail;
        }

        /// <summary>
        /// The GetCachedScormRescourceDetails.
        /// </summary>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormResourceDetail"/>.</returns>
        private ScormResourceDetail GetCachedScormRescourceDetails(string requestUrl)
        {
            //:Do we really need Redis Cache?
            var scormResourceDetail = scormContentCacheRegister.SingleOrDefault(scr => scr.Key == requestUrl).Value;

            //If not found in cache get from backend sources
            if (scormResourceDetail == null)
            {
                scormResourceDetail = this.GetScormRescourceDetails(requestUrl);
            }
            return scormResourceDetail;
        }
    }
}
