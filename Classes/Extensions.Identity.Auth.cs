#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using static Extensions.Identity.AuthMan;

namespace Extensions.Identity
{
    /// <summary>
    /// Auth class for holding authentication objects.
    /// </summary>
    [Serializable]
    public partial class Auth
    {
        /// <summary>
        /// The ID of the auth object in format {TenantID}=={AppId}=={ThumbPrint}"
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The container for this Auth object's associated tenant configuration.
        /// </summary>
        public TenantConfig TenantCfg { get; private set; } 
            = new TenantConfig();

        /// <summary>
        ///The type of ClientApplication the current Auth object is using.
        /// </summary>
        public ClientApplicationType AppType { get; private set; }

        /// <summary>
        /// The certificate of the current Auth object.
        /// </summary>
        public X509Certificate2 Cert { get; private set; }

        /// <summary>
        /// The current IConfidentialClientApplication of the current Auth object.
        /// </summary>
        public object App { get; private set; }

        /// <summary>
        /// The current AuthenticationResult of the current Auth object.
        /// </summary>
        public AuthenticationResult AuthResult { get; set; }

        /// <summary>
        /// The current scopes with with the current Auth object.
        /// Defaults to Graph.
        /// </summary>
        public string[] Scopes { get; set; }
            = Identity.Scopes.Graph;

        /// <summary>
        /// The current authenticated GraphClient of the current Auth object.
        /// </summary>
        public GraphServiceClient GraphClient { get; set; }

        /// <summary>
        /// The current authenticated HttpClient of the current Auth object.
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// The authentication refresh timer of the current Auth object.
        /// </summary>
        public System.Threading.Timer Timer { get; private set; }

        /// <summary>
        /// The constructor that populates all the member variables of this
        /// instance of the Auth object.
        /// </summary>
        /// <param name="tenantId">The Tenant/Directory ID of the target.</param>
        /// <param name="appId">The Application/Client ID of the target.</param>
        /// <param name="thumbprint">The thumbprint of the certificate to 
        /// use.</param>
        /// <param name="tenantString">The base tenant string used with the 
        /// current Auth object e.g. for "contoso.sharepoint.com" it would 
        /// be "contoso".</param>
        /// <param name="appType">The type of ClientApplication to use.</param>
        /// <param name="scopeType">The scope type used by this auth.</param>
        public Auth(
            string tenantId, 
            string appId, 
            string thumbprint, 
            string tenantString,
            ClientApplicationType appType = ClientApplicationType.Confidential,
            ScopeType scopeType = ScopeType.Graph)
        {
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, thumbprint, scopeType.ToString());
            //Save the parms.
            AppType = appType;
            //Save the tenant configuration values.
            TenantCfg.TenantDirectoryId = tenantId;
            TenantCfg.ApplicationClientId = appId;
            TenantCfg.CertThumbprint = thumbprint;
            TenantCfg.TenantString = tenantString;
            //Get the certificate.
            Cert = Identity.Cert.GetCertByThumbPrint(thumbprint);
            //Get the application.
            App = Identity.App.GetApp(appId, thumbprint, tenantString);
            //Set the scopes for this Auth object.
            Scopes = Identity.Scopes.GetScopes(scopeType);
            //Call refresh method to populate the rest.
            RefreshAuth(null);            
        }

        /// <summary>
        /// The constructor that populates all the member variables of this
        /// instance of the Auth object.
        /// </summary>
        /// <param name="tenantId">The Tenant/Directory ID of the target.</param>
        /// <param name="appId">The Application/Client ID of the target.</param>
        /// <param name="cert">The certificate to use.</param>
        /// <param name="tenantString">The base tenant string used with the 
        /// current Auth object e.g. for "contoso.sharepoint.com" it would 
        /// be "contoso".</param>
        /// <param name="appType">The type of ClientApplication to use.</param>
        /// <param name="scopeType">The scope type to use.</param>
        public Auth(
            string tenantId,
            string appId,
            X509Certificate2 cert,
            string tenantString,
            ClientApplicationType appType = ClientApplicationType.Confidential,
            ScopeType scopeType = ScopeType.Graph)
        {
            Cert = cert;
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, Cert.Thumbprint, scopeType.ToString());
            //Save the parms.
            AppType = appType;
            TenantCfg.TenantDirectoryId = tenantId;
            TenantCfg.ApplicationClientId = appId;
            TenantCfg.CertThumbprint = Cert.Thumbprint;
            TenantCfg.TenantString = tenantString;
            //Get the application.
            App = Identity.App.GetApp(appId, Cert.Thumbprint, tenantString);
            //Set the scopes for this Auth object.
            Scopes = Identity.Scopes.GetScopes(scopeType);
            //Call refresh method to populate the rest.
            RefreshAuth(null);
        }

        /// <summary>
        /// The constructor that populates all the member variables of this
        /// instance of the Auth object.
        /// </summary>
        /// <param name="tenantId">The Tenant/Directory ID of the target.</param>
        /// <param name="appId">The Application/Client ID of the target.</param>
        /// <param name="tenantString">The base tenant string used with the 
        /// current Auth object e.g. for "contoso.sharepoint.com" it would 
        /// be "contoso".</param>
        /// <param name="appType">The type of ClientApplication to use.</param>
        public Auth(
            string tenantId,
            string appId,
            string tenantString,
            ClientApplicationType appType = ClientApplicationType.Public)
        {
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, "PublicClientApplication", "");
            //Save the parms.
            AppType = appType;
            TenantCfg.TenantDirectoryId = tenantId;
            TenantCfg.ApplicationClientId = appId;
            TenantCfg.TenantString = tenantString;
            //Get the application.
            App = Identity.App.GetApp(appId, tenantString);
            //Call refresh method to populate the rest.
            RefreshAuth(null);
        }

        /// <summary>
        /// The method that refreshes the current Auth object's related
        /// authentication members.
        /// </summary>
        /// <param name="obj">Needed for the timer method.</param>
        public void RefreshAuth(Object obj)
        {
            //Build the application.
            switch (AppType)
            {
                case ClientApplicationType.Confidential:
                    App = Identity.App.GetApp(TenantCfg.ApplicationClientId,
                                              Cert,
                                              TenantCfg.TenantString);
                    break;
                case ClientApplicationType.Public:
                    App = Identity.App.GetApp(TenantCfg.ApplicationClientId,
                                              TenantCfg.TenantString);
                    break;
            }
            //Get a valid AuthenticationResult for the app.
            AuthResult = GetAuthResult(
                App, 
                TenantCfg.TenantDirectoryId, 
                AppType);
            //Build the GraphServiceClient object using the AuthenticatedResult
            //from the previous step.
            GraphClient = new GraphServiceClient(
                "https://graph.microsoft.us/v1.0",
                new DelegateAuthenticationProvider(
                    (C) =>
                    {
                        C.Headers.Authorization =
                            new AuthenticationHeaderValue(
                                "bearer",
                                AuthResult.AccessToken);
                        return Task.FromResult(0);
                    }));
            //Set the GraphClient timeout.
            GraphClient.HttpProvider.OverallTimeout = TimeSpan.FromHours(1);
            //Build the HttpClient.
            HttpClient = new HttpClient();
            //Add the Authorization header using the AccessToken from the
            //previously retrieved AuthenticationResult.
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AuthResult.AccessToken);
            //Set the client to accept JSON.
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            //Define the refresh timer that will fire 5 minutes before the
            //expiration of the AuthenticationResult.
            Timer = new System.Threading.Timer(
                RefreshAuth,
                null,
                (AuthResult.ExpiresOn - DateTime.UtcNow).Subtract(
                    TimeSpan.FromMinutes(5)),
                Timeout.InfiniteTimeSpan);
        }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
