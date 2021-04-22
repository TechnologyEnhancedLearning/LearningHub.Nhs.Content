// <copyright file="Settings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Configuration
{
    using System;

    /// <summary>
    /// Defines the <see cref="Settings" />.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
        }

        /// <summary>
        /// Gets or sets the LearningHubContentServerUrl.
        /// </summary>
        public string LearningHubContentServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubApiUrl.
        /// </summary>
        public string LearningHubApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the ContentServerClientIdentityKey.
        /// </summary>
        public string ContentServerClientIdentityKey { get; set; }
    }
}
