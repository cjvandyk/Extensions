/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using static Extensions.Constants;
using static Extensions.Identity.App;

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
        public static Dictionary<string, Auth> AuthStack { get; set; }
            = new Dictionary<string, Auth>();

        /// <summary>
        /// The target tenant configuration set.
        /// </summary>
        public static TenantConfig TargetTenantConfig { get; set; } = null;

        /// <summary>
        /// The current active Auth object.
        /// </summary>
        public static Auth ActiveAuth { get; set; } = null;

        /// <summary>
        /// Constructor method.
        /// </summary>
        static AuthMan()
        {
            //Intentionally left empty.
        }

        ///// <summary>
        ///// Method to get the AuthenticationResult of current ActiveAuth.
        ///// </summary>
        ///// <param name="scopes">The scopes to use for auth.</param>
        ///// <returns>The active AuthenticationResult.</returns>
        //public static AuthenticationResult GetAuthenticationResult(
        //    string[] scopes = null)
        //{
        //    if ((ActiveAuth.AuthResult == null) ||
        //        ((scopes != null) &&
        //        (ActiveAuth.Scopes != scopes)))
        //    {
        //        GetAuth(scopes);
        //    }
        //    return ActiveAuth.AuthResult;
        //}

        /// <summary>
        /// Method to get a matching Auth object from the stack or if it
        /// doesn't exist on the stack, generate the new Auth object and
        /// push it to the stack.
        /// </summary>
        /// <param name="scopes">An array of scope strings.</param>
        /// <param name="authStackReset">Boolean to trigger a clearing of the 
        /// Auth stack.  Default value is false.</param>
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetAuth(
            string[] scopes,
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
        public static Auth GetAuth(
            ScopeType scopeType = ScopeType.Graph, 
            bool authStackReset = false)
        {
            if (ActiveAuth == null)
            {
                TenantConfig tenantConfig = new TenantConfig();
                tenantConfig.LoadConfig();
                AuthMan.GetAuth(tenantConfig.TenantDirectoryId,
                                tenantConfig.ApplicationClientId,
                                tenantConfig.CertThumbprint,
                                tenantConfig.TenantString,
                                scopeType,
                                authStackReset);
            }
            return GetAuth(
                ActiveAuth.TenantCfg.TenantDirectoryId,
                ActiveAuth.TenantCfg.ApplicationClientId,
                ActiveAuth.TenantCfg.CertThumbprint,
                ActiveAuth.TenantCfg.TenantString,
                scopeType,
                authStackReset);
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
        public static Auth GetAuth(
            string tenantId, 
            string appId,
            string thumbPrint,
            string tenantString,
            ScopeType scopeType = ScopeType.Graph,
            bool authStackReset = false)
        {
            //If a reset is requested, clear the stack.
            if (authStackReset)
            {
                AuthStack.Clear();
            }
            //Generate the key from the parms.
            string key = GetKey(tenantId, appId, thumbPrint, scopeType.ToString());
            //Check if the key is on the stack.
            if (AuthStack.TryGetValue(key, out Auth value))
            {
                //If it is, set the current ActiveAuth to that stack instance.
                ActiveAuth = value;
            }
            else
            {
                //If it is not, generate a new Auth object.
                var auth = new Auth(
                    tenantId, 
                    appId, 
                    thumbPrint, 
                    tenantString,
                    ClientApplicationType.Confidential,
                    scopeType);
                //Set the current ActiveAuth to the new stack instance
                //that was pushed to the stack by its ctor.
                ActiveAuth = AuthStack[key];
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
        public static Auth GetAuth(
            string tenantId,
            string appId,
            X509Certificate2 cert,
            string tenantString,
            ScopeType scopeType = ScopeType.Graph,
            bool authStackReset = false)
        {
            return GetAuth(
                tenantId,
                appId,
                cert.Thumbprint,
                tenantString,
                scopeType,
                authStackReset);
        }

        //#region GetAuthorityDomain
        ///// <summary>
        ///// A public method to get the domain extension.
        ///// </summary>
        ///// <param name="azureEnvironment">The name of the Azure environment 
        ///// type.</param>
        ///// <returns>".us" if the environment is USGovGCCHigh otherwise it
        ///// will return ".com".</returns>
        //public static string GetAuthorityDomain(
        //    AzureEnvironmentName azureEnvironment = AzureEnvironmentName.O365USGovGCCHigh)
        //{
        //    if (azureEnvironment == AzureEnvironmentName.O365USGovGCCHigh)
        //    {
        //        return ".us";
        //    }
        //    return ".com";
        //}
        //#endregion GetAuthorityDomain

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
            if (AuthStack.TryGetValue(key, out Auth value))
            {
                //If it is, set the current ActiveAuth to that stack instance.
                ActiveAuth = value;
            }
            else
            {
                //If it is not, generate a new Auth object.
                var auth = new Auth(tenantId, appId, tenantString);
                //Push it to the stack.
                AuthStack.Add(auth.Id, auth);
                //Set the current ActiveAuth to the new stack instance.
                ActiveAuth = AuthStack[key];
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
        internal static string GetKey(
            string tenantId, 
            string appId, 
            string thumbPrint,
            string scopeType)
        {
            return NoNull(tenantId.ToLower()) + "==" + 
                   NoNull(appId.ToLower()) + "==" + 
                   NoNull(thumbPrint.ToLower()) + "==" +
                   NoNull(scopeType.ToLower());
        }

        /// <summary>
        /// Method to ensure a given object is not null.
        /// </summary>
        /// <param name="str">The object to check.</param>
        /// <returns>The string value of the object.</returns>
        internal static string NoNull(object str)
        {
            if (str == null)
            {
                return "";
            }
            return str.ToString();
        }

        ///// <summary>
        ///// Method to convert a string type to ClientApplicationType.
        ///// </summary>
        ///// <param name="clientApplicationType">The string to convert.</param>
        ///// <returns>A ClientApplicationType.  
        ///// Default is Confidential.</returns>
        //public static ClientApplicationType GetClientApplicationType(
        //    string clientApplicationType)
        //{
        //    switch (clientApplicationType.ToLower())
        //    {
        //        case "public":
        //            return ClientApplicationType.Public;
        //            break;
        //    }
        //    return ClientApplicationType.Confidential;
        //}

        ///// <summary>
        ///// Get the currently active TenantString.
        ///// </summary>
        ///// <returns>A string if the stack is not empty, otherwise it will
        ///// return null.</returns>
        //public static string GetTenantString()
        //{
        //    if (ActiveAuth != null)
        //    {
        //        return ActiveAuth.TenantCfg.TenantString;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            string env = GetEnv("TenantString");
        //            if (env != null)
        //            {
        //                return env;
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    //Load config from file.
        //                    using (StreamReader sr = new StreamReader(
        //                        GetRunFolder() + "\\" + $"TenantConfig.json"))
        //                    {
        //                        var tenantConfig = 
        //                            JsonSerializer.Deserialize<TenantConfig>(sr.ReadToEnd());
        //                        sr.Close();
        //                        return tenantConfig.TenantString;
        //                    }
        //                }
        //                catch (Exception ex2)
        //                {
        //                    //Err(ex2.ToString());
        //                    throw;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //Err(ex.ToString());
        //            throw;
        //        }
        //    }
        //}

        ///// <summary>
        ///// A method to return the current execution folder.
        ///// </summary>
        ///// <returns>A string containing the execution folder path.</returns>
        //public static string GetRunFolder()
        //{
        //    return Path.GetDirectoryName(
        //        System.Reflection.Assembly.GetEntryAssembly()
        //        .Location.TrimEnd('\\'));  //Ensure no double trailing slash.
        //}

        /// <summary>
        /// A method to return a valid HttpClient for the given 
        /// AuthenticationResult.
        /// </summary>
        /// <param name="authResult">The AuthenticationResult to use during
        /// HttpClient construction.</param>
        /// <returns>A valid HttpClient using the given 
        /// AuthenticationResult.  If authResult is null, it will return
        /// the HttpClient from the ActiveAuth.</returns>
        public static HttpClient GetHttpClient(
            AuthenticationResult authResult = null)
        {
            if (authResult == null)
            {
                if (ActiveAuth == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    AuthMan.GetAuth(tenantConfig.TenantDirectoryId,
                                    tenantConfig.ApplicationClientId,
                                    tenantConfig.CertThumbprint,
                                    tenantConfig.TenantString);
                }
                return ActiveAuth.HttpClient;
            }
#if NET5_0_OR_GREATER
            var socketsHandler = new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 1000,
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(55),
                PooledConnectionLifetime = TimeSpan.FromMinutes(60)
            };
            //Build the HttpClient object using the AuthenticationResult
            //from the previous step.
            HttpClient httpClient = new HttpClient(socketsHandler);
#endif
#if NETFRAMEWORK || NETSTANDARD
            HttpClient httpClient = new HttpClient();
#endif
            httpClient.Timeout = TimeSpan.FromMinutes(1) + 
                                 TimeSpan.FromSeconds(40);
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ThreadPool.SetMaxThreads(32767, 1000);
            ProductInfoHeaderValue productInfoHeaderValue =
                new ProductInfoHeaderValue("User-Agent", $"NONISV|Extensions");
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                productInfoHeaderValue);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        /// <summary>
        /// A method to return a valid GraphServiceClient for the given
        /// HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient to use during construction
        /// of the GraphServiceClient.</param>
        /// <returns>A valid GraphServiceClient for the given 
        /// HttpClient.</returns>
        public static GraphServiceClient GetGraphServiceClient(
            HttpClient httpClient = null)
        {
            if (TargetTenantConfig == null)
            {
                TenantConfig tenantConfig = new TenantConfig();
                tenantConfig.LoadConfig();
                TargetTenantConfig = tenantConfig;
            }
            if (httpClient != null)
            {
                return new GraphServiceClient(
                    httpClient,
                    null,
                    $"https://graph.microsoft{TargetTenantConfig.AuthorityDomain}/v1.0");
            }
            return ActiveAuth.GraphClient;
        }

        /// <summary>
        /// A method to return a valid Beta endpoint GraphServiceClient for 
        /// the given HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient to use during construction
        /// of the GraphServiceClient.</param>
        /// <returns>A valid Beta endpoint GraphServiceClient for the given 
        /// HttpClient.</returns>
        internal static GraphServiceClient GetGraphBetaServiceClient(
            HttpClient httpClient = null)
        {
            if (TargetTenantConfig == null)
            {
                TenantConfig tenantConfig = new TenantConfig();
                tenantConfig.LoadConfig();
                TargetTenantConfig = tenantConfig;
            }
            if (httpClient != null)
            {
                return new GraphServiceClient(
                    httpClient,
                    null,
                    $"https://graph.microsoft{TargetTenantConfig.AuthorityDomain}/beta");
            }
            return ActiveAuth.GraphBetaClient;
        }

        #region CSOM

        /// <summary>
        /// A method to add a given user as a Site Collection 
        /// Administrator for the Site at the given url.
        /// </summary>
        /// <param name="url">The target URL of the Site.</param>
        /// <param name="email">The email of the target user.</param>
        /// <returns>True if the user was successfully added else 
        /// false.</returns>
        public static bool AddSCA(string url,
                                  string email)
        {
            return SetSCA(url, email);
        }

        /// <summary>
        /// A method to remove a given user as a Site Collection 
        /// Administrator from the Site at the given url.
        /// </summary>
        /// <param name="url">The target URL of the Site.</param>
        /// <param name="email">The email of the target user.</param>
        /// <returns>True if the user was successfully removed else 
        /// false.</returns>
        public static bool RemoveSCA(string url,
                                     string email)
        {
            return SetSCA(url, email, false);
        }

        /// <summary>
        /// A method to add or remove a given user as a Site Collection 
        /// Administrator for the Site at the given url.
        /// </summary>
        /// <param name="url">The target URL of the Site.</param>
        /// <param name="email">The email of the target user.</param>
        /// <param name="admin">A boolean switch determining if the target
        /// user is added as Admin or removed as Admin.  If true, the target
        /// user is added as Admin.  If false, the target user is removed as
        /// Admin.  Defaults to true i.e. add as admin.</param>
        /// <returns>True if the user was successfully added or removed, else 
        /// false.</returns>
        public static bool SetSCA(string url,
                                  string email,
                                  bool admin = true)
        {
            try
            {
                //Change context to SharePoint.
                GetAuth(ScopeType.SharePoint);
                ClientContext ctx = GetClientContext(url);
                var site = ctx.Site;
                var web = site.RootWeb;
                ctx.Load(site, s => s.Usage);
                ctx.Load(web);
                ctx.ExecuteQuery();
                if (web == null)
                {
                    return false;
                }
                var user = web.EnsureUser(email);
                ctx.ExecuteQuery();
                if (user == null)
                {
                    return false;
                }
                user.IsSiteAdmin = true;
                user.Update();
                ctx.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //Change context back to Graph.
                GetAuth(ScopeType.Graph);
            }
        }

        /// <summary>
        /// A method to add or remove a given User to a given Site's given 
        /// SharePoint Group e.g. Owners, Members or Visitors.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <param name="userMembershipType">The target type of membership
        /// e.g. Owners, Members or Visitors.</param>
        /// <param name="add">Boolean parameter that determines if the user is
        /// being added or removed.  If true, the user is added.  If false, the
        /// user is removed.  Default is true i.e. add user.</param>
        /// <returns>True if the User was successfully added or removed, else 
        /// false.</returns>
        public static bool SetSiteUser(string url,
                                       string email,
                                       UserMembershipType userMembershipType,
                                       bool add = true)
        {
            try
            {
                //Change context to SharePoint.
                GetAuth(ScopeType.SharePoint);
                ClientContext ctx = GetClientContext(url);
                var site = ctx.Site;
                var web = site.RootWeb;
                //Initialize the group variable to Owners.
                var group = web.AssociatedOwnerGroup;
                if (userMembershipType == UserMembershipType.Members)
                {
                    //Swith group to Members.
                    group = web.AssociatedMemberGroup;
                }
                else if (userMembershipType == UserMembershipType.Visitors)
                {
                    //Switch group to Visitors.
                    group = web.AssociatedVisitorGroup;
                }
                ctx.Load(site, s => s.Usage);
                ctx.Load(web);
                ctx.Load(group);
                ctx.ExecuteQuery();
                if (web == null)
                {
                    return false;
                }
                //Ensure the User is valid.
                var user = web.EnsureUser(email);
                ctx.Load(user, u => u.LoginName);
                ctx.Load(user, u => u.Title);
                ctx.Load(user, u => u.Email);
                ctx.ExecuteQuery();
                if (user == null)
                {
                    return false;
                }
                if (add)
                {
                    //Construct the User creation object.
                    UserCreationInformation userCreationInformation =
                        new UserCreationInformation
                        {
                            LoginName = user.LoginName,
                            Title = user.Title,
                            Email = user.Email
                        };
                    //Add the user to the target group.
                    group.Users.Add(userCreationInformation);
                }
                else
                {
                    //Remove the user from the target group.
                    group.Users.Remove(user);
                }
                web.Update();
                ctx.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                Logit.Err(ex.ToString());
                return false;
            }
            finally
            {
                //Change context back to Graph.
                GetAuth(ScopeType.Graph);
            }
        }

        /// <summary>
        /// A method to add a given User to a given Site's given 
        /// SharePoint Group e.g. Owners, Members or Visitors.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <param name="userMembershipType">The target type of membership
        /// e.g. Owners, Members or Visitors.</param>
        /// <returns>True if the User was successfully added, else 
        /// false.</returns>
        public static bool AddSiteUser(string url,
                                       string email,
                                       UserMembershipType userMembershipType)
        {
            return SetSiteUser(url, email, userMembershipType);
        }

        /// <summary>
        /// A method to add a given User to a given Site's given 
        /// SharePoint Group e.g. Owners, Members or Visitors.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="email">The email of the target User.</param>
        /// <param name="userMembershipType">The target type of membership
        /// e.g. Owners, Members or Visitors.</param>
        /// <returns>True if the User was successfully added, else 
        /// false.</returns>
        public static bool RemoveSiteUser(string url,
                                          string email,
                                          UserMembershipType userMembershipType)
        {
            return SetSiteUser(url, email, userMembershipType, false);
        }

        /// <summary>
        /// A method to add a given list of Users to a given Site's given 
        /// SharePoint Group e.g. Owners, Members or Visitors.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="emails">The list of emails of the target Users.</param>
        /// <param name="userMembershipType">The target type of membership
        /// e.g. Owners, Members or Visitors.</param>
        /// <returns>True if the Users were all successfully added, else 
        /// false.</returns>
        public static bool AddSiteUsers(string url,
                                        List<string> emails,
                                        UserMembershipType userMembershipType)
        {
            try
            {
                //Change context to SharePoint.
                GetAuth(ScopeType.SharePoint);
                ClientContext ctx = GetClientContext(url);
                var site = ctx.Site;
                var web = site.RootWeb;
                //Initialize the group variable to Owners.
                var group = web.AssociatedOwnerGroup;
                if (userMembershipType == UserMembershipType.Members)
                {
                    //Swith group to Members.
                    group = web.AssociatedMemberGroup;
                }
                else if (userMembershipType == UserMembershipType.Visitors)
                {
                    //Switch group to Visitors.
                    group = web.AssociatedVisitorGroup;
                }
                ctx.Load(site, s => s.Usage);
                ctx.Load(web);
                ctx.Load(group);
                ctx.ExecuteQuery();
                if (web == null)
                {
                    return false;
                }
                //Multi thread this.
                List<InstanceExceptionInfo> result = emails.MultiThread(email =>
                {
                    //Ensure the User is valid.
                    var user = web.EnsureUser(email);
                    ctx.Load(user, u => u.LoginName);
                    ctx.Load(user, u => u.Title);
                    ctx.Load(user, u => u.Email);
                    ctx.ExecuteQuery();
                    if (user == null)
                    {
                        throw new Exception($"User [{email}] not found!");
                    }
                    //Construct the User creation object.
                    UserCreationInformation userCreationInformation =
                        new UserCreationInformation
                        {
                            LoginName = user.LoginName,
                            Title = user.Title,
                            Email = user.Email
                        };
                    //Add the user to the target group.
                    group.Users.Add(userCreationInformation);
                    web.Update();
                    ctx.ExecuteQuery();
                });
                if (result.Count > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logit.Err(ex.ToString());
                return false;
            }
            finally
            {
                //Change context back to Graph.
                GetAuth(ScopeType.Graph);
            }
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
            string accessToken = null,
            bool suppressErrors = false)
        {
            try
            {
                var clientContext = new ClientContext(url);
                if (accessToken == null)
                {
                    accessToken = ActiveAuth.AuthResult.AccessToken;
                }
                clientContext.ExecutingWebRequest += (sender, args) =>
                {
                    args.WebRequestExecutor.RequestHeaders["Authorization"] =
                        "Bearer " + accessToken;
                };
                ActiveAuth.CsomContext = clientContext;
                return clientContext;
            }
            catch (Exception ex)
            {
                if (!suppressErrors)
                {
                    throw;
                    //Err($"ERROR getting context for [{url}].\n\r" +
                    //    $"Message: [{ex.Message}]\n\r" +
                    //    $"Stack Trace: [{ex.StackTrace}]");
                }
                return null;
            }
        }

        /// <summary>
        /// A method to update the Title of a Site.  When this method is called
        /// directly after a Site was created the Async actions in Azure may
        /// not yet have completed.  In such cases, the change of Title will
        /// be lost unless the Async processes are allowed to complete.  The 
        /// recommended default delay is 10 seconds.
        /// </summary>
        /// <param name="url">The URL of the target Site.</param>
        /// <param name="title">The new title for the site.</param>
        /// <param name="delay">A boolean switch to control if an Azure
        /// Async delay is used.  Default is false.</param>
        /// <param name="delayMilliSeconds">The number of milliseconds to sleep
        /// the thread before proceeding.  This parameter is only effective if
        /// the "delay" parameter is true.</param>
        /// <returns>True if the Site title was successfully updated, else
        /// false.</returns>
        public static bool UpdateSiteTitle(string url, 
                                           string title,
                                           bool delay = false,
                                           int delayMilliSeconds = 10000)
        {
            try
            {
                //When this method is called directly after a Site was created
                //the Async actions in Azure may not yet have completed.  In
                //such cases, the change of Title will be lost unless the Async
                //processes are allowed to complete.  The recommended delay is
                //10 seconds.
                if (delay)
                {
                    System.Threading.Thread.Sleep(delayMilliSeconds);
                }
                //Change context to SharePoint.
                GetAuth(ScopeType.SharePoint);
                ClientContext ctx = GetClientContext(url);
                var site = ctx.Site;
                var web = site.RootWeb;
                ctx.Load(site, s => s.Usage);
                ctx.Load(web);
                ctx.ExecuteQuery();
                web.Title = title;
                web.Update();
                ctx.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //Change context back to Graph.
                GetAuth(ScopeType.Graph);
            }
        }
        #endregion CSOM

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
        internal static AuthenticationResult GetAuthResult(
            object app,
            string tenantId,
            ClientApplicationType appType = ClientApplicationType.Confidential,
            string[] scopes = null)
        {
            //Inf("AuthMan.GetAuthResult()");
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
            //Reset the auth result.
            AuthenticationResult authResult = null;
            //Configure retries.
            int retries = 0;

            //Generate the result.
            switch (appType)
            {
                case ClientApplicationType.Confidential:
                    var appC = app as IConfidentialClientApplication;
                    while (retries < 3)
                    {
                        try
                        {
                            authResult = appC.AcquireTokenForClient(scopes)
                                .WithTenantId(tenantId)
                                .ExecuteAsync().GetAwaiter().GetResult();
                            break;
                        }
                        catch (MsalServiceException msalex)
                        {
                            if (!msalex.IsRetryable)
                            {
                                throw;
                            }
                            retries++;
                        }
                    }
                    break;
                case ClientApplicationType.Public:
                    var appP = app as IPublicClientApplication;
                    var accounts = appP.GetAccountsAsync()
                        .GetAwaiter().GetResult();
                    while (retries < 3)
                    {
                        try
                        {
                            authResult = GetPublicAppAuthResult(
                                ref appP,
                                ref accounts,
                                AuthPublicAppResultType.Silent);
                            break;
                            if (authResult == null)
                            {
                                authResult = GetPublicAppAuthResult(
                                    ref appP,
                                    ref accounts,
                                    AuthPublicAppResultType.Interactive);
                                break;
                                if (authResult == null)
                                {
                                    authResult = GetPublicAppAuthResult(
                                        ref appP,
                                        ref accounts,
                                        AuthPublicAppResultType.Prompt);
                                    break;
                                }
                            }
                        }
                        catch (MsalClientException msalex)
                        {
                            if (!msalex.IsRetryable)
                            {
                                throw;
                            }
                            retries++;
                        }
                    }
                    break;
            }
            return authResult;
        }
    }
}
