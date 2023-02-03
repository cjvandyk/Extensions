#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

using static Extensions.Identity.AuthMan;
using Microsoft.Graph;
using System.Linq;

namespace Extensions
{
    /// <summary>
    /// An extension class that makes working with Microsoft.Graph in GCCHigh
    /// environments easy.
    /// </summary>
    public static class Graph
    {
        /// <summary>
        /// A method to get a Group by name.
        /// </summary>
        /// <param name="name">The name of the Group to get.</param>
        /// <returns>The Group object if found, else null.</returns>
        public static Group GetGroup(string name)
        {
            //Setup a list of QueryOption values.
            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("$count", "true"),
                new QueryOption("$filter", $"displayName eq '{name}'")
            };
            //Query using the specified options.
            var groups = ActiveAuth.GraphClient.Groups
                .Request(queryOptions)
                .Header("ConsistencyLevel", "eventual")
                .GetAsync().GetAwaiter().GetResult();
            //There should only be 1 group.  If so, return it.
            if ((groups != null) && (groups.Count == 1))
            {
                return groups[0];
            }
            return null;
        }

        /// <summary>
        /// A method to duplicate an existing M365 group.
        /// </summary>
        /// <param name="groupName">The name of the group to duplicate.</param>
        /// <param name="newName">The name to use for the new group.</param>
        /// <returns></returns>
        public static string DuplicateGroup(string groupName, string newName)
        {
            //Setup query options.
            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("$count", "true"),
                new QueryOption("$filter", $"displayName eq '{groupName}'")
            };
            //Find the target group.
            var groups = ActiveAuth.GraphClient.Groups
                .Request(queryOptions)
                .Header("ConsistencyLevel", "eventual")
                .GetAsync().GetAwaiter().GetResult();
            //If the group is not found, return null.
            if (groups.Count != 1)
            {
                return null;
            }
            //Setup group types.  Unified = M365
            List<string> groupTypes = new List<string>()
            {
                "Unified"
            };
            //Duplicate group info except name.
            var newGroupInfo = new Group
            {
                Description = groups[0].Description,
                DisplayName = newName,
                GroupTypes = (IEnumerable<string>)groupTypes,
                MailEnabled = groups[0].MailEnabled,
                MailNickname = newName,
                SecurityEnabled = groups[0].SecurityEnabled,
                AdditionalData = groups[0].AdditionalData
            };
            //Create the new group.
            var newGroup = ActiveAuth.GraphClient.Groups
                .Request()
                .AddAsync(newGroupInfo)
                .GetAwaiter().GetResult();
            //Return the ID of the new group.
            return newGroup.Id;
        }

        /// <summary>
        /// A method to retrieve a list of ID values (GUID) for all members
        /// of a specified Group.
        /// </summary>
        /// <param name="groupId">The ID (GUID) of the target group.</param>
        /// <returns>A list of strings representing the IDs of member 
        /// users.</returns>
        public static List<string> GetMembers(string groupId)
        {
            //Create the aggregation container.
            List<string> members = new List<string>();
            List<User> users = new List<User>();
            //Get the first page of members.
            var usersPage = ActiveAuth.GraphClient.Groups[groupId].Members
                .Request().GetAsync().GetAwaiter().GetResult();
            //Add members to the temporary list.
            users.AddRange(usersPage.CurrentPage.OfType<User>());
            //Recurively iterate until all members are found.
            while (usersPage.NextPageRequest != null)
            {
                usersPage = usersPage.NextPageRequest
                    .GetAsync().GetAwaiter().GetResult();
                users.AddRange(usersPage.CurrentPage.OfType<User>());
            }
            //Iterate the temporary list and grab the IDs.
            foreach (var user in users)
            {
                members.Add(user.Id);
            }
            //Return the list of IDs.
            return members;
        }

        /// <summary>
        /// Method to add one or more users to a group.
        /// </summary>
        /// <param name="groupId">The ID of the target group.</param>
        /// <param name="userIds">A list of IDs (GUIDs) of users to add to
        /// the target group.</param>
        public static async void CreateMembers(string groupId, List<string> userIds)
        {
            //During bulk operations, this method may be called many times.
            //Yield to prevent UI lockup.
            System.Threading.Thread.Yield();
            //Add the Graph location prefix.
            //NOTE: The use of .com for both Commercial and GCCHigh tenants.
            for (int C = 0; C < userIds.Count; C++)
            {
                userIds[C] = $"https://graph.microsoft.com/v1.0/directoryObjects/{userIds[C]}";
            }
            var group = new Group
            {
                AdditionalData = new Dictionary<string, object>()
                {
                    {"members@odata.bind", (object)userIds}
                }
            };
        }

        /// <summary>
        /// Get the site ID (GUID) of the specified site.
        /// </summary>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <returns>A GUID value representing the ID of the site.</returns>
        public static string GetSiteId(string sitePath)
        {
            return (ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Request()
                        .GetAsync().GetAwaiter().GetResult()).Id;
        }

        /// <summary>
        /// Get a list of all sites in SharePoint.
        /// </summary>
        /// <returns>A list of all sites in SharePoint.</returns>
        public static List<Site> GetSites()
        {
            //Create the aggregation container.
            var sites = new List<Site>();
            //Get the first page of sites.
            var sitePage1 = ActiveAuth.GraphClient.Sites.Request()
                .GetAsync().GetAwaiter().GetResult();
            //Create the iterator for multiple pages.
            var sitePageIterator = PageIterator<Site>.CreatePageIterator(
                ActiveAuth.GraphClient, 
                sitePage1,
                (C) =>
                {
                    sites.Add(C);
                    return true;
                });
            //Iterate pages to get all data.
            sitePageIterator.IterateAsync().GetAwaiter().GetResult();
            return sites;
        }

        /// <summary>
        /// Get the list ID (GUID) of the specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <returns>A GUID value representing the ID of the list.</returns>
        public static string GetListId(string listName, string sitePath)
        {
            return (ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Lists[listName]
                        .Request()
                        .GetAsync().GetAwaiter().GetResult()).Id;
        }

        /// <summary>
        /// Get a specific item in a specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <param name="id">The ID value of the item e.g. "3"</param>
        /// <returns>A list of ListItem containing the single item.</returns>
        public static List<ListItem> GetListItem(
            string listName,
            string sitePath,
            string id)
        {
            return GetListItems(listName, sitePath, id);
        }

        /// <summary>
        /// Get all or just one item(s) in a specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <param name="id">The ID value of the item e.g. "3".  This value
        /// defaults to null and in such case, will result in all items being
        /// returned.</param>
        /// <returns>A list of ListItem containing the item(s).</returns>
        public static List<ListItem> GetListItems(
            string listName,
            string sitePath,
            string id = null)
        {
            //Create the aggregation container.
            List<ListItem> listItems = new List<ListItem>();
            //Check if a specific item was requested.
            if (id != null)
            {
                try
                {
                    //Get the specified item.
                    var listItem = ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Lists[listName]
                        .Items[id]
                        .Request()
                        .Expand("Fields")
                        .GetAsync().GetAwaiter().GetResult();
                    //Check if the item was found.
                    if (listItem != null)
                    {
                        listItems.Add(listItem);
                    }                        
                }
                catch (Exception ex)
                {
                    //Swallow exception if no with given id exist.
                }
            }
            else
            {
                //Create the first page.
                IListItemsCollectionPage listItemsPage1 = null;
                try
                {
                    //Get the first page of results.
                    listItemsPage1 = ActiveAuth.GraphClient.Sites["root"]
                        .SiteWithPath(sitePath)
                        .Lists[listName]
                        .Items
                        .Request()
                        .Expand("Fields")
                        .GetAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Requested site could not be found"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }
                //If not results were found, return the empty list.
                if (listItemsPage1 == null)
                {
                    return listItems;
                }
                //Results were found so create the page iterator.
                var listItemsPageIterator = PageIterator<ListItem>
                    .CreatePageIterator(ActiveAuth.GraphClient,
                                        listItemsPage1,
                                        (C) =>
                                        {
                                            listItems.Add(C);
                                            return true;
                                        });
                //Get all pages.
                listItemsPageIterator.IterateAsync().GetAwaiter().GetResult();
            }
            //Return the aggregated list.
            return listItems;
        }

        /// <summary>
        /// Update a specified list item with specified values.
        /// </summary>
        /// <param name="siteGuid">The site ID (GUID) value.</param>
        /// <param name="listGuid">The list ID (GUID) value.</param>
        /// <param name="listItemId">The ID value of the item e.g. "3"</param>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to update.</param>
        public static void UpdateListItemFields(
            string siteGuid,
            string listGuid,
            string listItemId,
            Dictionary<string, object> newFields)
        {
            ActiveAuth.GraphClient.Sites[siteGuid]
                                  .Lists[listGuid]
                                  .Items[listItemId]
                                  .Fields
                                  .Request()
                                  .UpdateAsync(CreateFieldValueSet(newFields))
                                  .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a new list item with specified values.
        /// </summary>
        /// <param name="siteGuid">The site ID (GUID) value.</param>
        /// <param name="listGuid">The list ID (GUID) value.</param>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to update.</param>
        public static void CreateListItem(
            string siteGuid,
            string listGuid,
            Dictionary<string, object> newFields)
        {
            ActiveAuth.GraphClient.Sites[siteGuid]
                                  .Lists[listGuid]
                                  .Items
                                  .Request()
                                  .AddAsync(CreateListItemJSON(newFields))
                                  .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a new FieldValueSet JSON object.
        /// </summary>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to use.</param>
        /// <returns>A FieldValueSet object populated with the values from
        /// the dictionary.</returns>
        internal static FieldValueSet CreateFieldValueSet(
            Dictionary<string, object> newFields)
        {
            return new FieldValueSet
            {
                AdditionalData = newFields
            };
        }

        /// <summary>
        /// Create a new ListItem JSON object.
        /// </summary>
        /// <param name="newFields">A dictionary of objects representing the
        /// field names and values to use.</param>
        /// <returns>A ListItem object populated with the values from the
        /// dictionary.</returns>
        internal static ListItem CreateListItemJSON(
            Dictionary<string, object> newFields)
        {
            return new ListItem
            {
                Fields = CreateFieldValueSet(newFields)
            };
        }
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
