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
using static Extensions.Core;
using static Extensions.Identity.App;

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
        public AuthenticationResult AuthResult { get; private set; }
        /// <summary>
        /// The current scopes with with the current Auth object.
        /// Defaults to Graph.
        /// </summary>
        public string[] Scopes { get; private set; }
            = Identity.Scopes.Graph;
        /// <summary>
        /// The current authenticated GraphClient of the current Auth object.
        /// </summary>
        public GraphServiceClient GraphClient { get; private set; }
        /// <summary>
        /// The current authenticated HttpClient of the current Auth object.
        /// </summary>
        public HttpClient HttpClient { get; private set; }
        /// <summary>
        /// The Tenant/Directory ID used with the current Auth object.
        /// </summary>
        public string TenantId { get; private set; }
        /// <summary>
        /// The Application/Client ID used with the current Auth object.
        /// </summary>
        public string AppId { get; private set; }
        /// <summary>
        /// The certificate thumbprint used with the current Auth object.
        /// </summary>
        public string Thumbprint { get; private set; }
        /// <summary>
        /// The base tenant string used with the current Auth object
        /// e.g. for "contoso.sharepoint.com" it would be "contoso".
        /// </summary>
        public string TenantString { get; private set; }
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
            TenantId = tenantId;
            AppId = appId;
            Thumbprint = thumbprint;
            TenantString = tenantString;
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
            TenantId = tenantId;
            AppId = appId;
            Thumbprint = Cert.Thumbprint;
            TenantString = tenantString;
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
            TenantId = tenantId;
            AppId = appId;
            TenantString = tenantString;
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
                    App = Identity.App.GetApp(AppId, Cert, TenantString);
                    break;
                case ClientApplicationType.Public:
                    App = Identity.App.GetApp(AppId, TenantString);
                    break;
            }
            //Get a valid AuthenticationResult for the app.
            AuthResult = GetAuthResult(App, TenantId, AppType);
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

        /// <summary>
        /// Method to get a valid AuthenticationResult for the current 
        /// application, tenant and scopes.
        /// </summary>
        /// <param name="app">The application previously generated.</param>
        /// <param name="tenantId">The Tenant/Directory ID of the target 
        /// tenant.</param>
        /// <param name="appType">The type of ClientApplication to use.</param>
        /// <param name="scopes">The authentication scopes to target.</param>
        /// <returns>The AuthenticationResult containing the AccessToken
        /// to use for requests.</returns>
        public AuthenticationResult GetAuthResult(
            object app,
            string tenantId,
            ClientApplicationType appType = ClientApplicationType.Confidential,
            string[] scopes = null)
        {
            //If no scopes are specified, default to the current Scopes.
            if (scopes == null)
            {
                scopes = Scopes;
            }
            else
            {
                Scopes = scopes;
            }
            //Generate the result.
            switch (appType)
            {
                case ClientApplicationType.Confidential:
                    var appC = app as IConfidentialClientApplication;
                    AuthResult = appC.AcquireTokenForClient(scopes)
                        .WithTenantId(tenantId)
                        .ExecuteAsync().GetAwaiter().GetResult();
                    break;
                case ClientApplicationType.Public:
                    var appP = app as IPublicClientApplication;
                    var accounts = appP.GetAccountsAsync()
                        .GetAwaiter().GetResult();
                    AuthResult = GetPublicAppAuthResult(
                        ref appP,
                        ref accounts,
                        PublicAppAuthResultType.Silent);
                    if (AuthResult == null)
                    {
                        AuthResult = GetPublicAppAuthResult(
                            ref appP,
                            ref accounts,
                            PublicAppAuthResultType.Interactive);
                        if (AuthResult == null)
                        {
                            AuthResult = GetPublicAppAuthResult(
                                ref appP,
                                ref accounts,
                                PublicAppAuthResultType.Prompt);
                        }
                    }
                    break;
            }
            return AuthResult;
        }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
