namespace LearningHub.Nhs.Content.Pages
{
    using LearningHub.Nhs.Content.Models;
    using LearningHub.Nhs.Models.Resource;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Defines the <see cref="ForbiddenModel" />.
    /// </summary>
    public class ForbiddenModel : PageModel
    {
        /// <summary>
        /// Gets or sets the ScormContentDetail.
        /// </summary>
        public ScormContentServerViewModel ScormContentDetail { get; set; }

        /// <summary>
        /// Defines the Settings.
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenModel"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        public ForbiddenModel(IConfiguration configuration)
        {
            this.Settings = configuration.GetSection("Settings").Get<Settings>();
        }

        /// <summary>
        /// The OnGet.
        /// </summary>
        public void OnGet()
        {
            this.ScormContentDetail = (ScormContentServerViewModel)HttpContext.Items["ScormContentDetail"];
        }
    }
}
