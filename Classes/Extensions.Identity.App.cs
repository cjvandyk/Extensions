#pragma warning disable CS0162, CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Extensions.Identity
{
    /// <summary>
    /// A class for managing ConfidentialClientApplications in Azure.
    /// </summary>
    [Serializable]
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
                .WithAuthority(new Uri(authority))
                .Build();
        }

        /// <summary>
        /// Get a valid IPublicClientApplication.
        /// </summary>
        /// <param name="appId">The GUID for the App/Client ID.</param>
        /// <param name="tenantString">The tenant string e.g. for 
        /// 'contoso.sharepoint.com' the value would be 'contoso'.</param>
        /// <param name="environmentName">The environment name enum value.
        /// Defaults to O365USGovGCCHigh.</param>
        /// <returns>An IPublicClientApplication object.</returns>
        public static IPublicClientApplication GetApp(
            string appId,
            string tenantString,
            EnvironmentName environmentName = EnvironmentName.O365USGovGCCHigh)
        {
            //Get the authority URL for the given environment.
            string authority = GetAuthority(environmentName, tenantString);
            //Create the app using appId, cert and authority.
            var app = PublicClientApplicationBuilder.Create(appId)
                .WithAuthority(new Uri(authority))
                .WithDefaultRedirectUri()
                .Build();
            var accounts = app.GetAccountsAsync().GetAwaiter().GetResult();
            if (GetPublicAppAuthResult(
                ref app,
                ref accounts,
                PublicAppAuthResultType.Silent) == null)
            {
                if (GetPublicAppAuthResult(
                    ref app,
                    ref accounts,
                    PublicAppAuthResultType.Interactive) == null)
                {
                    if (GetPublicAppAuthResult(
                        ref app,
                        ref accounts,
                        PublicAppAuthResultType.Prompt) == null)
                    {
                        return null;
                    }
                }
            }
            return app;
        }

        /// <summary>
        /// The different types of authentication efforts to use.
        /// </summary>
        internal enum PublicAppAuthResultType
        {
            Silent,
            Interactive,
            Prompt
        }

        /// <summary>
        /// Get an IPublicClientApplication AuthenticationResult based on
        /// different authentication methods from least to most intrusive.
        /// </summary>
        /// <param name="app">A reference to the IPublicClientApplication
        /// that is being authenticated.</param>
        /// <param name="accounts">A reference to the accounts associated
        /// with the given IPublicClientApplication.</param>
        /// <param name="authType">The type of authentication to use.  Values
        /// range from least to most intrusive to the end user.</param>
        /// <returns>A valid AuthenticationResult or null.</returns>
        internal static AuthenticationResult GetPublicAppAuthResult(
            ref IPublicClientApplication app,
            ref System.Collections.Generic.IEnumerable<IAccount> accounts,
            PublicAppAuthResultType authType)
        {
            switch (authType)
            {
                case PublicAppAuthResultType.Silent:
                    return app.AcquireTokenSilent(
                        Scopes.Graph,
                        accounts.FirstOrDefault())
                        .ExecuteAsync().GetAwaiter().GetResult();
                    break;
                case PublicAppAuthResultType.Interactive:
                    return app.AcquireTokenInteractive(Scopes.Graph)
                        .ExecuteAsync().GetAwaiter().GetResult();
                    break;
                case PublicAppAuthResultType.Prompt:
                    return app.AcquireTokenInteractive(Scopes.Graph)
                        .WithAccount(accounts.FirstOrDefault())
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync().GetAwaiter().GetResult();
                    break;
            }
            return null;
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
            string result = "https://login.microsoftonline";
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
#pragma warning restore CS0162, CS1587, CS1998, IDE0059, IDE0028
