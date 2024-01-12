#pragma warning disable CS0168, CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Graph.Groups;
using Microsoft.Graph.Models;
using Microsoft.SharePoint.News.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;
using static Extensions.Identity.AuthMan;
using static System.Logit;

namespace Extensions
{
    /// <summary>
    /// An extension class for working with batch requests in Microsoft.Graph.
    /// </summary>
    [Serializable]
    public partial class BatchGroup
    {
        /// <summary>
        /// A collection of batch requests.
        /// </summary>
        public BatchRequestContentCollection batch { get; set;}
        /// <summary>
        /// A list of string IDs for requests contained in the batch.
        /// </summary>
        public List<string> ids { get; set;}
    }

    /// <summary>
    /// An extension class that makes working with Microsoft.Graph in GCCHigh
    /// environments easy.
    /// </summary>
    [Serializable]
    public static partial class Graph
    {
        /// <summary>
        /// A method to get a Group by name.
        /// </summary>
        /// <param name="name">The name of the Group to get.</param>
        /// <returns>The Group object if found, else null.</returns>
        public static Group GetGroup(string name)
        {
            var groups = ActiveAuth.GraphClient.Groups.GetAsync((C) =>
            {
                C.QueryParameters.Filter = $"displayName eq '{name}'";
                C.Headers.Add("ConsistencyLevel", "eventual");
            }).GetAwaiter().GetResult().Value;
            //There should only be 1 group.  If so, return it.
            if ((groups != null) && (groups.Count == 1))
            {
                return groups[0];
            }
            return null;
        }

        /// <summary>
        /// A method to retrieve a list containing all the Microsoft.Graph.Group
        /// values in the tenant that match the specified filter value.
        /// </summary>
        /// <param name="filter">An OData filter string to apply.</param>
        /// <returns>A list containing all the Microsoft.Graph.Group values in
        /// the tenant matching the given OData filter.</returns>
        public static List<Group> GetGroups(string filter)
        {
            var groups = new List<Group>();
            GetGroups(ref groups, filter);
            return groups;
        }

        /// <summary>
        /// A method to retrieve a list containing all the Microsoft.Graph.Group
        /// values in the tenant.
        /// </summary>
        /// <param name="groups">A reference to the aggregation container.</param>
        /// <param name="filter">An OData filter string to apply.</param>
        /// <returns>A list containing all the Microsoft.Graph.Group values in
        /// the tenant matching the given OData filter.</returns>
        internal static void GetGroups(ref List<Group> groups, string filter = "")
        {
            //Get the first page of groups.
            GroupCollectionResponse groupsPage = null;
            if (filter == "")
            {
                //There's no filter, so get all Groups.
                groupsPage = ActiveAuth.GraphClient.Groups
                    .GetAsync().GetAwaiter().GetResult();
            }
            else
            {
                //Apply the specified filter to the Groups request.
                groupsPage = ActiveAuth.GraphClient.Groups
                    .GetAsync((C) =>
                    {
                        C.QueryParameters.Filter = filter;
                        //C.QueryParameters.Select = new string[] { "id", "displayName" };
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
            }
            GetGroupsPages(ref groups, ref groupsPage);
        }

        /// <summary>
        /// A method to iterate a page collection reference and add retrieved
        /// items to an aggregation container reference.
        /// </summary>
        /// <param name="groups">A reference to the aggregation container.</param>
        /// <param name="groupsPage">A reference to the first page to iterate.</param>
        internal static void GetGroupsPages(
            ref List<Group> groups,
            ref GroupCollectionResponse groupsPage)
        {
            while (groupsPage.Value != null)
            {
                foreach (var group in groupsPage.Value)
                {
                    lock (groups)
                    {
                        groups.Add(group);
                    }
                }
                if (!string.IsNullOrEmpty(groupsPage.OdataNextLink))
                {
                    groupsPage = ActiveAuth.GraphClient.Groups
                        .WithUrl(groupsPage.OdataNextLink)
                        .GetAsync().GetAwaiter().GetResult();
                }
                WriteCount(groups.Count);
            }
        }

        ///// <summary>
        ///// A method to retrieve a list containing all the Microsoft.Graph.Group
        ///// values in the tenant that match the specified filter value.
        ///// </summary>
        ///// <param name="filter">An OData filter string to apply.</param>
        ///// <param name="fields">An array of field names to return.</param>
        ///// <returns>A list containing all the Microsoft.Graph.Group values in
        ///// the tenant matching the given OData filter.</returns>
        //public static List<Group> GetGroups(
        //    string filter,
        //    string[] fields = null)
        //{
        //    var groups = new List<Group>();
        //    GetGroups(ref groups, filter, fields);
        //    return groups;
        //}

        ///// <summary>
        ///// A method to retrieve a list containing all the Microsoft.Graph.Group
        ///// values in the tenant.
        ///// </summary>
        ///// <param name="groups">A reference to the aggregation container.</param>
        ///// <param name="filter">An OData filter string to apply e.g.
        ///// "displayName eq 'Extensions'.</param>
        ///// <param name="fields">A string array of fields to retrieve.
        ///// If null, no specific fields are targeted.</param>
        ///// <returns>A list containing all the Microsoft.Graph.Group values in
        ///// the tenant matching the given OData filter.</returns>
        //internal static void GetGroups(
        //    ref List<Group> groups, 
        //    string filter = "",
        //    string[] fields = null)
        //{
        //    try
        //    {
        //        GroupCollectionResponse groupCollectionResponse = null;
        //        List<string> ids = new List<string>();
        //        //ids = (List<string>)(object)ids.Load<List<string>>(@"GroupIds.bin");
        //        List<Group> result = new List<Group>();
        //        List<BatchGroup> batchGroups = new List<BatchGroup>();
        //        //using var ps = PowerShell.Create();
        //        //ps.AddCommand("Set-ExecutionPolicy")
        //        //    .AddParameter("ExecutionPolicy", "RemoteSigned")
        //        //    .AddParameter("Scope", "CurrentUser")
        //        //    .AddParameter("Force", "");
        //        //ps.AddCommand("Install-Module")
        //        //    .AddParameter("Name", "ExchangeOnlineManagement")
        //        //    .AddParameter("AllowClobber", "True")
        //        //    .AddParameter("Confirm", "True");
        //        //ps.AddCommand("Import-Module")
        //        //    .AddParameter("Name", "ExchangeOnlineManagement");
        //        //ps.AddCommand("Connect-ExchangeOnlineManagement")
        //        //    .AddParameter("ExchangeEnvironmentName", "O365USGovGCCHigh")
        //        //    .AddParameter("AccessToken", ActiveAuth.AuthResult.AccessToken);
        //        //ps.AddScript("$groups = GetUnifiedGroup; " +
        //        //             "foreach ($group in $groups)" +
        //        //             "{" +
        //        //             "  write-information $group;" +
        //        //             "}");
        //        //PSDataCollection<PSObject> output = new PSDataCollection<PSObject>();
        //        //output.DataAdded += (s, e) =>
        //        //{
        //        //    PSObject newRecord = ((PSDataCollection<PSObject>)s)[e.Index];
        //        //    Inf(newRecord.ToString());
        //        //};
        //        //var proc = ps.BeginInvoke<PSObject, PSObject>(null, output);
        //        //proc.AsyncWaitHandle.WaitOne();
        //        Inf(DateTime.Now.ToString());
        //        int count = 0;
        //        while (count < ids.Count)
        //        {
        //            var idBatch = ids.Take(20).ToList();
        //            count += 20;
        //            BatchGroup bg = new BatchGroup();
        //            bg.batch = new BatchRequestContentCollection(ActiveAuth.GraphClient);
        //            foreach (var id in idBatch)
        //            {
        //                var groupRequest = ActiveAuth.GraphClient.Groups[id]
        //                    .ToGetRequestInformation((C) =>
        //                {
        //                    C.Headers.Add("ConsistencyLevel", "eventual");
        //                    C.QueryParameters.Select = fields;
        //                });
        //                bg.ids.Add(bg.batch.AddBatchRequestStepAsync(
        //                    groupRequest).GetAwaiter().GetResult());
        //            }
        //            batchGroups.Add(bg);
        //            Console.Write(count.ToString());
        //            Console.CursorLeft = 0;
        //        }
        //        Console.WriteLine("\n\r");
        //        foreach (var batchChunk in batchGroups.Chunk(200))
        //        {
        //            Parallel.ForEach(batchChunk.ToList(), batchGroup =>
        //            {
        //                var ret = ActiveAuth.GraphClient.Batch.PostAsync(
        //                    batchGroup.batch).GetAwaiter().GetResult();
        //                foreach (var id in batchGroup.ids)
        //                {
        //                    try
        //                    {
        //                        lock (result)
        //                        {
        //                            result.Add(
        //                                ret.GetResponseByIdAsync<Group>(id)
        //                                .GetAwaiter().GetResult());
        //                        }
        //                    }
        //                    catch (HttpRequestException http)
        //                    {
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                }
        //                Console.Write(result.Count);
        //                Console.CursorLeft = 0;
        //            });
        //        }
        //        Inf(DateTime.Now.ToString());

        //        Parallel.ForEach(ids, id =>
        //        {
        //            try
        //            {
        //                var group = ActiveAuth.GraphClient.Groups[id]
        //                    .GetAsync(C =>
        //                    {
        //                        C.Headers.Add("ConsistencyLevel", "eventual");
        //                        C.QueryParameters.Select = fields;
        //                    }).GetAwaiter().GetResult();
        //                lock (result)
        //                {
        //                    result.Add(group);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        });
        //        //var res = (object)result.Save("Groups.bin");

        //        //Setup query configuration and get the first page.
        //        Microsoft.Kiota.Abstractions.RequestConfiguration<GroupsRequestBuilder.GroupsRequestBuilderGetQueryParameters> cfg =
        //            new Microsoft.Kiota.Abstractions.RequestConfiguration<GroupsRequestBuilder.GroupsRequestBuilderGetQueryParameters>();
        //        groupCollectionResponse = ActiveAuth.GraphClient.Groups
        //            .GetAsync((C) =>
        //            {
        //                Configure(ref C, filter, fields);
        //            }).GetAwaiter().GetResult();
        //        //Check if more than one page of results exist.
        //        if (groupCollectionResponse.OdataNextLink != null)
        //        {
        //            //If it does, create a PageIterator to get all pages.
        //            var pageIterator = PageIterator<Group, GroupCollectionResponse>
        //                .CreatePageIterator(
        //                    ActiveAuth.GraphClient,
        //                    groupCollectionResponse,
        //                    (group) =>
        //                    {
        //                        result.Add(group);
        //                        return true;
        //                    },
        //                    (req) =>
        //                    {
        //                        req.Headers.Add("ConsistencyLevel", "eventual");
        //                        Inf(result.Count.ToString());
        //                        return req;
        //                    });
        //            //Now let the iterator get everything.
        //            pageIterator.IterateAsync().GetAwaiter().GetResult();
        //        }
        //        else
        //        {
        //            //Only one page of results, so add it to the list.
        //            result.AddRange(groupCollectionResponse.Value);
        //        }
        //        Inf(DateTime.Now.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        Err(ex.ToString());
        //    }
        //}

        //static void Configure(
        //    ref Microsoft.Kiota.Abstractions.RequestConfiguration<GroupsRequestBuilder.GroupsRequestBuilderGetQueryParameters> cfg,
        //    string filter = "",
        //    string[] fields = null)
        //{
        //    //Add default query parameters.
        //    cfg.Headers.Add("ConsistencyLevel", "eventual");
        //    cfg.QueryParameters.Top = 999;
        //    //If a filter is specified, add it.
        //    if ((filter != null) &&
        //        (filter.Length > 0))
        //    {
        //        cfg.QueryParameters.Filter = filter;
        //    }
        //    //If fields are specified, add them.
        //    if ((fields != null) &&
        //        (fields.Length > 0))
        //    {
        //        cfg.QueryParameters.Select = new string[] { "id", "displayName" };
        //        //cfg.QueryParameters.Expand = fields;
        //    }
        //}

        /////// <summary>
        /////// A method to iterate a page collection reference and add retrieved
        /////// items to an aggregation container reference.
        /////// </summary>
        /////// <param name="groups">A reference to the aggregation container.</param>
        /////// <param name="page">A reference to the first page to iterate.</param>
        ////internal static void GetGroupsPages(
        ////    ref List<Group> groups, 
        ////    ref IGraphServiceGroupsCollectionPage page)
        ////{
        ////    //Add items to the temporary list.
        ////    groups.AddRange(page.CurrentPage.OfType<Group>());
        ////    //Recurively iterate until all items are found.
        ////    while (page.NextPageRequest != null)
        ////    {
        ////        page = page.NextPageRequest
        ////            .GetAsync().GetAwaiter().GetResult();
        ////        groups.AddRange(page.CurrentPage.OfType<Group>());
        ////        //Add visual feedback for command line usage.
        ////        WriteCount(groups.Count);
        ////    }
        ////}

        /// <summary>
        /// A method to aggregate all Groups in the tenant across 36 threads,
        /// returning the cumulative list of Groups.
        /// </summary>
        /// <returns>A list of all Groups in the tenant.</returns>
        public static List<Group> GetGroups()
        {
            var groups = new List<Group>();
            List<string> starters = new List<string>()
            {
                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o",
                "p","q","r","s","t","u","v","w","x","y","z","1","2","3","4",
                "5","6","7","8","9","0"
            };
            foreach (var starter in starters)
            {
                GetGroups(
                    ref groups, 
                    $"startswith(displayName, '{starter}')");
            }
            //Parallel.ForEach(starters, starter =>
            //{
            //    GetGroups(ref groups, $"startswith(displayName, '{starter}')");
            //});
            Inf(groups.Count.ToString());
            return groups;
        }

        /// <summary>
        /// A method to duplicate an existing M365 group.
        /// </summary>
        /// <param name="groupName">The name of the group to duplicate.</param>
        /// <param name="newName">The name to use for the new group.</param>
        /// <returns></returns>
        public static string DuplicateGroup(
            string groupName, 
            string newName)
        {
            //Find the target group.
            var groups = ActiveAuth.GraphClient.Groups
                .GetAsync((C) =>
                {
                    C.QueryParameters.Search = $"\"displayName:{groupName}\"";
                    C.Headers.Add("ConsistencyLevel", "eventual");
                }).GetAwaiter().GetResult();
            //If the group is not found, return null.
            if (groups.Value.Count != 1)
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
                Description = groups.Value[0].Description,
                DisplayName = newName,
                GroupTypes = groupTypes,
                MailEnabled = groups.Value[0].MailEnabled,
                MailNickname = newName,
                SecurityEnabled = groups.Value[0].SecurityEnabled,
                AdditionalData = groups.Value[0].AdditionalData
            };
            //Create the new group.
            var newGroup = ActiveAuth.GraphClient.Groups
                .PostAsync(newGroupInfo)
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
            var usersPage = ActiveAuth.GraphClient.Groups[groupId]
                .Members.GraphUser.GetAsync(C =>
                {
                    C.QueryParameters.Select = new string[] { "id" };
                }).GetAwaiter().GetResult();
            if (usersPage.Value.Count == 0)
            {
                return members;
            }
            var pageIterator = PageIterator<User, UserCollectionResponse>
                .CreatePageIterator(
                    ActiveAuth.GraphClient,
                    usersPage,
                    (user) =>
                    {
                        members.Add(user.Id);
                        return true;
                    });
            pageIterator.IterateAsync().GetAwaiter().GetResult();
            Inf(members.Count.ToString());
            return members;
        }

        /// <summary>
        /// A method to retrieve a list of ID values (GUID) for all owners
        /// of a specified Group.
        /// </summary>
        /// <param name="groupId">The ID (GUID) of the target group.</param>
        /// <returns>A list of strings representing the IDs of owner 
        /// users.</returns>
        public static List<string> GetOwners(string groupId)
        {
            //Create the aggregation container.
            List<string> owners = new List<string>();
            List<User> users = new List<User>();
            //Get the first page of owners.
            var usersPage = ActiveAuth.GraphClient.Groups[groupId]
                .Owners.GraphUser.GetAsync(C =>
                {
                    C.QueryParameters.Select = new string[] { "id" };
                }).GetAwaiter().GetResult();
            if (usersPage.Value.Count == 0)
            {
                return owners;
            }
            var pageIterator = PageIterator<User, UserCollectionResponse>
                .CreatePageIterator(
                    ActiveAuth.GraphClient,
                    usersPage,
                    (user) =>
                    {
                        owners.Add(user.Id);
                        return true;
                    });
            pageIterator.IterateAsync().GetAwaiter().GetResult();
            Inf(owners.Count.ToString());
            return owners;
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
                userIds[C] = $"https://graph.microsoft.us/v1.0/directoryObjects/{userIds[C]}";
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
            return (ActiveAuth.GraphClient.Sites[$"root:{sitePath}"]
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
            var sitesPage = ActiveAuth.GraphClient.Sites
                .GetAsync().GetAwaiter().GetResult();
            var pageIterator = PageIterator<Site, SiteCollectionResponse>
                .CreatePageIterator(
                    ActiveAuth.GraphClient,
                    sitesPage,
                    (site) =>
                    {
                        sites.Add(site);
                        return true;
                    });
            Inf(sites.Count.ToString());
            return sites;
        }

        /// <summary>
        /// Get the list ID (GUID) of the specified list.
        /// </summary>
        /// <param name="listName">The name of the list e.g. "Documents"</param>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <returns>A GUID value representing the ID of the list.</returns>
        public static string GetListId(
            string listName, 
            string sitePath)
        {
            return ActiveAuth.GraphClient.Sites[GetSiteId(sitePath)]
                .Lists.GetAsync((C) =>
                {
                    C.QueryParameters.Filter = $"displayName eq '{listName}'";
                }).GetAwaiter().GetResult().Value[0].Id;
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
        /// Method to get a field value from a ListItem object.
        /// </summary>
        /// <param name="item">The item to get data from.</param>
        /// <param name="field">The field to target.</param>
        /// <returns>The value of the field in the item.</returns>
        public static string GetFld(this ListItem item, string field)
        {
            return item.Fields.AdditionalData[field].ToString();
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
        /// <param name="filter">The filter syntax to use.</param>
        /// <returns>A list of ListItem containing the item(s).</returns>
        public static List<ListItem> GetListItems(
            string listName,
            string sitePath,
            string id = null,
            string filter = null)
        {
            //Create the aggregation container.
            List<ListItem> listItems = new List<ListItem>();
            //Check if a specific item was requested.
            if (id != null)
            {
                try
                {
                    //Get the specified item.
                    var listItem = ActiveAuth.GraphClient.Sites[GetSiteId(sitePath)]
                        .Lists[GetListId(listName, sitePath)]
                        .Items[id]
                        .GetAsync((C) =>
                        {
                            C.QueryParameters.Expand = new string[] { "fields" };
                        }).GetAwaiter().GetResult();
                    //Check if the item was found.
                    if (listItem != null)
                    {
                        listItems.Add(listItem);
                    }                        
                }
                catch (Exception ex)
                {
                    Err(ex.ToString());
                }
            }
            else
            {
                try
                {
                    var listItemCollectionResponse =
                        ActiveAuth.GraphClient.Sites[GetSiteId(sitePath)]
                        .Lists[GetListId(listName, sitePath)]
                        .Items.GetAsync((C) =>
                        {
                            if (filter != null)
                            {
                                C.QueryParameters.Filter = filter;
                            }
                            C.QueryParameters.Expand = new string[] { "fields" };
                        }).GetAwaiter().GetResult();
                    if (listItemCollectionResponse == null)
                    {
                        return listItems;
                    }
                    var pageIterator = PageIterator<ListItem, ListItemCollectionResponse>
                        .CreatePageIterator(
                            ActiveAuth.GraphClient,
                            listItemCollectionResponse,
                            (listItem) =>
                            {
                                listItems.Add(listItem);
                                return true;
                            });
                    pageIterator.IterateAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Requested site could not be found"))
                    {
                        Err(ex.ToString());
                    }
                    throw;
                }
            }
            //Return the aggregated list.
            Inf(listItems.Count.ToString());
            return listItems;
        }

        /// <summary>
        /// Get all items in a given list updated since a given datetime and
        /// with a pending status.
        /// </summary>
        /// <param name="listName">The name of the list from which to get 
        /// items e.g. "Documents".</param>
        /// <param name="sitePath">The site path of the site containing the 
        /// list e.g. "/sites/MySite"</param>
        /// <param name="dateTime">The date/time string to use to narrow
        /// down the returned result set e.g. "2017-07-04".  If not specified
        /// and the default value of null is found, this field will default
        /// to the current date/time minus one day.</param>
        /// <returns>A list of ListItem containing the item(s).</returns>
        public static List<ListItem> GetPendingListItemsUpdatedSince(
            string listName,
            string sitePath,
            string dateTime = null)
        {
            Inf(Core.GetEnv("NewItemsStatusFilter"));
            Inf($"Scope [{ActiveAuth.Scopes[0]}]");
            Inf($"GetPendingListItemsUpdatedSince(" +
                $"{listName}, {sitePath}, {dateTime}");
            if (dateTime == null)
            {
                dateTime = DateTime.Now.AddDays(-1).ToString();
            }
            var items = GetListItems(
                listName,
                sitePath,
                null,
                $"fields/Modified gt '{dateTime}'");
            Inf($"Pre-filtered items:[{items.Count}]");
            string filter = "pending";
            filter = Core.GetEnv("NewItemsStatusFilter").ToLower();
            for (int C = items.Count - 1; C >= 0; C--)
            {
                if (items[C].GetJsonString("Status").ToLower() != "filter")
                {
                    items.RemoveAt(C);
                }
            }
            Inf($"Post-filtered items:[{items.Count}]");
            return items;
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
                                  .PatchAsync(CreateFieldValueSet(newFields))
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
                                  .PostAsync(CreateListItemJSON(newFields))
                                  .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Method to create a new list item.
        /// </summary>
        /// <param name="siteGuid">The target site ID.</param>
        /// <param name="listGuid">The target list ID.</param>
        /// <param name="listItem">The ListItem to add to the target.</param>
        public static void CreateListItem(
            string siteGuid,
            string listGuid,
            ListItem listItem)
        {
            ActiveAuth.GraphClient.Sites[siteGuid]
                .Lists[listGuid]
                .Items
                .PostAsync(listItem)
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

        /// <summary>
        /// Write visible feedback to console for command line usage.  The
        /// feedback is written in a fixed location providing a counter like
        /// experience without screen scroll.
        /// </summary>
        /// <param name="count">The count to write.</param>
        internal static void WriteCount(int count)
        {
            //Capture the current cursor location.
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            //Write output.
            Console.Write(count);
            //Reset the cursor location.
            Console.CursorLeft = left;
            Console.CursorTop = top;
        }
    }
}
#pragma warning restore CS0168, CS1587, CS1998, IDE0059, IDE0028
