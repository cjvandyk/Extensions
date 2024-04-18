/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Extensions.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;

namespace Extensions
{
    /// <summary>
    /// An extension class to simplify working with Users.
    /// </summary>
    public static partial class Graph
    {
        /// <summary>
        /// A method to get a Microsoft.Graph.Models.User object given its
        /// email address.
        /// </summary>
        /// <param name="email">The email address of the target user.</param>
        /// <returns>A Microsoft.Graph.Models.User object if found, else 
        /// null.</returns>
        public static User GetUserByEmail(string email)
        {
            try
            {
                return AuthMan.ActiveAuth.GraphClient
                    .Users
                    .GetAsync(C =>
                    {
                        C.QueryParameters.Filter = $"mail eq '{email}'";
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult().Value[0];
            }
            catch (Exception ex)
            {
                Logit.Err(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Method to get the user given a user lookup id from SharePoint.
        /// </summary>
        /// <param name="id">The SharePoint list id of the user.</param>
        /// <param name="siteUsers">A reference to the list of users.</param>
        /// <returns>The user associated with the id or null.</returns>
        public static Microsoft.Graph.Models.ListItem GetUserFromLookupId(
            string id,
            ref List<Microsoft.Graph.Models.ListItem> siteUsers)
        {
            foreach (var item in siteUsers)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Method to get a user's EMail or UserName.  Particulary useful for
        /// translating SharePoint UserLookupIds like LastModified or Author
        /// from a numeric value to the UPN string.
        /// </summary>
        /// <param name="listItem">A ListItem instance from the site's 
        /// UserInformation list.</param>
        /// <returns>The EMail or UserName value from a ListItem instance 
        /// from the UserInformation list.
        /// </returns>
        public static string GetUserEmailUpn(
            Microsoft.Graph.Models.ListItem listItem)
        {
            if (listItem.Fields.AdditionalData.TryGetValue("EMail", out object result))
            {
                return result.ToString();
            }
            if (listItem.Fields.AdditionalData.TryGetValue("UserName", out result))
            {
                return result.ToString();
            }
            return null;
        }

        /// <summary>
        /// Method to get the user's UPN given a SharePoint lookup id.  
        /// Particulary useful for translating SharePoint UserLookupIds like 
        /// LastModified or Author from a numeric value to the UPN string.
        /// </summary>
        /// <param name="id">The lookup id of the user.</param>
        /// <param name="client">An authenticated GraphServiceClient object.</param>
        /// <returns>The EMail or UserName value from a ListItem instance 
        /// from the UserInformation list.</returns>
        public static string GetUserEmailUpn(
            string id,
            GraphServiceClient client)
        {
            try
            {
                if ((id == null) ||
                    (id == ""))
                {
                    return null;
                }
                var userListItems = Graph.GetListItems("User Information List",
                                                 TenantConfig.GetEnv("HomeSiteBaseUrl"),
                                                 id);
                if ((userListItems != null) &&
                    (userListItems.Count > 0) &&
                    (userListItems[0].Fields.AdditionalData.TryGetValue("EMail", out object result)))
                {
                    return result.ToString();
                }
                if (userListItems[0].Fields.AdditionalData.TryGetValue("UserName", out result))
                {
                    return result.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                Logit.Err(ex.ToString());
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Method to get the user's UPN given a SharePoint lookup id.  
        /// Particulary useful for translating SharePoint UserLookupIds like 
        /// LastModified or Author from a numeric value to the UPN string.
        /// </summary>
        /// <param name="id">The lookup id of the user.</param>
        /// <param name="siteUsers">A reference to the list of users.</param>
        /// <returns>The EMail or UserName value from a ListItem instance 
        /// from the UserInformation list.</returns>
        public static string GetUserEmailUpn(
            string id,
            ref List<Microsoft.Graph.Models.ListItem> siteUsers)
        {
            var item = GetUserFromLookupId(id, ref siteUsers);
            if (item == null)
            {
                return null;
            }
            try
            {
                if (item.Fields.AdditionalData.TryGetValue("EMail", out object result))
                {
                    return result.ToString();
                }
                try
                {
                    Logit.Wrn("Cannot get EMail field, trying UserName instead.");
                    if (item.Fields.AdditionalData.TryGetValue("UserName", out result))
                    {
                        if ((string)result == "app@sharepoint")
                        {
                            return null;
                        }
                        return result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    //Both fields don't exist so just drop through.
                }
                Logit.Err("Cannot get UserName field either.");
                return null;
            }
            catch (Exception ex)
            {
                Logit.Wrn("Cannot get EMail field, trying UserName instead.");
                if (item.Fields.AdditionalData.TryGetValue("UserName", out object result))
                {
                    return result.ToString();
                }
                Logit.Err("Cannot get UserName field either.\n" + ex.ToString());
                return null;
            }
            finally
            {
            }
        }
    }
}
