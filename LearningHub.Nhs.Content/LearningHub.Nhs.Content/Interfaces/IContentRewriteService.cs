// <copyright file="IContentRewriteService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Interfaces
{
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Resource;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IContentRewriteService" />.
    /// </summary>
    public interface IContentRewriteService
    {
        /// <summary>
        /// Gets the content details for a particular external url.
        /// </summary>
        /// <param name="resourceExternalUrl">The resourceExternalUrl<see cref="string"/>.</param>
        /// <param name="cacheKey">The cacheKey.</param>
        /// <returns>The <see cref="ContentServerViewModel"/>.</returns>
        Task<ContentServerViewModel> GetContentDetailsByExternalUrlAsync(string resourceExternalUrl, string cacheKey);

        /// <summary>
        /// The GetContentDetailsByExternalReference.
        /// </summary>
        /// <param name="resourceExternalReference">.</param>
        /// <param name="cacheKey">The cacheKey.</param>
        /// <returns>The <see cref="ContentServerViewModel"/>.</returns>
        Task<ContentServerViewModel> GetContentDetailsByExternalReferenceAsync(
            string resourceExternalReference, string cacheKey);

        /// <summary>
        /// The GetMigrationSourcesAsync.
        /// </summary>
        /// <param name="cacheKey">The cacheKey.</param>
        /// <returns>The <see cref="List{MigrationSourceViewModel}"/>.</returns>
        Task<List<MigrationSourceViewModel>> GetMigrationSourcesAsync(string cacheKey);

        /// <summary>
        /// The LogResourceReferenceEventAsync.
        /// </summary>
        /// <param name="resourceReferenceEvent">The resourceReferenceEvent<see cref="ResourceReferenceEventViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task LogResourceReferenceEventAsync(ResourceReferenceEventViewModel resourceReferenceEvent);
    }
}
