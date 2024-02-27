/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using static Microsoft.Graph.Drives.Item.Items.Item.Children.ChildrenRequestBuilder;
using static Microsoft.Graph.Groups.GroupsRequestBuilder;
using Microsoft.Graph.Models;
using static Microsoft.Graph.Users.Item.Drives.DrivesRequestBuilder;
using static Microsoft.Graph.Users.UsersRequestBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions.Identity;
using static Extensions.Constants;
using static System.Logit;
using System.IO;

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
        public BatchRequestContentCollection Batch { get; set;}
        /// <summary>
        /// A list of string IDs for requests contained in the batch.
        /// </summary>
        public List<string> Ids { get; set;}
    }

    /// <summary>
    /// An extension class that makes working with Microsoft.Graph in GCCHigh
    /// environments easy.
    /// </summary>
    [Serializable]
    public static partial class Graph
    {
        public static Drive GetDrive(User user)
        {
            var drives = GetDrives($"id eq '{user.Id}'", "");
            return drives.FirstOrDefault();
        }

        public static Drive GetDrive(string userId)
        {
            var drives = GetDrives($"id eq '{userId}'", "");
            return drives.FirstOrDefault();
        }

        public static List<Drive> GetDrives()
        {
            return GetDrives("", "", null);
        }

        public static List<Drive> GetDrives(string[] select = null)
        {
            return GetDrives("", "", select);
        }

        public static List<Drive> GetDrives(string userFilter = "",
                                            string[] select = null)
        {
            return GetDrives(userFilter, "", select);
        }

        public static List<Drive> GetDrives(string userFilter = "",
                                            string filter = "",
                                            string[] select = null)
        {
            var drives = new List<Drive>();
            var users = GetUsers(userFilter);
            Parallel.ForEach(users, user =>
            {
                GetDrives(ref drives, user, filter, select);
            });
            return drives;
        }
                
        /// <summary>
        /// A method to retrieve a list containing all the 
        /// Microsoft.Graph.Models.Drive child items for the given user and
        /// filter.  Optionally, additional Drive metadata can be requested
        /// through the select string array parameter.  The method will add
        /// all discovered Drive items to the referenced drives List.
        /// </summary>
        /// <param name="drives">A reference to the aggregation container.</param>
        /// <param name="user">The Graph User for which drives are retrieved.</param>
        /// <param name="filter">An optional OData filter string to apply.</param>
        /// <param name="select">An optional string array of additional Drive metadata
        /// field names to retrieve.</param>
        internal static void GetDrives(ref List<Drive> drives, 
                                       User user,
                                       string filter = "",
                                       string[] select = null)
        {
            //Get the first page of drives.
            DriveCollectionResponse drivesPage = null;
            DrivesRequestBuilderGetQueryParameters queryParameters = null;
            if (filter != "")
            {
                queryParameters = new DrivesRequestBuilderGetQueryParameters();
                queryParameters.Filter = filter;
            }
            if (select != null)
            {
                if (queryParameters == null)
                {
                    queryParameters = new DrivesRequestBuilderGetQueryParameters();
                }
                queryParameters.Select = select;
            }
            try
            {
                if (queryParameters == null)
                {
                    //Get all Drives.
                    drivesPage = AuthMan.ActiveAuth.GraphClient.Users[user.Id]
                        .Drives
                        .GetAsync((C) =>
                        {
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                }
                else
                {
                    //Apply the specified filter to the Drives request.
                    drivesPage = AuthMan.ActiveAuth.GraphClient.Users[user.Id]
                        .Drives
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            //At present, Graph does not support filtering with NOT
                            //without using setting Count being equal to true.
                            //It throws a 400 exception with an HResult of -214623388.
                            //The message states "Operator 'not' is not supported
                            //because the required parameters might be missing.
                            //Try adding $count=true query parameter and
                            //ConsistencyLevel:eventual header.
                            //Refer to https://aka.ms/graph-docs/advanced-queries for
                            //more information."
                            C.QueryParameters.Count = true;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                }
                GetDrivesPages(ref drives, ref drivesPage);
            }
            catch (Exception ex)
            {
                //Ignore exceptions when the user's OneDrive is not configured.
                if ((!ex.Message.Contains("Unable to retrieve user's mysite URL.")) &&
                    (!ex.Message.Contains("User's mysite not found.")))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// A method to iterate a page collection reference and add retrieved
        /// items to an aggregation container reference.
        /// </summary>
        /// <param name="drives">A reference to the aggregation container.</param>
        /// <param name="drivesPage">A reference to the first page to iterate.</param>
        internal static void GetDrivesPages(
            ref List<Drive> drives,
            ref DriveCollectionResponse drivesPage)
        {
            while (drivesPage.Value != null)
            {
                lock (drives)
                {
                    drives.AddRange(drivesPage.Value);
                }
                if (!string.IsNullOrEmpty(drivesPage.OdataNextLink))
                {
                    drivesPage = AuthMan.ActiveAuth.GraphClient.Drives
                        .WithUrl(drivesPage.OdataNextLink)
                        .GetAsync((C) =>
                        {
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult() ;
                }
                else
                {
                    drivesPage.Value = null;
                }
            }
        }

        /// <summary>
        /// A method to get the size of a OneDrive in bytes.
        /// </summary>
        /// <param name="drive">The target Drive to calculate.</param>
        /// <param name="authStack">An optional list of Auth objects that can
        /// be passed to be used during multi-threaded operations e.g. if the
        /// environment is huge, 429 throttling can occur on the AppId being
        /// used.  By spreading the load across multiple AppIds, their auth
        /// tokens can be generated before hand and then passed to be used.</param>
        /// <param name="blackCheetah">A boolean switch to control if
        /// BlackCheetah is used in the operation.  Defaults to false.</param>
        /// <returns>A long value of the number of bytes of the Drive.</returns>
        public static long GetDriveContentSize(this Drive drive,                                                
                                               List<Auth> authStack = null,
                                               bool blackCheetah = false)
        {
            long result = 0;
            if (drive == null)
            { 
                return result; 
            }
#if BlackCheetah
            if (blackCheetah)
            {
                if (authStack == null)
                {
                    authStack = new List<Auth>();
                    authStack.Add(ActiveAuth);
                }
                List<GraphServiceClient> graphClients =
                    new List<GraphServiceClient>();
                foreach (var auth in authStack)
                {
                    graphClients.Add(auth.GraphClient);
                }
                foreach (var driveItem in BlackCheetah.Graph.GetDriveItems(
                    drive,
                    graphClients))
                {
                    result += (long)driveItem.Size;
                }
                return result;
            }
#endif
            List<DriveItem> driveItems = GetDriveItems(
                drive, 
                "", 
                null, 
                authStack, 
                blackCheetah);
            foreach (var driveItem in GetDriveItems(drive))
            {
                result += (long)driveItem.Size;
            }
            return result;
        }

        /// <summary>
        /// A method to get a specific DriveItem.
        /// </summary>
        /// <param name="driveId">The containing Drive ID.</param>
        /// <param name="itemId">The target item ID.</param>
        /// <param name="getFile">A boolean switch to indicate if the original
        /// Microsoft.Graph.Models.DriveItem is to be returned or the actual
        /// underlying file.  Defaults to false i.e. the DriveItem is returned
        /// and not the file.</param>
        /// <returns>The DriveItem if found and getFile is false, else null.
        /// If getFile is true the underlying file is returned as a byte array
        /// of if not found, null is returned.</returns>
        public static object GetDriveItem(string driveId,
                                          string itemId,
                                          bool getFile = false)
        {
            var driveItem = AuthMan.ActiveAuth.GraphClient.Drives[driveId]
                    .Items[itemId]
                    .GetAsync((C) =>
                    {
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
            if (!getFile)
            {
                if (driveItem != null)
                {
                    return driveItem;
                }
            }
            else
            {
                if (driveItem.Size == null)
                {
                    throw new Exception("DriveItem.Size is null.");
                }
                var bytes = new byte[(int)driveItem.Size];
                using (var stream = AuthMan.ActiveAuth.GraphClient.Drives[driveId]
                           .Items[itemId]
                           .Content
                           .GetAsync((C) =>
                           {
                               C.Headers.Add("ConsistencyLevel", "eventual");
                           }).GetAwaiter().GetResult())
                {
                    if (stream == null)
                    {
                        throw new Exception("DriveItem Content stream is empty.");
                    }
                    stream.Position = 0;
                    int read = stream.Read(bytes, 0, (int)driveItem.Size);
                    if (read != (int)driveItem.Size)
                    {
                        throw new Exception(
                            $"Expected bytes [{(int)driveItem.Size}]" +
                            $"Received bytes [{read}]");
                    }
                    return bytes;
                }
            }
            return null;
        }

        /// <summary>
        /// A method to retrieve a list containing all the 
        /// Microsoft.Graph.Models.DriveItem child items for the given user.
        /// </summary>
        /// <param name="userGuid">The ID of the target user to iterate.</param>
        /// <param name="filter">An OData filter string to apply.</param>
        /// <param name="select">An optional string array of additional metadata
        /// field names to retrieve.</param>
        /// <param name="authStack">An optional list of Auth objects that can
        /// be passed to be used during multi-threaded operations e.g. if the
        /// environment is huge, 429 throttling can occur on the AppId being
        /// used.  By spreading the load across multiple AppIds, their auth
        /// tokens can be generated before hand and then passed to be used.</param>
        /// <param name="blackCheetah">A boolean switch to control if
        /// BlackCheetah is used in the operation.  Defaults to false.</param>
        /// <returns>A list containing all the Microsoft.Graph.Models.DriveItem
        /// child items for the given user.</returns>
        public static List<DriveItem> GetDriveItems(
            string userGuid,
            List<Auth> authStack = null,
            string filter = null,
            string[] select = null,
            bool blackCheetah = false)
        {
            var driveItems = new List<DriveItem>();
            if (userGuid == null)
            {
                return driveItems;
            }
            if (authStack == null)
            {
                authStack = new List<Auth>();
                authStack.Add(AuthMan.ActiveAuth);
            }
            var user = AuthMan.ActiveAuth.GraphClient.Users[userGuid]
                .GetAsync().GetAwaiter().GetResult();
            var drive = AuthMan.ActiveAuth.GraphClient.Drives[user.Id]
                .GetAsync().GetAwaiter().GetResult();
#if BlackCheetah
                    if (blackCheetah)
                    {
                        if (authStack == null)
                        {
                            authStack = new List<Auth>();
                            authStack.Add(ActiveAuth);
                        }
                        List<GraphServiceClient> graphClients =
                            new List<GraphServiceClient>();
                        foreach (var auth in authStack)
                        {
                            graphClients.Add(auth.GraphClient);
                        }
                        List<BlackCheetah.DriveItem> results =
                            new List<BlackCheetah.DriveItem>();
                        results = BlackCheetah.Graph.GetDriveItems(drive, 
                                                                   graphClients, 
                                                                   filter);
                        foreach (var result in results)
                        {
                            driveItems.Add(BlackCheetah.Graph.ToGraphDriveItem(result));
                        }
                        return driveItems;
                    }
#endif
            return GetDriveItems(drive, filter, select, authStack);
        }

        /// <summary>
        /// A method to retrieve a list containing all the 
        /// Microsoft.Graph.Models.DriveItem child items for the given Drive.
        /// </summary>
        /// <param name="drive">The target Drive to iterate.</param>
        /// <param name="filter">An OData filter string to apply.</param>
        /// <param name="select">An optional string array of additional metadata
        /// field names to retrieve.</param>
        /// <param name="authStack">An optional list of Auth objects that can
        /// be passed to be used during multi-threaded operations e.g. if the
        /// environment is huge, 429 throttling can occur on the AppId being
        /// used.  By spreading the load across multiple AppIds, their auth
        /// tokens can be generated before hand and then passed to be used.</param>
        /// <param name="blackCheetah">A boolean switch to control if
        /// BlackCheetah is used in the operation.  Defaults to false.</param>
        /// <returns>A list containing all the Microsoft.Graph.Models.DriveItem
        /// child items for the given Drive.</returns>
        public static List<DriveItem> GetDriveItems(
            Drive drive,
            string filter = null,
            string[] select = null,
            List<Auth> authStack = null,
            bool blackCheetah = false)
        {
            //Create result container.
            var allDriveItems = new List<DriveItem>();
            //If we don't have a valid Drive, return empty container.
            if (drive == null)
            {
                return allDriveItems;
            }
            //Get the DriveItem of the Drive root.
            var driveItemRoot = AuthMan.ActiveAuth.GraphClient.Drives[drive.Id]
                .Root.GetAsync().GetAwaiter().GetResult();
            GetDriveItems(ref allDriveItems, 
                          driveItemRoot, 
                          filter, 
                          select, 
                          authStack, 
                          blackCheetah);
            return allDriveItems;
        }

        /// <summary>
        /// A method to retrieve a list containing all the 
        /// Microsoft.Graph.Models.DriveItem child items for the given DriveItem
        /// and filter.  Optionally, additional DriveItem metadata can be
        /// requested through the select string array parameter.  The method 
        /// will add all discovered DriveItems to the referenced DriveItems List.
        /// </summary>
        /// <param name="driveItems">A reference to the aggregation container.</param>
        /// <param name="item">The target DriveItem to iterate.</param>
        /// <param name="filter">An optional OData filter string to apply.</param>
        /// <param name="select">An optional string array of additional metadata
        /// field names to retrieve.</param>
        /// <param name="blackCheetah">A boolean switch to control if
        /// BlackCheetah is used in the operation.  Defaults to false.</param>
        /// <param name="authStack">An optional list of Auth objects that can
        /// be passed to be used during multi-threaded operations e.g. if the
        /// environment is huge, 429 throttling can occur on the AppId being
        /// used.  By spreading the load across multiple AppIds, their auth
        /// tokens can be generated before hand and then passed to be used.</param>
        internal static void GetDriveItems(ref List<DriveItem> driveItems,
                                           DriveItem item,
                                           string filter = "",
                                           string[] select = null,
                                           List<Auth> authStack = null,
                                           bool blackCheetah = false)
        {
#if BlackCheetah
            if (blackCheetah)
            {
                List<GraphServiceClient> clientStack = new List<GraphServiceClient>();
                if (authStack == null)
                {
                    authStack.Add(ActiveAuth);
                }
                foreach (var auth in authStack)
                {
                    clientStack.Add(auth.GraphClient);
                }
                var bcItems = BlackCheetah.Graph.GetDriveItems(
                    item, 
                    item.ParentReference.DriveId, 
                    filter, 
                    select, 
                    clientStack);
                if ((bcItems != null) &&
                    (bcItems.Count > 0) &&
                    (bcItems.ContainsKey("driveItems")) &&
                    (bcItems["driveItems"].Count > 0))
                {
                    lock (driveItems)
                    {
                        foreach (var bcDriveItem in bcItems["driveItems"])
                        {
                            driveItems.Add(BlackCheetah.Graph.ToGraphDriveItem(bcDriveItem));
                        }
                    }
                }
                return;
            }
#endif
            //Get the first page of drive items.
            DriveItemCollectionResponse driveItemsPage = null;
            ChildrenRequestBuilderGetQueryParameters queryParameters = null;
            if (filter != "")
            {
                queryParameters = new ChildrenRequestBuilderGetQueryParameters();
                queryParameters.Filter = filter;
            }
            if (select != null)
            {
                if (queryParameters == null)
                {
                    queryParameters = new ChildrenRequestBuilderGetQueryParameters();
                }
                queryParameters.Select = select;
            }
            if (queryParameters == null)
            {
                //There no filter, so get all items.
                driveItemsPage = AuthMan.ActiveAuth.GraphClient.Drives[item.ParentReference.DriveId]
                    .Items[item.Id]
                    .Children.GetAsync((C) =>
                    {
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
            }
            else
            {
                //Apply the specified filter to the request.
                driveItemsPage = AuthMan.ActiveAuth.GraphClient.Drives[item.ParentReference.DriveId]
                    .Items[item.Id]
                    .Children.GetAsync((C) =>
                    {
                        C.QueryParameters = queryParameters;
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
            }
            GetDriveItemsPages(ref driveItems,
                               ref driveItemsPage,
                               item.ParentReference.DriveId);
        }

        /// <summary>
        /// A method to iterate a page collection reference and add retrieved
        /// items to an aggregation container reference.
        /// </summary>
        /// <param name="driveItems">A reference to the aggregation container.</param>
        /// <param name="driveItemsPage">A reference to the first page to iterate.</param>
        /// <param name="driveId">The ID of the containing Drive.</param>
        internal static void GetDriveItemsPages(
            ref List<DriveItem> driveItems,
            ref DriveItemCollectionResponse driveItemsPage,
            string driveId)
        {
            //Was anything returned.
            while (driveItemsPage.Value != null)
            {
                //If so, lock the container and add found items.
                lock (driveItems)
                {
                    driveItems.AddRange(driveItemsPage.Value);
                }
                //Are the more pages of items?
                if (!string.IsNullOrEmpty(driveItemsPage.OdataNextLink))
                {
                    //If so, get the next page of items.
                    driveItemsPage = AuthMan.ActiveAuth.GraphClient.Drives[driveId]
                        .Items
                        .WithUrl(driveItemsPage.OdataNextLink)
                        .GetAsync((C) =>
                        {
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                }
                else
                {
                    //No more result pages remain.
                    break;
                }
            }
        }

        /// <summary>
        /// A method to get a Group by name.
        /// </summary>
        /// <param name="name">The name of the Group to get.</param>
        /// <param name="fields">An optional string array of fields to retrieve
        /// for the target Group.</param>
        /// <returns>The Group object if found, else null.</returns>
        public static Group GetGroup(string name, string[] fields = null)
        {
            var queryParameters = new GroupsRequestBuilderGetQueryParameters();
            queryParameters.Filter = $"displayName eq '{name}'";
            if (fields != null)
            {
                queryParameters.Select = fields;
            }
            var groups = AuthMan.ActiveAuth.GraphClient.Groups.GetAsync((C) =>
            {
                C.QueryParameters = queryParameters;
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
                groupsPage = AuthMan.ActiveAuth.GraphClient.Groups
                    .GetAsync().GetAwaiter().GetResult();
            }
            else
            {
                //Apply the specified filter to the Groups request.
                groupsPage = AuthMan.ActiveAuth.GraphClient.Groups
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
                    groupsPage = AuthMan.ActiveAuth.GraphClient.Groups
                        .WithUrl(groupsPage.OdataNextLink)
                        .GetAsync().GetAwaiter().GetResult();
                }
                WriteCount(groups.Count);
            }
        }

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
        /// A method to get a User by email or ID.
        /// </summary>
        /// <param name="key">The email or ID of the user to get.</param>
        /// <param name="fields">Optional string array of fields to retrieve
        /// for the target User.</param>
        /// <returns>The User object if found, else null.</returns>
        public static User GetUser(string key, string[] fields = null)
        {            
            var queryParameters = new UsersRequestBuilderGetQueryParameters();
            if (System.Guid.TryParse(key, out var id))
            {
                queryParameters.Filter = $"id eq '{key}'";
            }
            else
            {
                queryParameters.Filter = $"mail eq '{key}'";
            }
            if (fields != null)
            {
                queryParameters.Select = fields;
            }
            var users = AuthMan.ActiveAuth.GraphClient.Users.GetAsync((C) =>
            {
                C.QueryParameters = queryParameters;
                C.Headers.Add("ConsistencyLevel", "eventual");
            }).GetAwaiter().GetResult().Value;
            //There should only be 1 user.  If so, return it.
            if ((users != null) && (users.Count == 1))
            {
                return users[0];
            }
            return null;
        }

        /// <summary>
        /// A method to return a list of all users using the given filter.
        /// </summary>
        /// <param name="filter">The OData filter to apply.</param>
        /// <returns>A list of Microsoft.Graph.Models.User for the given
        /// filter or all users if the filter is blank.</returns>
        public static List<User> GetUsers(string filter = "")
        {
            var users = new List<User>();
            GetUsers(ref users, filter);
            return users;
        }

        /// <summary>
        /// A method to populate a given reference list of Users using the
        /// given filter.
        /// </summary>
        /// <param name="users">The aggregation container to use.</param>
        /// <param name="filter">The OData filter to apply to the request.</param>
        internal static void GetUsers(ref List<User> users, 
                                      string filter = "")
        {
            //Get the first page of Users.
            UserCollectionResponse usersPage = null;
            if (filter == "")
            {
                //There's no filter, so get all Users.
                usersPage = AuthMan.ActiveAuth.GraphClient.Users
                    .GetAsync((C) =>
                    {
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
            }
            else
            {
                //Apply the specified filter to the Users request.
                usersPage = AuthMan.ActiveAuth.GraphClient.Users
                    .GetAsync((C) =>
                    {
                        C.QueryParameters.Filter = filter;
                        //At present, Graph does not support filtering with NOT
                        //without using setting Count = true.
                        //It throws a 400 exception with an HResult of -2146233088.
                        //The mssage states "Operator 'not' is not supported
                        //because the required parameters might be missing.
                        //Try adding $count=true query parameter and 
                        //ConsistencyLevel:eventual header.
                        //Refer to https://aka.ms/graph-docs/advanced-queries
                        //for more information.
                        C.QueryParameters.Count = true;
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
            }
            GetUsersPages(ref users, ref usersPage);
        }

        /// <summary>
        /// A method to iterate all the result pages and aggregate the values
        /// in the given reference list of Users.
        /// </summary>
        /// <param name="users">The aggregation container to use.</param>
        /// <param name="usersPage">The first page of the response.</param>
        internal static void GetUsersPages(
            ref List<User> users,
            ref UserCollectionResponse usersPage)
        {
            while (usersPage.Value != null)
            {
                foreach (var user in usersPage.Value)
                {
                    lock (users)
                    {
                        users.Add(user);
                    }
                }
                if (!string.IsNullOrEmpty(usersPage.OdataNextLink))
                {
                    usersPage = AuthMan.ActiveAuth.GraphClient.Users
                        .WithUrl(usersPage.OdataNextLink)
                        .GetAsync((C) =>
                        {
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                }
                else
                {
                    usersPage.Value = null;
                }
            }
        }

        /// <summary>
        /// A method to duplicate an existing M365 group.
        /// </summary>
        /// <param name="groupName">The name of the group to duplicate.</param>
        /// <param name="newName">The name to use for the new group.</param>
        /// <param name="returnType">The type of data to return after
        /// duplicating the Group i.e. the GUID ID of the Group or the actual
        /// Group itself.</param>
        /// <returns>The GUID string ID of the Group if returnType = Id and
        /// the Group object if returnType = Group.</returns>
        public static object DuplicateGroup(
            string groupName, 
            string newName,
            DuplicateGroupReturnType returnType = 
                DuplicateGroupReturnType.Id)
        {
            //Find the source group.
            Group group = GetGroup(groupName);
            //If the group is not found, return null.
            if (group == null)
            {
                return null;
            }
            Dictionary<string, object> sourceFields = new Dictionary<string, object>();
            Group newGroupInfo = new Group();
            System.Reflection.FieldInfo fieldInfo;
            object fieldValue;
            foreach (System.Reflection.FieldInfo sourceFieldInfo in typeof(Group).GetFields())
            {
                fieldInfo = typeof(Group).GetField(sourceFieldInfo.Name);
                fieldValue = fieldInfo.GetValue(group);
                fieldInfo.SetValue(newGroupInfo, fieldValue);
            }
            //Create the new group.
            var newGroup = AuthMan.ActiveAuth.GraphClient.Groups
                .PostAsync(newGroupInfo)
                .GetAwaiter().GetResult();
            //Determine what to return.
            if (returnType == DuplicateGroupReturnType.Id)
            {
                //Return the ID of the new group.
                return newGroup.Id;
            }
            return newGroup;
        }

        /// <summary>
        /// Create a new Group given a set of fields and values.  If values
        /// in the dictionary are invalid for whatever reason, the exception
        /// is allowed to bubble up.
        /// </summary>
        /// <param name="groupFields">The dictionary containing the fields
        /// and values to use for Group creation.</param>
        /// <returns>The Group after creation.</returns>
        public static Group NewGroup(
            Dictionary<string, object> groupFields)
        {
            var newGroupInfo = new Group();
            System.Reflection.FieldInfo fieldInfo;
            foreach (KeyValuePair<string, object> kvp in groupFields)
            {
                fieldInfo = typeof(Group).GetField(kvp.Key);
                fieldInfo.SetValue(newGroupInfo, kvp.Value);
            }
            //Create the new group.
            var newGroup = AuthMan.ActiveAuth.GraphClient.Groups
                .PostAsync(newGroupInfo)
                .GetAwaiter().GetResult();
            return newGroup;
        }

        /// <summary>
        /// Updates a given Group given its name and a set of fields and 
        /// values.  If values in the dictionary are invalid for whatever
        /// reason, the exception is allowed to bubble up.
        /// </summary>
        /// <param name="groupName">The name of the Group to update.</param>
        /// <param name="groupFields">The dictionary containing the fields
        /// and values to use for Group update.</param>
        /// <param name="groupUpdateType">The type of value being passed in
        /// the groupName parameter i.e. DisplayName or Guid.  Default is
        /// DisplayName.</param>
        /// <returns>The Group after update.</returns>
        public static Group UpdateGroup(
            string groupName,
            Dictionary<string, object> groupFields,
            GroupUpdateType groupUpdateType = 
                GroupUpdateType.DisplayName)
        {
            string id = "";
            if (groupUpdateType == GroupUpdateType.DisplayName)
            {
                var groupToUpdate = GetGroup(groupName);
                id = groupToUpdate.Id;
            }
            var updatedGroupInfo = ConstructGroupObject(groupFields);
            //Create the new group.
            var updatedGroup = AuthMan.ActiveAuth.GraphClient.Groups[id]
                .PatchAsync(updatedGroupInfo)
                .GetAwaiter().GetResult();
            return updatedGroup;
        }

        /// <summary>
        /// Constructs a new in memory Group object given a set of fields 
        /// and values.  If values in the dictionary are invalid for 
        /// whatever reason, the exception is allowed to bubble up.
        /// </summary>
        /// <param name="groupFields">The dictionary containing the fields
        /// and values to use for Group creation.</param>
        /// <returns>The Group object after construction.</returns>
        public static Group ConstructGroupObject(
            Dictionary<string, object> groupFields)
        {
            var newGroupInfo = new Group();
            System.Reflection.FieldInfo fieldInfo;
            foreach (KeyValuePair<string, object> kvp in groupFields)
            {
                fieldInfo = typeof(Group).GetField(kvp.Key);
                fieldInfo.SetValue(newGroupInfo, kvp.Value);
            }
            return newGroupInfo;
        }
        /// <summary>
        /// Updates a given Group given its name and a the ID of the target
        /// sensitivity label.  Exceptions are allowed to bubble up.
        /// </summary>
        /// <param name="groupName">The name of the Group to update.</param>
        /// <param name="sensitivityGuid">The GUID string of the target
        /// sensitivity label's ID value.</param>
        /// <param name="groupUpdateType">The type of value being passed in
        /// the groupName parameter i.e. DisplayName or Guid.  Default is
        /// DisplayName.</param>
        /// <returns>The Group after update.</returns>
        public static Group SetGroupSensitivity(
            string groupName,
            string sensitivityGuid,
            GroupUpdateType groupUpdateType =
                GroupUpdateType.DisplayName)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("AssignedLabels", new List<AssignedLabel>()
            {
                new AssignedLabel()
                {
                    LabelId = sensitivityGuid
                }
            });
            return UpdateGroup(groupName, dic, groupUpdateType);
        }

        /// <summary>
        /// A method to retrieve a list of ID values (GUID) for all members
        /// of a specified Group.
        /// </summary>
        /// <param name="groupId">The ID (GUID) of the target group.</param>
        /// <param name="userInfoType">The type of user info to return
        /// i.e. "id", "mail" or "userProfileName".  Default is "id".</param>
        /// <returns>A list of strings representing the IDs of member 
        /// users.</returns>
        public static List<string> GetMembers(
            string groupId, 
            UserInfoType userInfoType = UserInfoType.id)
        {
            //Create the aggregation container.
            List<string> members = new List<string>();
            List<User> users = new List<User>();
            //Get the first page of members.
            var usersPage = AuthMan.ActiveAuth.GraphClient.Groups[groupId]
                .Members.GraphUser.GetAsync(C =>
                {
                    C.QueryParameters.Select = new string[] 
                    { 
                        userInfoType.ToString() 
                    };
                }).GetAwaiter().GetResult();
            if (usersPage.Value.Count == 0)
            {
                return members;
            }
            var pageIterator = PageIterator<User, UserCollectionResponse>
                .CreatePageIterator(
                    AuthMan.ActiveAuth.GraphClient,
                    usersPage,
                    (user) =>
                    {
                        switch (userInfoType)
                        {
                            case UserInfoType.id:
                                if (user.Id != null &&
                                    user.Id.Trim().Length > 0)
                                {
                                    members.Add(user.Id.Trim());
                                }
                                break;
                            case UserInfoType.mail:
                                if (user.Mail != null &&
                                    user.Mail.Trim().Length > 0)
                                {
                                    members.Add(user.Mail.Trim());
                                }
                                break;
                            case UserInfoType.userProfileName:
                                if (user.UserPrincipalName != null &&
                                    user.UserPrincipalName.Trim().Length > 0)
                                {
                                    members.Add(user.UserPrincipalName.Trim());
                                }
                                break;
                        }
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
        /// <param name="userInfoType">The type of user info to return
        /// i.e. "id", "mail" or "userProfileName".  Default is "id".</param>
        /// <returns>A list of objects representing the information about the
        /// Owners users depending on the userInfoType specified i.e.
        /// - For UserInfoType.id a list of strings containing user ID
        /// values is returned.
        /// - For UserInfoType.mail a list of strings containing user
        /// email address values is returned.
        /// - For UserInfoType.userPrincipalName a list of strings 
        /// containing user UPN values is returned.
        /// - For UserInfoType.all a list of Microsoft.Graph.Models.User
        /// objects is returned.</returns>
        public static List<object> GetOwners(
            string groupId,
            UserInfoType userInfoType = UserInfoType.id)
        {
            //Create the aggregation container.
            List<object> owners = new List<object>();
            List<User> users = new List<User>();
            //Get the first page of owners.
            var usersPage = AuthMan.ActiveAuth.GraphClient.Groups[groupId]
                .Owners.GraphUser.GetAsync(C =>
                {
                    C.QueryParameters.Select = new string[]
                    {
                        (userInfoType == UserInfoType.All ? "" :
                        userInfoType.ToString())
                    };
                }).GetAwaiter().GetResult();
            if (usersPage.Value.Count == 0)
            {
                return owners;
            }
            var pageIterator = PageIterator<User, UserCollectionResponse>
                .CreatePageIterator(
                    AuthMan.ActiveAuth.GraphClient,
                    usersPage,
                    (user) =>
                    {
                        switch (userInfoType)
                        {
                            case UserInfoType.id:
                                if (user.Id != null &&
                                    user.Id.Trim().Length > 0)
                                {
                                    owners.Add(user.Id.Trim());
                                }
                                break;
                            case UserInfoType.mail:
                                if (user.Mail != null &&
                                    user.Mail.Trim().Length > 0)
                                {
                                    owners.Add(user.Mail.Trim());
                                }
                                break;
                            case UserInfoType.userProfileName:
                                if (user.UserPrincipalName != null &&
                                    user.UserPrincipalName.Trim().Length > 0)
                                {
                                    owners.Add(user.UserPrincipalName.Trim());
                                }
                                break;
                            case UserInfoType.All:
                                owners.Add(user);
                                break;
                        }
                        return true;
                    });
            pageIterator.IterateAsync().GetAwaiter().GetResult();
            Inf(owners.Count.ToString());
            return owners;
        }

        /// <summary>
        /// A method to retrieve a list of ID values (GUID) for all owners
        /// of a specified Group.
        /// </summary>
        /// <param name="groupId">The ID (GUID) of the target group.</param>
        /// <param name="userInfoType">The type of user info to return
        /// i.e. "id", "mail" or "userProfileName".  Default is "id".</param>
        /// <param name="groupUserMembershipType">The type of Group membership
        /// users to retrieve i.e. Owners of the Group, Members of the Group 
        /// or both.</param>
        /// <returns>A list of objects representing the information about the
        /// Owners users depending on the userInfoType specified i.e.
        /// - For UserInfoType.id a list of strings containing user ID
        /// values is returned.
        /// - For UserInfoType.mail a list of strings containing user
        /// email address values is returned.
        /// - For UserInfoType.userPrincipalName a list of strings 
        /// containing user UPN values is returned.
        /// - For UserInfoType.all a list of Microsoft.Graph.Models.User
        /// objects is returned.</returns>
        public static List<object> GetGroupUsers(
            string groupId,
            UserInfoType userInfoType = UserInfoType.id,
            GroupUserMembershipType groupUserMembershipType
                = GroupUserMembershipType.All)
        {
            //Create the aggregation container.
            List<object> users = new List<object>();
            //Get the first page of users.
            UserCollectionResponse usersPage = 
                //If groupUserMembershipType is All or Owners, get the Owners first page.
                ((groupUserMembershipType == GroupUserMembershipType.All ||
                  groupUserMembershipType == GroupUserMembershipType.Owners) ?
                    AuthMan.ActiveAuth.GraphClient.Groups[groupId]
                        .Owners.GraphUser.GetAsync(C =>
                        {
                            C.QueryParameters.Select = new string[]
                            {
                                //Check if the userInfoType is All and if so,
                                //do NOT specify any .Select parameters.
                                (userInfoType == UserInfoType.All ? "" :
                                    userInfoType.ToString())
                            };
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }) :
                    //If its not All or Owners, its Members so get the Members first page.
                    AuthMan.ActiveAuth.GraphClient.Groups[groupId]
                        .Members.GraphUser.GetAsync(C =>
                        {
                            C.QueryParameters.Select = new string[]
                            {
                                (userInfoType == UserInfoType.All ? "" :
                                    userInfoType.ToString())
                            };
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        })
                ).GetAwaiter().GetResult();
            //If groupUserMembershipType is not All and there are no items, return list.
            //If groupUserMembershipType is All, and there are no items in the Owners
            //page then we still need to get the Members.
            if ((groupUserMembershipType != GroupUserMembershipType.All) &&
                (usersPage.Value.Count == 0))
            {
                return users;
            }
            //There are items so create a PageIterator.
            var pageIterator = PageIterator<User, UserCollectionResponse>
                .CreatePageIterator(
                    AuthMan.ActiveAuth.GraphClient,
                    usersPage,
                    (user) =>
                    {
                        //Aggregate the list based on the userInfoType requested.
                        switch (userInfoType)
                        {
                            //Get User ID GUID values.
                            case UserInfoType.id:
                                if (user.Id != null &&
                                    user.Id.Trim().Length > 0)
                                {
                                    users.Add(user.Id.Trim());
                                }
                                break;
                            //Get User email address values.
                            case UserInfoType.mail:
                                if (user.Mail != null &&
                                    user.Mail.Trim().Length > 0)
                                {
                                    users.Add(user.Mail.Trim());
                                }
                                break;
                            //Get User UPN values, usually the same as email
                            //but can be different.
                            case UserInfoType.userProfileName:
                                if (user.UserPrincipalName != null &&
                                    user.UserPrincipalName.Trim().Length > 0)
                                {
                                    users.Add(user.UserPrincipalName.Trim());
                                }
                                break;
                            //Get the entire User object.
                            case UserInfoType.All:
                                users.Add(user);
                                break;
                        }
                        return true;
                    });
            pageIterator.IterateAsync().GetAwaiter().GetResult();
            //Before returning our list check if All was requested.
            if (groupUserMembershipType == GroupUserMembershipType.All)
            {
                //If All was requested, the list contains Owners info.
                //Now get the Member info as well.
                usersPage = AuthMan.ActiveAuth.GraphClient.Groups[groupId]
                    .Members.GraphUser.GetAsync(C =>
                    {
                        C.QueryParameters.Select = new string[]
                        {
                            //Check if the userInfoType is All and if so,
                            //do NOT specify any .Select parameters.
                            (userInfoType == UserInfoType.All ? "" :
                                userInfoType.ToString())
                        };
                        C.Headers.Add("ConsistencyLevel", "eventual");
                    }).GetAwaiter().GetResult();
                //If there are no Members, return the aggregated list.
                if (usersPage.Value.Count == 0)
                {
                    return users;
                }
                //If however there are Members, assign the first page to the
                //PageIterator for processing.
                pageIterator = PageIterator<User, UserCollectionResponse>
                    .CreatePageIterator(
                        AuthMan.ActiveAuth.GraphClient,
                        usersPage,
                        (user) =>
                        {
                            //Aggregate the list based on the userInfoType requested.
                            switch (userInfoType)
                            {
                                //Get User ID GUID values.
                                case UserInfoType.id:
                                    if (user.Id != null &&
                                        user.Id.Trim().Length > 0)
                                    {
                                        users.Add(user.Id.Trim());
                                    }
                                    break;
                                //Get User email address values.
                                case UserInfoType.mail:
                                    if (user.Mail != null &&
                                        user.Mail.Trim().Length > 0)
                                    {
                                        users.Add(user.Mail.Trim());
                                    }
                                    break;
                                //Get User UPN values, usually the same as email
                                //but can be different.
                                case UserInfoType.userProfileName:
                                    if (user.UserPrincipalName != null &&
                                        user.UserPrincipalName.Trim().Length > 0)
                                    {
                                        users.Add(user.UserPrincipalName.Trim());
                                    }
                                    break;
                                //Get the entire User object.
                                case UserInfoType.All:
                                    users.Add(user);
                                    break;
                            }
                            return true;
                        });
                pageIterator.IterateAsync().GetAwaiter().GetResult();
            }
            //Write out the user count for debug.
            Inf(users.Count.ToString());
            //Return the aggregated list.
            return users;
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
            return (AuthMan.ActiveAuth.GraphClient.Sites[$"root:{sitePath}"]
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
            var sitesPage = AuthMan.ActiveAuth.GraphClient.Sites
                .GetAsync().GetAwaiter().GetResult();
            var pageIterator = PageIterator<Site, SiteCollectionResponse>
                .CreatePageIterator(
                    AuthMan.ActiveAuth.GraphClient,
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
            return AuthMan.ActiveAuth.GraphClient.Sites[GetSiteId(sitePath)]
                .Lists.GetAsync((C) =>
                {
                    C.QueryParameters.Filter = $"displayName eq '{listName}'";
                    C.Headers.Add("ConsistencyLevel", "eventual");
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
                    var listItem = AuthMan.ActiveAuth.GraphClient.Sites[GetSiteId(sitePath)]
                        .Lists[GetListId(listName, sitePath)]
                        .Items[id]
                        .GetAsync((C) =>
                        {
                            C.QueryParameters.Expand = new string[] { "fields" };
                            C.Headers.Add("ConsistencyLevel", "eventual");
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
                        AuthMan.ActiveAuth.GraphClient.Sites[GetSiteId(sitePath)]
                        .Lists[GetListId(listName, sitePath)]
                        .Items.GetAsync((C) =>
                        {
                            if (filter != null)
                            {
                                C.QueryParameters.Filter = filter;
                                C.Headers.Add("ConsistencyLevel", "eventual");
                            }
                            C.QueryParameters.Expand = new string[] { "fields" };
                        }).GetAwaiter().GetResult();
                    if (listItemCollectionResponse == null)
                    {
                        return listItems;
                    }
                    var pageIterator = PageIterator<ListItem, ListItemCollectionResponse>
                        .CreatePageIterator(
                            AuthMan.ActiveAuth.GraphClient,
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
            Inf(GetEnv("NewItemsStatusFilter"));
            Inf($"Scope [{AuthMan.ActiveAuth.Scopes[0]}]");
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
            filter = GetEnv("NewItemsStatusFilter").ToLower();
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
        /// Gets the Team associated with the current Group.
        /// </summary>
        /// <param name="group">The Group for which to get the Team.</param>
        /// <returns>The Team if it exist, else null.</returns>
        public static Team GetTeam(
            this Microsoft.Graph.Models.Group group)
        {
            if (group.HasTeam())
            {
                return GetTeam(group.Id);
            }
            return null;
        }

        /// <summary>
        /// Gets the Team associated with the current Group.
        /// </summary>
        /// <param name="groupId">The ID of the Group for which to get 
        /// the Team.</param>
        /// <returns>The Team if it exist, else null.</returns>
        public static Team GetTeam(
            string groupId)
        {
            Team team = null;
            try
            {
                team = AuthMan.ActiveAuth.GraphClient.Groups[groupId].Team
                    .GetAsync().GetAwaiter().GetResult();
                return team;
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Checks if the Group has an associated Team.
        /// </summary>
        /// <param name="group">The Group being checked.</param>
        /// <returns>True if the Group has an associated Team, else false.</returns>
        public static bool HasTeam(
            this Group group)
        {
            var groups = GetGroups("((id eq '" + group.Id + 
                "') and (resourceProvisioningOptions/Any(x:x eq 'Team')))");
            return groups.Count > 0;
        }

        /// <summary>
        /// Add a Team to the current group.
        /// </summary>
        /// <param name="group">The target Group for Team addition.</param>
        /// <param name="newTeamInfo">Optional Team options to use when 
        /// creating the new Team.</param>
        /// <returns>The Team after creation.  If the Team already exist, the
        /// existing Team is returned.  If creation fails it'll retry for 30
        /// seconds before returning null.</returns>
        public static Team Teamify(
            this Group group,
            Team newTeamInfo = null)
        {
            return Teamify(group.Id, newTeamInfo);
        }

        /// <summary>
        /// Add a Team to the current group.
        /// </summary>
        /// <param name="groupId">The ID of the target Group for Team 
        /// addition.</param>
        /// <param name="newTeamInfo">Optional Team options to use when 
        /// creating the new Team.</param>
        /// <returns>The Team after creation.  If the Team already exist, the
        /// existing Team is returned.  If creation fails it'll retry for 30
        /// seconds before returning null.</returns>
        public static Team Teamify(
            string groupId, 
            Team newTeamInfo = null)
        {
            Team team = null;
            bool done = false;
            int count = 0;
            while (!done)
            {
                count++;
                try
                {
                    team = GetTeam(groupId);
                    if (team != null)
                    {
                        done = true;
                        return team;
                    }
                    else
                    {
                        if (newTeamInfo == null)
                        {
                            newTeamInfo = new Team
                            {
                                MemberSettings = new TeamMemberSettings
                                {
                                    AllowCreatePrivateChannels = true,
                                    AllowCreateUpdateChannels = true
                                },
                                MessagingSettings = new TeamMessagingSettings
                                {
                                    AllowUserDeleteMessages = true,
                                    AllowUserEditMessages = true
                                },
                                FunSettings = new TeamFunSettings
                                {
                                    AllowGiphy = true,
                                    GiphyContentRating = GiphyRatingType.Strict
                                }
                            };
                        }
                        team = AuthMan.ActiveAuth.GraphClient.Groups[groupId].Team
                            .PutAsync(newTeamInfo)
                            .GetAwaiter().GetResult();
                        if (team != null)
                        {
                            done = true;
                            return team;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Azure async processes may not yet have created the Team.
                    //Wait for a second before retrying.
                    Thread.Sleep(1000);
                }
                if (count > 30)
                {
                    done = true;
                    return null;
                }
            }
            return team;
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
            AuthMan.ActiveAuth.GraphClient.Sites[siteGuid]
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
            AuthMan.ActiveAuth.GraphClient.Sites[siteGuid]
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
            AuthMan.ActiveAuth.GraphClient.Sites[siteGuid]
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
        /// A method to force Azure to provision the back end SharePoint site
        /// for a given Group or Team.  When new Teams are created via code,
        /// Azure will not spin up the SharePoint site until a user clicks on
        /// the Files link.  Problem is that when the user does that they get
        /// an error message telling them to try again later.  This is a
        /// terrible user experience so the .SharePointify() method can be
        /// used to force the process to take place so the user never sees
        /// that error.
        /// </summary>
        /// <param name="groupId">The GUID ID of the Group associated with the
        /// Team.</param>
        /// <returns>A reference to the Drive object if successful.  The process
        /// will retry every second for 30 seconds afterwhich it will timeout
        /// and simply return null.  This seldom happens.</returns>
        public static Drive SharePointify(
            string groupId)
        {
            Drive drive = null;
            bool done = false;
            int count = 0;
            while (!done)
            {
                count++;
                try
                {
                    drive = AuthMan.ActiveAuth.GraphClient.Groups[groupId].Drive
                        .GetAsync().GetAwaiter().GetResult();
                    if (drive != null)
                    {
                        done = true;
                        return drive;
                    }
                }
                catch (Exception ex)
                {
                    //Azure async processes may not yet have created the Group.
                    //Wait for a second before retrying.
                    Thread.Sleep(1000);
                }
                if (count > 30)
                {
                    done = true;
                }
            }
            return drive;
        }

        /// <summary>
        /// A method to force Azure to provision the back end SharePoint site
        /// for a given Group or Team.  When new Teams are created via code,
        /// Azure will not spin up the SharePoint site until a user clicks on
        /// the Files link.  Problem is that when the user does that they get
        /// an error message telling them to try again later.  This is a
        /// terrible user experience so the .SharePointify() method can be
        /// used to force the process to take place so the user never sees
        /// that error.
        /// </summary>
        /// <param name="group">The Group associated with the Team.</param>
        /// <returns>A reference to the Drive object if successful.  The process
        /// will retry every second for 30 seconds afterwhich it will timeout
        /// and simply return null.  This seldom happens.</returns>
        public static Drive SharePointify(
            this Microsoft.Graph.Models.Group group)
        {
            return SharePointify(group.Id);
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

        /// <summary>
        /// A method to get an EnvironmentVariable value.
        /// </summary>
        /// <param name="key">The target variable name.</param>
        /// <returns>The value of the EnvironmentVariable or.</returns>
        internal static string GetEnv(string key)
        {
            if (Environment.GetEnvironmentVariable(key) != null)
            {
                return Environment.GetEnvironmentVariable(key);
            }
            return GetSetting(key);
        }

        /// <summary>
        /// An method to allow settings values to be retrieved.
        /// </summary>
        /// <param name="key">The name of the setting value to retrieve.</param>
        /// <returns>If the requested setting exist in the config, its value is
        /// returned else a blank string is returned.</returns>
        internal static string GetSetting(string key)
        {
            if (AuthMan.ActiveAuth.TenantCfg.Settings.ContainsKey(key))
            {
                return AuthMan.ActiveAuth.TenantCfg.Settings[key];
            }
            return "";
        }
    }
}
