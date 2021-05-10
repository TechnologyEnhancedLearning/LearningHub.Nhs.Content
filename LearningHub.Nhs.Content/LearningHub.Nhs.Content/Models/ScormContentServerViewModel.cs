using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Content.Models
{
    /// <summary>
    /// Defines the <see cref="ScormContentServerViewModel" />.
    /// </summary>
    public class ScormContentServerViewModel
    {
        /// <summary>
        /// Gets or sets the ExternalUrl. Could be an LH one or an historic one for a migrated resource (i.e. a totally different domain).
        /// </summary>
        public string ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets the ContentFilePath.
        /// </summary>
        public string ContentFilePath { get; set; }

        /// <summary>
        /// Gets or sets the ManifestUrl.
        /// </summary>
        public string ManifestUrl { get; set; }
    }
}
