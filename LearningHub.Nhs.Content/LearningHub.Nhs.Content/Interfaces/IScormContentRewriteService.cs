// <copyright file="IScormContentRewriteService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Interfaces
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// Defines the <see cref="IScormContentRewriteService" />.
    /// </summary>
    public interface IScormContentRewriteService
    {
        /// <summary>
        /// The GetScormResourceDetail.
        /// </summary>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        Task<ScormContentServerViewModel> GetScormResourceDetailAsync(string requestUrl);
    }
}
