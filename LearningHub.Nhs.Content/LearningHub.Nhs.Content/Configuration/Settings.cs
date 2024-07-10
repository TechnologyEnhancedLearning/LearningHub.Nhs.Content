// <copyright file="Settings.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Configuration
{
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
        /// Gets or sets the LearningHubContentPhysicalPath.
        /// </summary>
        public string LearningHubContentPhysicalPath { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubContentVirtualPath.
        /// </summary>
        public string LearningHubContentVirtualPath { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubContentLogTable.
        /// </summary>
        public string LearningHubContentLogTable { get; set; }

        /// <summary>
        /// Gets or sets the LearningHubApiUrl.
        /// </summary>
        public string LearningHubApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the ContentServerClientIdentityKey.
        /// </summary>
        public string ContentServerClientIdentityKey { get; set; }

        /// <summary>
        /// Gets or sets the EnableSuccessMessageForLogResourceRefrence 
        /// </summary>
        public string EnableSuccessMessageForLogResourceReference { get; set; }

        public string EnableAzureFileStorageRESTAPIaccess { get; set; }

        public string AzureFileStorageConnectionString { get; set; }

        public string AzureFileStorageResourceShareName { get; set; }

        public string AzureSourceArchiveStorageConnectionString {  get; set; }

        public string LearningHubContentServerUrlRedis { get; set; }

        public string LearningHubContentVirtualPathRedis { get; set; }
  }
}
