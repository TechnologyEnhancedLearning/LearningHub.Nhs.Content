//using LearningHub.Nhs.Models.Entities.Migration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LearningHub.Nhs.Content.Helpers
//{
//    public class ReWriteHelper
//    {
//        /// <summary>
//        /// Defines the contentSegment.
//        /// </summary>
//        static string ContentSegment = $"/content/";

//        static string AdapterSegment = $"/jsadapter12_aspnet/jsadapter12_asp/oracle_scorm_adapter_js.html?starting_url=";

//        public static String ReWriteLearninHubRequest(MigrationSourceViewModel migrationSource, string requestPath)
//        {
//            const int resourceSegmentPosition = 3;
//            var pathSegments = requestPath.Split('/');

//            if (pathSegments.Length < resourceSegmentPosition)
//            {
//                return;
//            }

//            // Index 2 resourcename
//            var resourceSegment = pathSegments[2];

//            string hostSegment = requestUrl.Substring(0, requestUrl.IndexOf(this.contentSegment));
//            var resourceUrl = $"{hostSegment}{this.contentSegment}{resourceSegment}";
//            ScormContentServerViewModel scormResourceDetail = this.scormContentRewriteService.GetScormResourceDetailAsync(resourceUrl).Result;

//            if (scormResourceDetail == null)
//            {
//                return null;
//            }
//            if (pathSegments.Length > resourceSegmentPosition)
//            {
//                return requestPath.Replace(resourceSegment, scormResourceDetail.ContentFilePath); ;
//            }
//            else if (pathSegments.Length == resourceSegmentPosition)
//            {
//                return $"{hostSegment}{AdapterSegment}{resourceUrl}/{scormResourceDetail.ManifestUrl}";
//            }
//        }
//    }
//}
