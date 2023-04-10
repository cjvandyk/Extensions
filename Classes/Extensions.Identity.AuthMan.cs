#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0168, CS1587, CS1998, IDE0028, IDE0059, IDE0060

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static Extensions.Core;
using static Extensions.Identity.App;
using static Extensions.Identity.Auth;
using static System.Logit;

namespace Extensions.Identity
{
    /// <summary>
    /// Authentication Manager class for working with M365 and GCCHigh.
    /// </summary>
    [Serializable]
    public static partial class AuthMan
    {
        /// <summary>
        /// The stack of current Auth objects.
        /// </summary>
        public static Dictionary<string, Auth> AuthStack { get; private set; }
            = new Dictionary<string, Auth>();

        /// <summary>
        /// The currently active Auth object from the stack.
        /// </summary>
        public static Auth ActiveAuth { get; private set; }
            = GetAuth(GetEnv("TenantDirectoryId"),
                      GetEnv("ApplicationClientId"),
                      GetEnv("CertificateThumbprint"),
                      GetEnv("TenantString"));

        /// <summary>
        /// An empty JSON node.
        /// </summary>
        public static JsonNode jsonNode = JsonNode.Parse("{}");

        /// <summary>
        /// Constructor method.
        /// </summary>
        static AuthMan()
        {
            try
            {
                //Load config from file.
                using (StreamReader sr = new StreamReader(
                    GetRunFolder() + "\\" + $"TenantConfig.json"))
                {
                    var tenantConfig =
                        JsonSerializer.Deserialize<TenantConfig>(sr.ReadToEnd());
                    //Initialize auth with config values.
                    GetAuth(tenantConfig.TenantDirectoryId,
                            tenantConfig.ApplicationClientId,
                            tenantConfig.CertThumbprint,
                            tenantConfig.TenantString);
                    sr.Close();
                }
            }
            catch (Exception ex) 
            {
                try
                {
                    //Default to environment variables if no JSON provided.
                    GetAuth(GetEnv("TenantId"),
                            GetEnv("AppClientId"),
                            GetEnv("CertThumbPrint"),
                            GetEnv("TenantString"));
                }
                catch (Exception ex2)
                {
                    string msg = "No config values available to GetAuth()\n\r" + 
                        ex2.Message;
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// Method to get the AuthenticationResult of current ActiveAuth.
        /// </summary>
        /// <param name="scopes">The scopes to use for auth.</param>
        /// <returns>The active AuthenticationResult.</returns>
        public static AuthenticationResult GetAuthenticationResult(
            string[] scopes = null)
        {
            if ((ActiveAuth.AuthResult == null) ||
                ((scopes != null) &&
                (ActiveAuth.Scopes != scopes)))
            {
                GetAuth(scopes);
            }
            return ActiveAuth.AuthResult;
        }

        /// <summary>
        /// Method to get a matching Auth object from the stack or if it
        /// doesn't exist on the stack, generate the new Auth object and
        /// push it to the stack.
        /// </summary>
        /// <param name="scopes">An array of scope strings.</param>
        /// <param name="authStackReset">Boolean to trigger a clearing of the 
        /// Auth stack.  Default value is false.</param>
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetAuth(string[] scopes,
                                   bool authStackReset = false)
        {
            ScopeType scopeType = Scopes.GetScopeType(scopes);
            return GetAuth(scopeType, authStackReset);
        }

        /// <summary>
        /// Method to get a matching Auth object from the stack or if it
        /// doesn't exist on the stack, generate the new Auth object and
        /// push it to the stack.
        /// </summary>
        /// <param name="scopeType">The scope type of the Auth.  Default value
        /// is Graph.</param>
        /// <param name="authStackReset">Boolean to trigger a clearing of the 
        /// Auth stack.  Default value is false.</param>
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetAuth(ScopeType scopeType = ScopeType.Graph, 
                                   bool authStackReset = false)
        {
            if (authStackReset)
            {
                AuthStack.Clear();
            }
            //Generate the key from the parms.
            string key = GetKey(ActiveAuth.TenantCfg.TenantDirectoryId,
                                ActiveAuth.TenantCfg.ApplicationClientId,
                                ActiveAuth.TenantCfg.CertThumbprint,
                                scopeType.ToString());
            //Check if the key is on the stack.
            if (AuthStack.ContainsKey(key))
            {
                //If it is, set the current ActiveAuth to that stack instance.
                ActiveAuth = AuthStack[key];
            }
            else
            {
                //If it is not, generate a new Auth object.
                var auth = new Auth(ActiveAuth.TenantCfg.TenantDirectoryId,
                                    ActiveAuth.TenantCfg.ApplicationClientId,
                                    ActiveAuth.TenantCfg.CertThumbprint,
                                    ActiveAuth.TenantCfg.TenantString,
                                    Auth.ClientApplicationType.Confidential,
                                    scopeType);
                //Push it to the stack.
                AuthStack.Add(auth.Id, auth);
                ActiveAuth = AuthStack[auth.Id];
            }
            //Return the ActiveAuth object.
            return ActiveAuth;
        }

        /// <summary>
        /// Method to get a matching Auth object from the stack or if it
        /// doesn't exist on the stack, generate the new Auth object and
        /// push it to the stack.
        /// </summary>
        /// <param name="tenantId">The Tenant/Directory ID to use for the 
        /// Auth object.</param>
        /// <param name="appId">The Application/Client ID to use for the 
        /// Auth object.</param>
        /// <param name="thumbPrint">The certificate thumbprint to use for 
        /// the Auth object.</param>
        /// <param name="tenantString">The base tenant string to use for 
        /// the Auth object e.g. for "contoso.sharepoint.com" it would 
        /// be "contoso".</param>
        /// <param name="scopeType">The scope type of the Auth.</param>
        /// <param name="authStackReset">Boolean to trigger a clearing of the 
        /// Auth stack.</param>
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetAuth(string tenantId, 
                                   string appId,
                                   string thumbPrint,
                                   string tenantString,
                                   ScopeType scopeType = ScopeType.Graph,
                                   bool authStackReset = false)
        {
            if (authStackReset)
            {
                AuthStack.Clear();
            }
            //Generate the key from the parms.
            string key = GetKey(tenantId, appId, thumbPrint, scopeType.ToString());
            //Check if the key is on the stack.
            if (AuthStack.ContainsKey(key))
            {
                //If it is, set the current ActiveAuth to that stack instance.
                ActiveAuth = AuthStack[key];
            }
            else
            {
                //If it is not, generate a new Auth object.
                var auth = new Auth(tenantId, 
                                    appId, 
                                    thumbPrint, 
                                    tenantString,
                                    Auth.ClientApplicationType.Confidential,
                                    scopeType);
                //Push it to the stack.
                AuthStack.Add(auth.Id, auth);
                //Set the current ActiveAuth to the new stack instance.
                ActiveAuth = AuthStack[auth.Id];
            }
            //Return the ActiveAuth object.
            return ActiveAuth;
        }

        /// <summary>
        /// Method to get a matching Auth object from the stack or if it
        /// doesn't exist on the stack, generate the new Auth object and
        /// push it to the stack.
        /// </summary>
        /// <param name="tenantId">The Tenant/Directory ID to use for the 
        /// Auth object.</param>
        /// <param name="appId">The Application/Client ID to use for the 
        /// Auth object.</param>
        /// <param name="cert">The certificate to use for the Auth 
        /// object.</param>
        /// <param name="tenantString">The base tenant string to use for 
        /// the Auth object e.g. for "contoso.sharepoint.com" it would 
        /// be "contoso".</param>
        /// <param name="scopeType">The scope type of the Auth.</param>
        /// <param name="authStackReset">Boolean to trigger a clearing of the 
        /// Auth stack.</param>
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetAuth(string tenantId,
                                   string appId,
                                   X509Certificate2 cert,
                                   string tenantString,
                                   ScopeType scopeType = ScopeType.Graph,
                                   bool authStackReset = false)
        {
            //Generate the key from the parms.
            string key = GetKey(tenantId, appId, cert.Thumbprint, scopeType.ToString());
            //Check if the key is on the stack.
            if (AuthStack.ContainsKey(key))
            {
                //If it is, set the current ActiveAuth to that stack instance.
                ActiveAuth = AuthStack[key];
            }
            else
            {
                //If it is not, generate a new Auth object.
                var auth = new Auth(tenantId, 
                                    appId, 
                                    cert.Thumbprint, 
                                    tenantString,
                                    Auth.ClientApplicationType.Confidential,
                                    scopeType);
                //Push it to the stack.
                AuthStack.Add(auth.Id, auth);
                //Set the current ActiveAuth to the new stack instance.
                ActiveAuth = AuthStack[auth.Id];
            }
            //Return the ActiveAuth object.
            return ActiveAuth;
        }

        /// <summary>
        /// Method to get a matching Auth object from the stack or if it
        /// doesn't exist on the stack, generate the new Auth object and
        /// push it to the stack.
        /// </summary>
        /// <param name="tenantId">The Tenant/Directory ID to use for the 
        /// Auth object.</param>
        /// <param name="appId">The Application/Client ID to use for the 
        /// Auth object.</param>
        /// <param name="tenantString">The base tenant string to use for 
        /// the Auth object e.g. for "contoso.sharepoint.com" it would 
        /// be "contoso".</param>
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetPublicAuth(string tenantId,
                                         string appId,
                                         string tenantString)
        {
            //Generate the key from the parms.
            string key = GetKey(tenantId, appId, "PublicClientApplication", "");
            //Check if the key is on the stack.
            if (AuthStack.ContainsKey(key))
            {
                //If it is, set the current ActiveAuth to that stack instance.
                ActiveAuth = AuthStack[key];
            }
            else
            {
                //If it is not, generate a new Auth object.
                var auth = new Auth(tenantId, appId, tenantString);
                //Push it to the stack.
                AuthStack.Add(auth.Id, auth);
                //Set the current ActiveAuth to the new stack instance.
                ActiveAuth = AuthStack[auth.Id];
            }
            //Return the ActiveAuth object.
            return ActiveAuth;
        }

        /// <summary>
        /// Internal method to generate a consistent key for the Auth object.
        /// </summary>
        /// <param name="tenantId">The Tenant/Directory ID to use for the 
        /// Auth object.</param>
        /// <param name="appId">The Application/Client ID to use for the 
        /// Auth object.</param>
        /// <param name="thumbPrint">The certificate thumbprint to use for 
        /// the Auth object.</param>
        /// <param name="scopeType">The scope type to use.</param>
        /// <returns>A string consisting of the lower case version of the
        /// specified parameters in the format of
        /// {TenantID}=={AppId}=={ThumbPrint}"</returns>
        internal static string GetKey(string tenantId, 
                                      string appId, 
                                      string thumbPrint,
                                      string scopeType)
        {
            return tenantId.ToLower() + "==" + 
                   appId.ToLower() + "==" + 
                   thumbPrint.ToLower() + "==" +
                   scopeType.ToLower();
        }

        /// <summary>
        /// Get the currently active TenantString.
        /// </summary>
        /// <returns>A string if the stack is not empty, otherwise it will
        /// return null.</returns>
        public static string GetTenantString()
        {
            if (ActiveAuth != null)
            {
                return ActiveAuth.TenantCfg.TenantString;
            }
            else
            {
                try
                {
                    string env = GetEnv("TenantString");
                    if (env != null)
                    {
                        return env;
                    }
                    else
                    {
                        try
                        {
                            //Load config from file.
                            using (StreamReader sr = new StreamReader(
                                GetRunFolder() + "\\" + $"TenantConfig.json"))
                            {
                                var tenantConfig = 
                                    JsonSerializer.Deserialize<TenantConfig>(sr.ReadToEnd());
                                sr.Close();
                                return tenantConfig.TenantString;
                            }
                        }
                        catch (Exception ex2)
                        {
                            Err(ex2.ToString());
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Err(ex.ToString());
                    throw;
                }
            }
        }

        /// <summary>
        /// A method to return the current execution folder.
        /// </summary>
        /// <returns>A string containing the execution folder path.</returns>
        public static string GetRunFolder()
        {
            return Path.GetDirectoryName(
                System.Reflection.Assembly.GetEntryAssembly()
                .Location.TrimEnd('\\'));  //Ensure no double trailing slash.
        }

        /// <summary>
        /// A method to return a valid HttpClient for the currently active Auth.
        /// </summary>
        /// <returns>A valid HttpClient for the currently active Auth.</returns>
        public static HttpClient GetHttpClient()
        {
            return ActiveAuth.HttpClient;
        }

        /// <summary>
        /// A method to return a valid HttpClient for the currently active Auth.
        /// </summary>
        /// <param name="scopes">A string array of scopes to use for Auth.</param>
        /// <returns>A valid HttpClient for the currently active Auth.</returns>
        public static HttpClient GetHttpClient(string[] scopes)
        {
            //Build the HttpClient.
            ActiveAuth.HttpClient = new HttpClient();
            //Add the Authorization header using the AccessToken from the
            //previously retrieved AuthenticationResult.
            ActiveAuth.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "bearer", 
                    ActiveAuth.AuthResult.AccessToken);
            return ActiveAuth.HttpClient;
        }

        /// <summary>
        /// A method to return a valid GraphServiceClient for the given
        /// AuthenticationResult object.  If the authResult is null, the
        /// AuthenticationResult object from the ActiveAuth is used.
        /// </summary>
        /// <param name="authResult">The AuthenticationResult object to use.</param>
        /// <returns>A valid GraphService Client using the given
        /// AuthenticationResult.</returns>
        public static GraphServiceClient GetGraphServiceClient(
            AuthenticationResult authResult = null)
        {
            if (authResult == null)
            {
                authResult = ActiveAuth.AuthResult;
            }
            //Build the GraphServiceClient object using the AuthenticationResult
            //from the previous step.
            ActiveAuth.GraphClient = new GraphServiceClient(
                "https://graph.microsoft.us/v1.0",
                new DelegateAuthenticationProvider(
                    (C) =>
                    {
                        C.Headers.Authorization = new AuthenticationHeaderValue(
                            "bearer",
                            ActiveAuth.AuthResult.AccessToken);
                        return Task.FromResult(0);
                    }));
            //Set the GraphClient timeout.
            ActiveAuth.GraphClient.HttpProvider.OverallTimeout = 
                TimeSpan.FromHours(1);
            return ActiveAuth.GraphClient;
        }

        /// <summary>
        /// Get a CSOM ClientContent for the given URL.
        /// </summary>
        /// <param name="url">The URL for which to get the Context.</param>
        /// <param name="accessToken">The token to use for the bearer.</param>
        /// <param name="suppressErrors">Should errors be suppressed?  Default
        /// is false.</param>
        /// <returns>A valid ClientContext for the site or null if an exception
        /// occurred.</returns>
        public static ClientContext GetClientContext(
            string url,
            string accessToken,
            bool suppressErrors = false)
        {
            try
            {
                var clientContext = new ClientContext(url);
                clientContext.ExecutingWebRequest += (sender, args) =>
                {
                    args.WebRequestExecutor.RequestHeaders["Authorization"] =
                        "Bearer " + accessToken;
                };
                return clientContext;
            }
            catch (Exception ex)
            {
                if (!suppressErrors)
                {
                    E($"ERROR getting context for [{url}].\n\r" + 
                        $"Message: [{ex.Message}]\n\r" +
                        $"Stack Trace: [{ex.StackTrace}]");
                }
                return null;
            }
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
        public static AuthenticationResult GetAuthResult(
            object app,
            string tenantId,
            ClientApplicationType appType = ClientApplicationType.Confidential,
            string[] scopes = null)
        {
            //If no scopes are specified, default to the current Scopes.
            if (scopes == null)
            {
                if (ActiveAuth == null)
                {
                    scopes = Scopes.Graph;
                }
                else
                {
                    scopes = ActiveAuth.Scopes;
                }
            }
            else
            {
                ActiveAuth.Scopes = scopes;
            }
            //Generate the result.
            switch (appType)
            {
                case ClientApplicationType.Confidential:
                    var appC = app as IConfidentialClientApplication;
                    ActiveAuth.AuthResult = appC.AcquireTokenForClient(scopes)
                        .WithTenantId(tenantId)
                        .ExecuteAsync().GetAwaiter().GetResult();
                    break;
                case ClientApplicationType.Public:
                    var appP = app as IPublicClientApplication;
                    var accounts = appP.GetAccountsAsync()
                        .GetAwaiter().GetResult();
                    ActiveAuth.AuthResult = GetPublicAppAuthResult(
                        ref appP,
                        ref accounts,
                        PublicAppAuthResultType.Silent);
                    if (ActiveAuth.AuthResult == null)
                    {
                        ActiveAuth.AuthResult = GetPublicAppAuthResult(
                            ref appP,
                            ref accounts,
                            PublicAppAuthResultType.Interactive);
                        if (ActiveAuth.AuthResult == null)
                        {
                            ActiveAuth.AuthResult = GetPublicAppAuthResult(
                                ref appP,
                                ref accounts,
                                PublicAppAuthResultType.Prompt);
                        }
                    }
                    break;
            }
            return ActiveAuth.AuthResult;
        }
    }
}
#pragma warning restore CS0168, CS1587, CS1998, IDE0028, IDE0059, IDE0079
