/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using static Extensions.Core;

namespace Extensions
{
    /// <summary>
    /// A class container for custom SharePoint methods.
    /// </summary>
    [Serializable]
    public static partial class SharePoint
    {
        /// <summary>
        /// A method that validates if a given user is a valid SharePoint user.
        /// By default, the tenant root site is used for the validation, but
        /// this can be overriden by providing the relative site URL to the
        /// site you wish to use instead, in the relativeSiteUrl parameter.
        /// </summary>
        /// <param name="email">The email of the user to validate.</param>
        /// <param name="relativeSiteUrl">An optional string value that can be
        /// provided to override the default which will use the tenant root
        /// site for the validation.  Format is "/sites/Research".</param>
        /// <returns>True if the user is valid, else false.  NOTE: This
        /// method calls SharePoint's /_api/web/ensureuser REST method to do
        /// the validation.</returns>
        public static bool ValidUser(
            string email, 
            string relativeSiteUrl = null)
        {
            try
            {
                var auth = GetAuth(Identity.ScopeType.SharePoint);
                string url = ActiveAuth.TenantCfg.TenantUri.ToString()
                    .TrimEnd('/') +
                    "/sites/" +
                    relativeSiteUrl == null ?
                        ActiveAuth.TenantCfg.TenantUri.ToString().TrimEnd('/') :
                        relativeSiteUrl +
                    "/_api/web/ensureusers";
                string payload = "{'logonName': '" + email + "'}";
                HttpContent httpContent = new StringContent(payload);
                httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    "application/json;odata=verbose;charset=utf-8");
                httpContent.Headers.ContentType.Parameters.Add(
                    new NameValueHeaderValue("odata", "verbose"));
                var response = auth.HttpClient.PostAsync(url, httpContent)
                    .GetAwaiter().GetResult();
                var responseBody = response.Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();
                if (responseBody.Contains("\"odata.type\":\"SP.User\""))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
            }
            return false;
        }
    }
}
