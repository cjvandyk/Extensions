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
        /// The certificate of the current Auth object.
        /// </summary>
        public X509Certificate2 Cert { get; private set; }
        /// <summary>
        /// The current ConfidentialClientApplication of the current Auth object.
        /// </summary>
        public IConfidentialClientApplication App { get; private set; }
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
        public string ThumbPrint { get; private set; }
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
        /// <param name="thumbPrint">The thumbprint of the certificate to 
        /// use.</param>
        /// <param name="tenantString">The base tenant string used with the 
        /// current Auth object e.g. for "contoso.sharepoint.com" it would 
        /// be "contoso".</param>
        public Auth(string tenantId, 
                    string appId, 
                    string thumbPrint, 
                    string tenantString)
        {
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, thumbPrint);
            //Save the parms.
            TenantId = tenantId;
            AppId = appId;
            ThumbPrint = thumbPrint;
            TenantString = tenantString;
            //Get the certificate.
            Cert = Identity.Cert.GetCertByThumbPrint(thumbPrint);
            //Get the application.
            App = Identity.App.GetApp(appId, thumbPrint, tenantString);
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
        public Auth(string tenantId,
                    string appId,
                    X509Certificate2 cert,
                    string tenantString)
        {
            Cert = cert;
            //Generate the unique ID.
            Id = AuthMan.GetKey(tenantId, appId, Cert.Thumbprint);
            //Save the parms.
            TenantId = tenantId;
            AppId = appId;
            ThumbPrint = Cert.Thumbprint;
            TenantString = tenantString;
            //Get the application.
            App = Identity.App.GetApp(appId, Cert.Thumbprint, tenantString);
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
            App = Identity.App.GetApp(AppId, Cert, TenantString);
            //Get a valid AuthenticationResult for the app.
            AuthResult = GetAuthResult(App, TenantId);
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
        /// <param name="scopes">The authentication scopes to target.</param>
        /// <returns>The AuthenticationResult containing the AccessToken
        /// to use for requests.</returns>
        public AuthenticationResult GetAuthResult(IConfidentialClientApplication app,
                                                  string tenantId,
                                                  string[] scopes = null)
        {
            //If no scopes are specified, default to the current Scopes.
            if (scopes is null)
            {
                scopes = Scopes;
            }
            else
            {
                Scopes = scopes;
            }
            //Generate the result.
            AuthResult = app.AcquireTokenForClient(scopes)
                .WithTenantId(tenantId)
                .ExecuteAsync().GetAwaiter().GetResult();
            return AuthResult;
        }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
