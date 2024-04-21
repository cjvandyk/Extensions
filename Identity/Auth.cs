/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using static Extensions.Identity.AuthMan;
using static Extensions.Constants;

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
        /// The container to maintain the current Auth object scope type.
        /// </summary>
        public ScopeType AuthScopeType { get; set; }

        /// <summary>
        /// The container for this Auth object's associated tenant configuration.
        /// </summary>
        public TenantConfig TenantCfg { get; private set; } = new TenantConfig();

        /// <summary>
        ///The type of ClientApplication the current Auth object is using.
        /// </summary>
        public ClientApplicationType AppType { get; private set; }

        /// <summary>
        /// The certificate of the current Auth object.
        /// </summary>
        public X509Certificate2 Cert { get; private set; } = null;

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
        /// The current authenticated GraphClient of the current Auth object
        /// for the Beta endpoint.
        /// </summary>
        public GraphServiceClient GraphBetaClient { get; set; }

        /// <summary>
        /// The current authenticated HttpClient of the current Auth object.
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// The current authenticated CSOM ClientContext of the current Auth object.
        /// </summary>
        public Microsoft.SharePoint.Client.ClientContext CsomContext { get; set; }

        /// <summary>
        /// The active stack of Site objects.
        /// </summary>
        public Dictionary<string, Microsoft.Graph.Models.Site> ActiveSiteStack { get; set; }
            = new Dictionary<string, Microsoft.Graph.Models.Site>();

        /// <summary>
        /// The active stack of List objects.
        /// </summary>
        public Dictionary<string, Microsoft.Graph.Models.List> ActiveListStack { get; set; }
            = new Dictionary<string, Microsoft.Graph.Models.List>();

        /// <summary>
        /// The authentication refresh timer of the current Auth object.
        /// </summary>
        public System.Threading.Timer Timer { get; private set; }

        /// <summary>
        /// Empty constructor for Auth.
        /// </summary>
        public Auth() 
        {
        }

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
            if (TenantCfg.TenantString != tenantString)
            {
                TenantCfg = new TenantConfig(tenantString);
            }
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, thumbprint, scopeType.ToString());
            AuthScopeType = scopeType;
            //Save the parms.
            AppType = appType;
            //Save the tenant configuration values.
            if (TenantCfg.TenantDirectoryId != tenantId)
            {
                TenantCfg.TenantDirectoryId = tenantId;
            }
            if (TenantCfg.ApplicationClientId != appId)
            {
                TenantCfg.ApplicationClientId = appId;
            }
            if (TenantCfg.CertThumbprint != thumbprint)
            {
                TenantCfg.CertThumbprint = thumbprint;
            }
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
            if (TenantCfg.TenantString != tenantString)
            {
                TenantCfg = new TenantConfig(tenantString);
            }
            Cert = cert;
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, Cert.Thumbprint, scopeType.ToString());
            AuthScopeType = scopeType;
            //Save the parms.
            AppType = appType;
            if (TenantCfg.TenantDirectoryId != tenantId)
            {
                TenantCfg.TenantDirectoryId = tenantId;
            }
            if (TenantCfg.ApplicationClientId != appId)
            {
                TenantCfg.ApplicationClientId = appId;
            }
            if (TenantCfg.CertThumbprint != cert.Thumbprint)
            {
                TenantCfg.CertThumbprint = cert.Thumbprint;
            }
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
            if (TenantCfg.TenantString != tenantString)
            {
                TenantCfg = new TenantConfig(tenantString);
            }
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, "PublicClientApplication", "");
            //Save the parms.
            AppType = appType;
            if (TenantCfg.TenantDirectoryId != tenantId)
            {
                TenantCfg.TenantDirectoryId = tenantId;
            }
            if (TenantCfg.ApplicationClientId != appId)
            {
                TenantCfg.ApplicationClientId = appId;
            }
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
                AppType,
                Scopes);
            //Build the GraphServiceClient object using the AuthenticatedResult
            //from the previous step.
            HttpClient = GetHttpClient(AuthResult);
            GraphClient = GetGraphServiceClient(HttpClient);
            GraphBetaClient = GetGraphBetaServiceClient(HttpClient);
            //Check if the current Auth object is in the stack.
            if (AuthStack.ContainsKey(Id))
            {
                //It is, so update it.
                lock (AuthStack)
                {
                    AuthStack[Id] = this;
                }
            }
            else
            {
                //It is not, so push it to the stack.
                lock(AuthStack)
                {
                    AuthStack.Add(Id, this);
                }
            }
            ActiveAuth = AuthStack[Id];
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
