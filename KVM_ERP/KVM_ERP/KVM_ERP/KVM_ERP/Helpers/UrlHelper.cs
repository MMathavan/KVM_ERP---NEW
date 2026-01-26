using System;
using System.Web;
using System.Web.Mvc;

namespace ClubMembership.Helpers
{
    public static class UrlHelper
    {
        /// <summary>
        /// Gets the base URL for the current application
        /// </summary>
        /// <param name="helper">The URL helper</param>
        /// <returns>The base URL</returns>
        public static string GetBaseUrl(this System.Web.Mvc.UrlHelper helper)
        {
            var request = HttpContext.Current.Request;
            return request.Url.GetLeftPart(UriPartial.Authority) + helper.Content("~/");
        }

        /// <summary>
        /// Converts a relative path to an absolute URL using the application's base URL
        /// </summary>
        /// <param name="helper">The URL helper</param>
        /// <param name="relativePath">The relative path (e.g., "/Uploads/image.jpg")</param>
        /// <returns>The absolute URL</returns>
        public static string ToAbsoluteUrl(this System.Web.Mvc.UrlHelper helper, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            // If it's already an absolute URL, return as is
            if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativePath;

            var baseUrl = helper.GetBaseUrl();
            
            // Remove leading slash if present to avoid double slashes
            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);
                
            // Remove trailing slash from base URL to avoid double slashes
            baseUrl = baseUrl.TrimEnd('/');
            
            return baseUrl + "/" + relativePath;
        }

        /// <summary>
        /// Converts a relative image path to an absolute URL
        /// </summary>
        /// <param name="helper">The URL helper</param>
        /// <param name="imagePath">The image path (e.g., "/Uploads/Gallery/image.jpg")</param>
        /// <returns>The absolute image URL</returns>
        public static string ImageUrl(this System.Web.Mvc.UrlHelper helper, string imagePath)
        {
            return helper.ToAbsoluteUrl(imagePath);
        }
    }
}
