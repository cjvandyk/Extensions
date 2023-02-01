#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace System.Identity
{
    /// <summary>
    /// Authentication Manager class for working with M365 and GCCHigh.
    /// </summary>
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
        public static Auth ActiveAuth { get; private set; } = null;

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
        /// <returns>A valid Auth object from the stack.</returns>
        public static Auth GetAuth(string tenantId, 
                                   string appId,
                                   string thumbPrint,
                                   string tenantString)
        {
            //Generate the key from the parms.
            string key = GetKey(tenantId, appId, thumbPrint);
            //Check if the key is on the stack.
            if (AuthStack.ContainsKey(key))
            {
                //If it is, set the current ActiveAuth to that stack instance.
                ActiveAuth = AuthStack[key];
            }
            else
            {
                //If it is not, generate a new Auth object.
                var auth = new Auth(tenantId, appId, thumbPrint, tenantString);
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
        /// <returns>A string consisting of the lower case version of the
        /// specified parameters in the format of
        /// {TenantID}=={AppId}=={ThumbPrint}"</returns>
        internal static string GetKey(string tenantId, 
                                      string appId, 
                                      string thumbPrint)
        {
            return tenantId.ToLower() + "==" + 
                   appId.ToLower() + "==" + 
                   thumbPrint.ToLower();
        }

        /// <summary>
        /// Get the currently active TenantString.
        /// </summary>
        /// <returns>A string if the stack is not empty, otherwise it will
        /// return null.</returns>
        public static string GetTenantString()
        {
            return ActiveAuth == null ? "" : ActiveAuth.TenantString;
        }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
