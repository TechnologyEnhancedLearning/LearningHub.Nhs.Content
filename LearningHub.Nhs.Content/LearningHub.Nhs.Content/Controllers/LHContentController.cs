namespace LearningHub.Nhs.Content.Controllers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Content.Configuration;
    using LearningHub.Nhs.WebUI.EventSource;
    using LearningHub.Nhs.Content.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="LearningSessionsController" />.
    /// </summary>
    // [Route("lhcontent")]
    public class LHContentController : Controller
    {
        // private readonly IResourceService resourceService;
        private readonly IFileService fileService;
        private readonly IOptions<Settings> settings;
        // private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningSessionsController"/> class.
        /// </summary>
        /// <param name="resourceService">Resource service.</param>
        /// <param name="fileService">File service.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="userGroupService">userGroupService.</param>
        public LHContentController(
            // IResourceService resourceService,
            IFileService fileService,
            IOptions<Settings> settings)
            // IUserGroupService userGroupService)
        {
            // this.resourceService = resourceService;
            this.fileService = fileService;
            this.settings = settings;
            // this.userGroupService = userGroupService;
        }

        /// <summary>
        /// The Scorm.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>bool.</returns>
        public async Task<IActionResult> Scorm(int id)
        {
            //var rv = await this.resourceService.GetItemByIdAsync(id);
            //if (rv != null)
            //{
            //    this.ViewBag.FilePath = $"/ScormContent/{rv.ScormDetails.ContentFilePath}/{rv.ScormDetails.ScormManifest.ManifestUrl}";
            //}

            //this.ViewBag.ResourceReferenceId = id;
            //this.ViewBag.KeepUserSessionAliveInterval = Convert.ToInt32(10) * 60000;
            ////this.ViewBag.UseTraceWindow = await this.userGroupService.UserHasPermissionAsync("Scorm_Trace_Window");

            return this.View();
        }

        /// <summary>
        /// The ScormContent.
        /// </summary>
        /// <param name="filePath">filePath.</param>
        /// <returns>bool.</returns>
        //// [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 0, NoStore = true)] // disable caching
        [AllowAnonymous]
        [HttpGet]
        [Route("lhcontent/{*filePath}")]
        public async Task<IActionResult> ScormContent(string filePath)
        {
            IActionResult result;
            var sw = Stopwatch.StartNew();
            long bytesServed = 0;
            string fileName = string.Empty;

            try
            {
                var directory = filePath.Substring(0, filePath.LastIndexOf("/"));
                fileName = filePath.Substring(filePath.LastIndexOf("/") + 1, filePath.Length - filePath.LastIndexOf("/") - 1);

                var file = await this.fileService.DownloadFileAsync(directory, fileName);

                if (!new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                result = this.File(file.Content, contentType);
                bytesServed = file.ContentLength;
            }
            catch (Azure.RequestFailedException rfe) when (rfe.Status == (int)HttpStatusCode.NotFound)
            {
                result = this.NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                this.ViewBag.FilePath = filePath;
                this.ViewBag.ErrorMessage = "Unauthorised";
                this.ViewBag.ErrorDetail = ex.Message;
                result = this.View();
            }
            catch (Exception ex)
            {
                this.ViewBag.FilePath = filePath;
                this.ViewBag.ErrorMessage = "Unable to find content.";
                this.ViewBag.ErrorDetail = ex.Message;
                result = this.View();
            }

            ScormContentEventSource.Instance.AddRequestProcessMetadata(sw.ElapsedMilliseconds, bytesServed, Path.GetExtension(fileName));
            return result;
        }

        /// <summary>
        /// The TraceWindow.
        /// </summary>
        /// <returns>bool.</returns>
        public IActionResult TraceWindow()
        {
            return this.View();
        }
    }
}