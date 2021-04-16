namespace LearningHub.Nhs.Content.Interfaces
{
    using LearningHub.Nhs.Content.Models;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IScormContentRewriteService" />.
    /// </summary>
    public interface IScormContentRewriteService
    {
        /// <summary>
        /// The GetScormResourceDetail.
        /// </summary>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormResourceDetail"/>.</returns>
        Task<ScormResourceDetail> GetScormResourceDetailAsync(string requestUrl);
    }
}
