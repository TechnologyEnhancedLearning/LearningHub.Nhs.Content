// <copyright file="IScormContentRewriteService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Interfaces
{
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IScormContentRewriteService" />.
    /// </summary>
    public interface IScormContentRewriteService
    {
        /// <summary>
        /// Gets the SCORM content details for a particular external url.
        /// </summary>
        /// <param name="resourceExternalUrl">The resourceExternalUrl<see cref="string"/>.</param>
        /// <param name="cacheKey">The cacheKey.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        Task<ScormContentServerViewModel> GetScormContentDetailsByExternalUrlAsync(string resourceExternalUrl, string cacheKey);

        /// <summary>
        /// The GetScormContentDetailsByExternalReference.
        /// </summary>
        /// <param name="resourceExternalReference">.</param>
        /// <param name="cacheKey">The cacheKey.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        Task<ScormContentServerViewModel> GetScormContentDetailsByExternalReferenceAsync(
            string resourceExternalReference, string cacheKey);

        /// <summary>
        /// The GetMigrationSourcesAsync.
        /// </summary>
        /// <param name="cacheKey">The cacheKey.</param>
        /// <returns>The <see cref="Task{List{MigrationSourceViewModel}}"/>.</returns>
        Task<List<MigrationSourceViewModel>> GetMigrationSourcesAsync(string cacheKey);

        /// <summary>
        /// The LogScormResourceReferenceEventAsync.
        /// </summary>
        /// <param name="scormResourceReferenceEvent">The scormResourceReferenceEvent<see cref="ScormResourceReferenceEventViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task LogScormResourceReferenceEventAsync(ScormResourceReferenceEventViewModel scormResourceReferenceEvent);
    }
}
