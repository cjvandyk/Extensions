#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using Microsoft.Identity.Client;
using System.Security.Cryptography.X509Certificates;

namespace Extensions.Identity
{
    /// <summary>
    /// A class for managing ConfidentialClientApplications in Azure.
    /// </summary>
    public static partial class App
    {
        /// <summary>
        /// An enum for the type of Azure environment.
        /// </summary>
        public enum EnvironmentName
        {
            /// <summary>
            /// China national cloud.
            /// </summary>
            O365China,
            /// <summary>
            /// Default commercial cloud.
            /// </summary>
            O365Default,  //Commercial tenant
            /// <summary>
            /// Germany national cloud.
            /// </summary>
            O365GermanyCloud,
            /// <summary>
            /// US Government Department of Defense cloud.
            /// </summary>
            O365USGovDoD,
            /// <summary>
            /// US Government High Security cloud.
            /// </summary>
            O365USGovGCCHigh  //GCCHigh tenant
        }

        /// <summary>
        /// Get a valid IConfidentialClientApplication.
        /// </summary>
        /// <param name="appId">The GUID for the App/Client ID.</param>
        /// <param name="thumbPrint">The thumbprint of the associated 
        /// certificate.</param>
        /// <param name="tenantString">The tenant string e.g. for 
        /// 'contoso.sharepoint.com' the value would be 'contoso'.</param>
        /// <param name="environmentName">The environment name enum value.
        /// Defaults to O365USGovGCCHigh.</param>
        /// <returns>An IConfidentialClientApplication object.</returns>
        public static IConfidentialClientApplication GetApp(
            string appId,
            string thumbPrint,
            string tenantString,
            EnvironmentName environmentName = EnvironmentName.O365USGovGCCHigh)
        {
            //Get the target cert.
            var cert = Cert.GetCertByThumbPrint(thumbPrint);
            //Using the cert, get the app.
            return GetApp(appId, cert, tenantString, environmentName);
        }

        /// <summary>
        /// Get a valid IConfidentialClientApplication.
        /// </summary>
        /// <param name="appId">The GUID for the App/Client ID.</param>
        /// <param name="cert">The associated certificate.</param>
        /// <param name="tenantString">The tenant string e.g. for 
        /// 'contoso.sharepoint.com' the value would be 'contoso'.</param>
        /// <param name="environmentName">The environment name enum value.
        /// Defaults to O365USGovGCCHigh.</param>
        /// <returns>An IConfidentialClientApplication object.</returns>
        public static IConfidentialClientApplication GetApp(
            string appId,
            X509Certificate2 cert,
            string tenantString,
            EnvironmentName environmentName = EnvironmentName.O365USGovGCCHigh)
        {
            //Get the authority URL for the given environment.
            string authority = GetAuthority(environmentName, tenantString);
            //Create the app using appId, cert and authority.
            return ConfidentialClientApplicationBuilder.Create(appId)
                .WithCertificate(cert)
                .WithAuthority(authority)
                .Build();
        }

        /// <summary>
        /// Get the authentication authority URL for the given environment.
        /// </summary>
        /// <param name="environmentName">The specified environment.
        /// Defaults to GCCHigh.</param>
        /// <param name="tenantString">The specified tenant string.</param>
        /// <returns>The URL of the authentication authority.</returns>
        /// <exception cref="Exception">Unimplemented clouds.</exception>
        public static string GetAuthority(EnvironmentName environmentName,
                                          string tenantString)
        {
            string result = "https://login.microsoftonline.";
            switch (environmentName)
            {
                case EnvironmentName.O365China:
                    throw new Exception("O365China is not presently supported.");
                    break;
                case EnvironmentName.O365Default:
                    result += $".com/{tenantString}.sharepoint.com";
                    break;
                case EnvironmentName.O365GermanyCloud:
                    throw new Exception("O365GermanyCloud is not presently supported.");
                    break;
                case EnvironmentName.O365USGovDoD:
                    throw new Exception("O365USGovDoD is not presently supported.");
                    break;
                case EnvironmentName.O365USGovGCCHigh:
                    result += $".us/{tenantString}.sharepoint.us";
                    break;
            }
            return result;
        }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
