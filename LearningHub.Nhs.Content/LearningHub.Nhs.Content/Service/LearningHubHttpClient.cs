// <copyright file="LearningHubHttpClient.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Content.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Content.Configuration;
    using LearningHub.Nhs.Content.Interfaces;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The learning hub http client. Simplified from those in WebUI and AdminUI because the content server is never authenticated.
    /// Simply passes in a client key in header. All WebAPI methods called have the AuthorizeOrCallFromLH attribute.
    /// </summary>
    public class LearningHubHttpClient : ILearningHubHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubHttpClient"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        public LearningHubHttpClient(
            IOptions<Settings> settings,
            HttpClient client)
        {
            this.settings = settings.Value;
            this.httpClient = client;
            this.Initialise();
        }

        /// <summary>
        /// The get client async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public HttpClient GetClient()
        {
            return this.httpClient;
        }

        /// <summary>
        /// The initialise.
        /// </summary>
        private void Initialise()
        {
            this.httpClient.BaseAddress = new Uri(this.settings.LearningHubApiUrl);
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add(
                "Client-Identity-Key",
                this.settings.ContentServerClientIdentityKey);
        }
    }
}