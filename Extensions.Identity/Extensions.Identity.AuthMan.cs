#pragma warning disable CS0168, CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using static Extensions.Core;
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
                    GetAuth(tenantConfig.TenantId,
                            tenantConfig.AppClientId,
                            tenantConfig.CertThumbPrint,
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
                    throw new Exception("No config values available to GetAuth()");
                }
            }
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
                return ActiveAuth.TenantString;
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
                                    JsonSerializer.Deserialize<Extensions.TenantConfig>(sr.ReadToEnd());
                                sr.Close();
                                return tenantConfig.TenantString;
                            }
                        }
                        catch (Exception ex2)
                        {
                            Err(ex2.ToString());
                            throw;
                        }
                        finally
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    Err(ex.ToString());
                    throw;
                }
                finally
                {
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
    }
}
#pragma warning restore CS0168, CS1587, CS1998, IDE0059, IDE0028
