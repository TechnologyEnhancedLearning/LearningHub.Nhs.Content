using System.Linq;

namespace LearningHub.Nhs.Content.Extensions
{
    using LearningHub.Nhs.Content.Models;
    using LearningHub.Nhs.Models.Entities.Migration;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="SourceSystemExtensions" />.
    /// </summary>
    public static class SourceSystemExtensions
    {
        /// <summary>
        /// The SourceType.
        /// </summary>
        /// <param name="migrationSourceViewModel">The migrationSourceViewModel<see cref="MigrationSourceViewModel"/>.</param>
        /// <returns>The <see cref="SourceType"/>.</returns>
        public static SourceType SourceType(this MigrationSourceViewModel migrationSourceViewModel)
        {
            return (SourceType)migrationSourceViewModel.Id;
        }

        /// <summary>
        /// The GetMigrationSource.
        /// </summary>
        /// <param name="sourceTypes">The sourceTypes<see cref="List{MigrationSourceViewModel}"/>.</param>
        /// <param name="requestUrl">The requestUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="MigrationSourceViewModel"/>.</returns>
        public static MigrationSourceViewModel GetMigrationSource(this List<MigrationSourceViewModel> sourceTypes, string requestUrl)
        {
            foreach (var migrationSource in sourceTypes.Where(x => x.ResourcePath != null && requestUrl.Contains(x.ResourcePath)))
            {
                return migrationSource;
            }

            return null;
        }
    }
}
