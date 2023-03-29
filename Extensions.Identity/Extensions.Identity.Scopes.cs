#pragma warning disable CS0162, CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;

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
        /// SharePoint scope.
        /// </summary>
        SharePoint
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
        /// Access scope for SharePoint.
        /// </summary>
        public static string[] SharePoint = new string[]
        {
            $"https://{AuthMan.GetTenantString().TrimEnd('/')}.sharepoint.us/.default",
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
                case ScopeType.SharePoint:
                    return Scopes.SharePoint;
                    break;
                case ScopeType.Exchange:
                    return Scopes.Exchange;
                    break;
            }
            return AuthMan.ActiveAuth.Scopes;
        }

    }
}
#pragma warning restore CS0162, CS1587, CS1998, IDE0059, IDE0028
