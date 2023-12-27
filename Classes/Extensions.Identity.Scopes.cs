#pragma warning disable CS0162, CS1587, CS1998, IDE0028, IDE0059

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using static Extensions.Core;
using static System.Logit;

namespace Extensions.Identity
{
    /// <summary>
    /// The types of scopes Identity could use.
    /// </summary>
    [Serializable]
    public enum ScopeType
    {
        /// <summary>
        /// Exchange scope.
        /// </summary>
        Exchange,
        /// <summary>
        /// Graph scope.
        /// </summary>
        Graph,
        /// <summary>
        /// Management scope.
        /// </summary>
        Management,
        /// <summary>
        /// PowerBI scope.
        /// </summary>
        PowerBI,
        /// <summary>
        /// SharePoint scope.
        /// </summary>
        SharePoint,
        /// <summary>
        /// SharePoint Admin scope.
        /// </summary>
        SharePointAdmin
    }

    /// <summary>
    /// A class containing the various scope string arrays.
    /// </summary>
    [Serializable]
    public static partial class Scopes
    {
        /// <summary>
        /// Universal definition for the offline access scope string.
        /// </summary>
        internal static string Offline = "offline_access";

        /// <summary>
        /// Default access scope set to Graph.
        /// </summary>
        public static string[] Default = Graph;

        /// <summary>
        /// Access scope for Exchange.
        /// </summary>
        public static string[] Exchange = new string[]
        {
            "https://outlook.office365.us/.default"
        };

        /// <summary>
        /// Access scope for Graph.
        /// </summary>
        public static string[] Graph = new string[]
        {
            "https://graph.microsoft.us/.default",
            Offline
        };

        /// <summary>
        /// Access scope for M365 Management.
        /// </summary>
        public static string[] Management = new string[]
        {
            "https://manage.office365.us/.default",
            Offline
        };

        /// <summary>
        /// Access scope for PowerBI.
        /// </summary>
        public static string[] PowerBI = new string[]
        {
            "https://high.analysis.usgovcloudapi.net/powerbi/api/.default",
            Offline
        };
        //"https://analysis.windows.net/powerbi/api/.default"

        /// <summary>
        /// Access scope for SharePoint.
        /// </summary>
        public static string[] SharePoint = new string[]
        {
            $"https://{AuthMan.GetTenantString().TrimEnd('/')}.sharepoint.us/.default",
            Offline
        };

        /// <summary>
        /// Access scope for SharePoint Admin portal.
        /// </summary>
        public static string[] SharePointAdmin = new string[]
        {
            $"https://{CoreBase.TenantString}-admin.sharepoint.us/.default",
            Offline
        };

        /// <summary>
        /// Return the scopes array based on the type of scope being
        /// requested.
        /// </summary>
        /// <param name="scopeType">The type of scope.</param>
        /// <returns>A string array of scopes.</returns>
        public static string[] GetScopes(ScopeType scopeType)
        {
            switch (scopeType)
            {
                case ScopeType.Graph:
                    return Scopes.Graph;
                    break;
                //For performance reasons SharePoint is listed second as its
                //the second most common scope type in use after Graph.  This
                //eliminates needless checks on common queries.
                case ScopeType.SharePoint:
                    return Scopes.SharePoint;
                    break;
                case ScopeType.Management:
                    return Scopes.Management;
                    break;
                case ScopeType.PowerBI:
                    return Scopes.PowerBI;
                    break;
                case ScopeType.Exchange:
                    return Scopes.Exchange;
                    break;
                case ScopeType.SharePointAdmin:
                    return Scopes.SharePointAdmin;
                    break;
            }
            return AuthMan.ActiveAuth.Scopes;
        }

        /// <summary>
        /// Method to convert a string array of scopes back to a ScopeType object.
        /// </summary>
        /// <param name="scopes">The string array of scopes.</param>
        /// <returns>The associated ScopeType object.</returns>
        /// <exception cref="Exception">Thrown if conversion fails.</exception>
        public static ScopeType GetScopeType(string[] scopes)
        {
            switch (scopes[0].ToLower())
            {
                case "https://graph.microsoft.us/.default":
                    return ScopeType.Graph;
                    break;
                case "https://outlook.office365.us/.default":
                    return ScopeType.Exchange;
                    break;
                case "https://analysis.windows.net/powerbi/api/.default":
                    return ScopeType.PowerBI;
                    break;
                default:
                    if (scopes[0].ToLower() ==
                        $"https://{AuthMan.GetTenantString().ToLower().TrimEnd('/')}.sharepoint.us/.default")
                    {
                        return ScopeType.SharePoint;
                    }
                    if (scopes[0].ToLower() ==
                        $"https://{AuthMan.GetTenantString().ToLower().TrimEnd('/')}-admin.sharepoint.us/.default")
                    {
                        return ScopeType.SharePointAdmin;
                    }
                    break;
            }
            string msg = "";
            for (int C = 0; C < scopes.Length; C++)
            {
                msg += scopes[C];
            }
            msg = $"ERROR!  Scopes [{msg} is invalid.]";
            Err(msg);
            throw new Exception(msg);
        }
    }
}

#pragma warning restore CS0162, CS1587, CS1998, IDE0028, IDE0059
