/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using static Extensions.Identity.AuthMan;

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
        public static string[] Exchange
        {
            get
            {
                if (TargetTenantConfig == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    TargetTenantConfig = tenantConfig;
                }
                return new string[]
                {
                    $"https://outlook.office365{TargetTenantConfig.AuthorityDomain}/.default"
                };
            }
        }

        /// <summary>
        /// Access scope for Graph.
        /// </summary>
        public static string[] Graph
        {
            get
            {
                if (TargetTenantConfig == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    TargetTenantConfig = tenantConfig;
                }
                return new string[]
                {
                    $"https://graph.microsoft{TargetTenantConfig.AuthorityDomain}/.default",
                    Offline
                };
            }
        }

        /// <summary>
        /// Access scope for M365 Management.
        /// </summary>
        public static string[] Management
        {
            get
            {
                if (TargetTenantConfig == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    TargetTenantConfig = tenantConfig;
                }
                return new string[]
                {
                    $"https://manage.office365{TargetTenantConfig.AuthorityDomain}/.default",
                    Offline
                };
            }
        }

        /// <summary>
        /// Access scope for PowerBI.
        /// </summary>
        public static string[] PowerBI
        {
            get
            {
                if (TargetTenantConfig == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    TargetTenantConfig = tenantConfig;
                }
                return new string[]
                {
                        TargetTenantConfig.AuthorityDomain == ".us"
                            ? "https://high.analysis.usgovcloudapi.net/powerbi/api/.default"
                            : "https://analysis.windows.net/powerbi/api/.default",
                        Offline
                };
            }
        }
        //"https://analysis.windows.net/powerbi/api/.default"

        /// <summary>
        /// Access scope for SharePoint.
        /// </summary>
        public static string[] SharePoint
        {
            get
            {
                if (TargetTenantConfig == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    TargetTenantConfig = tenantConfig;
                }
                return new string[]
                {
                    $"https://{TargetTenantConfig.TenantString.TrimEnd('/')}" +
                        $".sharepoint{TargetTenantConfig.AuthorityDomain}/.default",
                    Offline
                };
            }
        }

        /// <summary>
        /// Access scope for SharePoint Admin portal.
        /// </summary>
        public static string[] SharePointAdmin
        {
            get
            {
                if (TargetTenantConfig == null)
                {
                    TenantConfig tenantConfig = new TenantConfig();
                    tenantConfig.LoadConfig();
                    TargetTenantConfig = tenantConfig;
                }
                return new string[]
                {
                    $"https://{TargetTenantConfig.TenantString.TrimEnd('/')}" +
                        $"-admin.sharepoint{TargetTenantConfig.AuthorityDomain}/.default",
                    Offline
                };
            }
        }

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
                default:
                    return Scopes.Graph;
                    break;
            }
        }

        /// <summary>
        /// Method to convert a string array of scopes back to a ScopeType object.
        /// </summary>
        /// <param name="scopes">The string array of scopes.</param>
        /// <returns>The associated ScopeType object.</returns>
        /// <exception cref="Exception">Thrown if conversion fails.</exception>
        public static ScopeType GetScopeType(string[] scopes)
        {
            if (scopes[0].ToLower().Contains("graph.microsoft"))
            {
                return ScopeType.Graph;
            }
            if (scopes[0].ToLower().Contains("outlook.office365"))
            {
                return ScopeType.Exchange;
            }
            if (scopes[0].ToLower().Contains("powerbi"))
            {
                return ScopeType.PowerBI;
            }
            if (scopes[0].ToLower().Contains("admin.sharepoint"))
            {
                return ScopeType.SharePointAdmin;
            }
            if (scopes[0].ToLower().Contains("sharepoint"))
            {
                return ScopeType.SharePoint;
            }
            string msg = "";
            for (int C = 0; C < scopes.Length; C++)
            {
                msg += $"[{scopes[C]}],";
            }
            msg = $"ERROR!  Scopes [{msg} is invalid.]";
            throw new Exception(msg);
        }
    }
}
