/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph;
using Microsoft.Graph.Models;
using static Microsoft.Graph.Drives.Item.Items.Item.Children.ChildrenRequestBuilder;
using static Microsoft.Graph.Users.Item.Drives.DrivesRequestBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions.Identity;
using static Extensions.Constants;
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
        #region GroupOwnersMembers
        /// <summary>
        /// A method to add or remove a given User as an Owner or Member of 
        /// the current Group.
        /// </summary>
        /// <param name="group">The current target Group.</param>
        /// <param name="user">The target User to add or remove as Owner or 
        /// Member.</param>
        /// <param name="membershipType">The type of membership to target i.e.
        /// Owner or Member.  Defaults to Owners.</param>
        /// <param name="add">A boolean parameter to control if the target user
        /// is added or removed.  Defaults to true i.e. add.</param>
        /// <param name="client">Optional GraphServiceClient object to use
        /// during processing.  If none is supplied, the ActiveAuth's version
        /// is used.  Defaults to null i.e. use the ActiveAuth version.</param>
        /// <returns>True if the User was added or removed, else false.</returns>
        public static bool SetGroupUser(
            this Group group,
            User user,
            UserMembershipType membershipType = UserMembershipType.Owners,
            bool add = true,
            GraphServiceClient client = null)
        { 
            if (client == null)
            {
                client = ActiveAuth.GraphClient;
            }
            var requestBody = new ReferenceCreate
            {
                OdataId = $"https://graph.microsoft" +
                    $"{ActiveAuth.TenantCfg.AuthorityDomain}/v1.0" +
                    $"/users/{user.Id}"
            };
            try
            {
                if (membershipType == UserMembershipType.Owners)
                {
                    if (add)
                    {
                        client.Groups[group.Id].Owners.Ref.PostAsync(requestBody)
                            .GetAwaiter().GetResult();
                        return true;
                    }
                    else
                    {
                        client.Groups[group.Id].Owners[user.Id].Ref.DeleteAsync()
                            .GetAwaiter().GetResult();
                        return true;
                    }
                }
                if (membershipType == UserMembershipType.Members)
                {
                    if (add)
                    {
                        client.Groups[group.Id].Members.Ref.PostAsync(requestBody)
                            .GetAwaiter().GetResult();
                        return true;
                    }
                    else
                    {
                        client.Groups[group.Id].Members[user.Id].Ref.DeleteAsync()
                            .GetAwaiter().GetResult();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("One or more added object references " +
                    "already exist for the following modified properties"))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// A method to add a given User as an Owner of the current Group.
        /// </summary>
        /// <param name="group">The current target Group.</param>
        /// <param name="user">The target User to add as Owner.</param>
        /// <param name="client">Optional GraphServiceClient object to use
        /// during processing.  If none is supplied, the ActiveAuth's version
        /// is used.  Defaults to null i.e. use the ActiveAuth version.</param>
        /// <returns>True if the User was added, else false.</returns>
        public static bool AddGroupOwner(this Group group,
                                         User user,
                                         GraphServiceClient client = null)
        {
            return SetGroupUser(group, user, UserMembershipType.Owners, true, client);
        }

        /// <summary>
        /// A method to remove a given User as an Owner of the current Group.
        /// </summary>
        /// <param name="group">The current target Group.</param>
        /// <param name="user">The target User to add as Owner.</param>
        /// <param name="client">Optional GraphServiceClient object to use
        /// during processing.  If none is supplied, the ActiveAuth's version
        /// is used.  Defaults to null i.e. use the ActiveAuth version.</param>
        /// <returns>True if the User was removed, else false.</returns>
        public static bool RemoveGroupOwner(this Group group,
                                            User user,
                                            GraphServiceClient client = null)
        {
            return SetGroupUser(group, user, UserMembershipType.Owners, false, client);
        }

        /// <summary>
        /// A method to add a given User as a Member of the current Group.
        /// </summary>
        /// <param name="group">The current target Group.</param>
        /// <param name="user">The target User to add as Member.</param>
        /// <param name="client">Optional GraphServiceClient object to use
        /// during processing.  If none is supplied, the ActiveAuth's version
        /// is used.  Defaults to null i.e. use the ActiveAuth version.</param>
        /// <returns>True if the User was added, else false.</returns>
        public static bool AddGroupMember(this Group group,
                                          User user,
                                          GraphServiceClient client = null)
        {
            return SetGroupUser(group, user, UserMembershipType.Members, true, client);
        }

        /// <summary>
        /// A method to remove a given User as a Member of the current Group.
        /// </summary>
        /// <param name="group">The current target Group.</param>
        /// <param name="user">The target User to add as Member.</param>
        /// <param name="client">Optional GraphServiceClient object to use
        /// during processing.  If none is supplied, the ActiveAuth's version
        /// is used.  Defaults to null i.e. use the ActiveAuth version.</param>
        /// <returns>True if the User was removed, else false.</returns>
        public static bool RemoveGroupMember(this Group group,
                                             User user,
                                             GraphServiceClient client = null)
        {
            return SetGroupUser(group, user, UserMembershipType.Members, false, client);
        }
        #endregion GroupOwnersMembers

        /// <summary>
        /// A method to get the OneDrive of the target User.
        /// </summary>
        /// <param name="user">The target User.</param>
        /// <returns>The Drive object of the target User.</returns>
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

        public static List<Drive> GetDrives(this Group group)
        {
            return ConvertObjToDrive((List<object>)Get(
                GraphObjectType.Drive,
                GraphDriveParentType.Group,
                group)
                .GetAwaiter().GetResult());
        }

        public static List<Drive> GetDrives(this Site site)
        {
            return ConvertObjToDrive((List<object>)Get(
                GraphObjectType.Drive,
                GraphDriveParentType.Site,
                site).GetAwaiter().GetResult());
        }

        public static List<Drive> GetDrives(this User user)
        {
            return ConvertObjToDrive((List<object>)Get(
                GraphObjectType.Drive,
                GraphDriveParentType.User,
                user).GetAwaiter().GetResult());
        }

        internal static List<Drive> ConvertObjToDrive(List<object> objs)
        {
            List<Drive> drives = new List<Drive>();
            foreach (object obj in objs)
            {
                drives.Add((Drive)obj);
            }
            return drives;
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
            var queryParameters = new Microsoft.Graph.Groups.GroupsRequestBuilder
                .GroupsRequestBuilderGetQueryParameters();
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
                groups.AddRange(groupsPage.Value);                
                if (!string.IsNullOrEmpty(groupsPage.OdataNextLink))
                {
                    groupsPage = AuthMan.ActiveAuth.GraphClient.Groups
                        .WithUrl(groupsPage.OdataNextLink)
                        .GetAsync().GetAwaiter().GetResult();
                }
                else
                {
                    break;
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
            List<string> starterStrings = new List<string>()
            {
                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o",
                "p","q","r","s","t","u","v","w","x","y","z","1","2","3","4",
                "5","6","7","8","9","0"
            };
            int divisor = (int)((double)(Environment.ProcessorCount / 2.25));
            if (divisor > 1)
            {
                List<string> starters;
                while (starterStrings.Count > 0)
                {
                    starters = starterStrings.TakeAndRemove(divisor);
                    Parallel.ForEach(starters, starter =>
                    {
                        GetGroups(ref groups, $"startswith(displayName, '{starter}')");
                    });
                    Inf($"[{groups.Count}] Groups, " +
                        $"[{starterStrings.Count}] starterStrings remain.");
                }
            }
            else
            {
                foreach (var starter in starterStrings)
                {
                    GetGroups(ref groups,
                              $"startswith(displayName, '{starter}')");
                    Inf($"[{groups.Count}] Groups");
                }
            }
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
            var queryParameters = new Microsoft.Graph.Users.UsersRequestBuilder
                .UsersRequestBuilderGetQueryParameters();
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
            var userCollectionResponse = ActiveAuth.GraphClient.Users
                .GetAsync((C) =>
                {
                    C.Headers.Add("ConsistencyLevel", "eventual");
                }).GetAwaiter().GetResult();
            if (userCollectionResponse != null)
            {
                var pageIterator = PageIterator<User, UserCollectionResponse>
                    .CreatePageIterator(
                        ActiveAuth.GraphClient,
                        userCollectionResponse,
                        (user) =>
                        {
                            users.Add(user);
                            if (users.Count % 5000 == 0)
                            {
                                Inf($"[{DateTime.Now}] [{users.Count}] users and counting...");
                            }
                            return true;
                        });
                pageIterator.IterateAsync().GetAwaiter().GetResult();
            }
            //GetUsers(ref users, filter);
            return users;
        }

        /// <summary>
        /// A method to populate a given reference list of Users using the
        /// given filter.
        /// </summary>
        /// <param name="users">The aggregation container to use.</param>
        /// <param name="filter">The OData filter to apply to the request.</param>
        [Obsolete("Use Graph's new built in PageIterator instead.")]
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
        [Obsolete("Use Graph's new built in PageIterator instead.")]
        internal static void GetUsersPages(
            ref List<User> users,
            ref UserCollectionResponse usersPage)
        {
            while (usersPage.Value != null)
            {
                lock (users)
                {
                    users.AddRange(usersPage.Value);
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
        /// A method to get all users in the Site's Owner group.
        /// </summary>
        /// <param name="siteUrl">The URL of the target SharePoint site.</param>
        /// <param name="userInfoType">The type of user information to return
        /// i.e. Ids, Email Adresses, User Principal Name or the actual
        /// Microsoft.SharePoint.Client.User objects.</param>
        /// <returns>A list of objects that is either a list of strings
        /// containing the Ids or the Email Adresses or the User Principal 
        /// Names (depending on the userInfoType specified) or a list of
        /// Microsoft.SharePoint.Client.User objects.</returns>
        public static List<object> GetSiteOwners(
            string siteUrl,
            UserInfoType userInfoType = UserInfoType.id)
        {
            return GetSiteUsers(siteUrl, 
                                userInfoType, 
                                UserMembershipType.Owners);
        }

        /// <summary>
        /// A method to get all users in the Site's Members group.
        /// </summary>
        /// <param name="siteUrl">The URL of the target SharePoint site.</param>
        /// <param name="userInfoType">The type of user information to return
        /// i.e. Ids, Email Adresses, User Principal Name or the actual
        /// Microsoft.SharePoint.Client.User objects.</param>
        /// <returns>A list of objects that is either a list of strings
        /// containing the Ids or the Email Adresses or the User Principal 
        /// Names (depending on the userInfoType specified) or a list of
        /// Microsoft.SharePoint.Client.User objects.</returns>
        public static List<object> GetSiteMembers(
            string siteUrl,
            UserInfoType userInfoType = UserInfoType.id)
        {
            return GetSiteUsers(siteUrl,
                                userInfoType,
                                UserMembershipType.Members);
        }

        /// <summary>
        /// A method to get all users in the Site's Owners, Members or both group.
        /// </summary>
        /// <param name="siteUrl">The URL of the target SharePoint site.</param>
        /// <param name="userInfoType">The type of user information to return
        /// i.e. Ids, Email Adresses, User Principal Name or the actual
        /// Microsoft.SharePoint.Client.User objects.</param>
        /// <param name="userMembershipType">The type of users to return i.e.
        /// Owners, Members or both.</param>
        /// <returns>A list of objects that is either a list of strings
        /// containing the Ids or the Email Adresses or the User Principal 
        /// Names (depending on the userInfoType specified) or a list of
        /// Microsoft.SharePoint.Client.User objects.</returns>
        public static List<object> GetSiteUsers(
            string siteUrl,
            UserInfoType userInfoType = UserInfoType.id,
            UserMembershipType userMembershipType = UserMembershipType.All)
        {
            List<object> users = new List<object>();
            GetAuth(ScopeType.SharePoint);
            var context = GetClientContext(siteUrl,
                                           ActiveAuth.AuthResult.AccessToken,
                                           true);
            Microsoft.SharePoint.Client.Group spGroup = null;
            switch (userMembershipType)
            {
                case UserMembershipType.Members:
                    spGroup = context.Web.AssociatedMemberGroup;                    
                    break;
                default:
                    spGroup = context.Web.AssociatedOwnerGroup;
                    break;
            }
            context.Load(spGroup);
            var spUsers = spGroup.Users;
            context.Load(spUsers);
            context.ExecuteQuery();
            foreach (var spUser in spUsers)
            {
                switch (userInfoType)
                {
                    case UserInfoType.id:
                        users.Add(spUser.Id);
                        break;
                    case UserInfoType.mail:
                        users.Add(spUser.Email.ToLower());
                        break;
                    case UserInfoType.userProfileName:
                        users.Add(spUser.UserPrincipalName);
                        break;
                    case UserInfoType.All:
                        users.Add(spUser);
                        break;
                }
            }
            //If all users were requested, get the Members to add to the
            //Owners above.
            if (userMembershipType == UserMembershipType.All)
            {
                spGroup = context.Web.AssociatedMemberGroup;
                context.Load(spGroup);
                spUsers = spGroup.Users;
                context.Load(spUsers);
                context.ExecuteQuery();
                foreach (var spUser in spUsers)
                {
                    switch (userInfoType)
                    {
                        case UserInfoType.id:
                            users.Add(spUser.Id);
                            break;
                        case UserInfoType.mail:
                            users.Add(spUser.Email.ToLower());
                            break;
                        case UserInfoType.userProfileName:
                            users.Add(spUser.UserPrincipalName);
                            break;
                        case UserInfoType.All:
                            users.Add(spUser);
                            break;
                    }
                }
            }
            GetAuth();
            return users;
        }

        /// <summary>
        /// A method to retrieve a list of ID values (GUID) for all owners
        /// of a specified Group.
        /// </summary>
        /// <param name="groupId">The ID (GUID) of the target group.</param>
        /// <param name="userInfoType">The type of user info to return
        /// i.e. "id", "mail" or "userProfileName".  Default is "id".</param>
        /// <param name="userMembershipType">The type of Group membership
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
            UserMembershipType userMembershipType
                = UserMembershipType.All)
        {
            //Create the aggregation container.
            List<object> users = new List<object>();
            //Get the first page of users.
            UserCollectionResponse usersPage = 
                //If groupUserMembershipType is All or Owners, get the Owners first page.
                ((userMembershipType == UserMembershipType.All ||
                  userMembershipType == UserMembershipType.Owners) ?
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
            if ((userMembershipType != UserMembershipType.All) &&
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
            if (userMembershipType == UserMembershipType.All)
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
        /// A generic async method to retrieve a list of Microsoft.Graph.Models
        /// objects with optional filter and select parameters applied.  If a
        /// filter is provided and the filter contains the 'not' clause, this
        /// method will automatically add the Count = true QueryParameter
        /// because...
        /// At present, Graph does not support filtering with NOT
        /// without using setting Count being equal to true.
        /// It throws a 400 exception with an HResult of -214623388.
        /// The message states "Operator 'not' is not supported
        /// because the required parameters might be missing.
        /// Try adding $count=true query parameter and
        /// ConsistencyLevel:eventual header.
        /// Refer to https://aka.ms/graph-docs/advanced-queries for
        /// more information."
        /// </summary>
        /// <param name="graphObjectType">The type of Microsoft.Graph.Models
        /// objects to get.</param>
        /// <param name="parentType">The type of parent container object.  This
        /// values defaults to Site and is only relevant when the requested
        /// GraphObjectType is Drive in which case the the value of the
        /// optionalParentContainerItem is no longer optional i.e. if getting
        /// drives and the parentType is Group, the value being passed in
        /// optionalParentContainerItem must be a Group.</param>
        /// <param name="optionalParentContainerItem">An optional parameter
        /// in most cases except when child items are requested e.g. DriveItem
        /// or ListItem.  In these cases, the parent container object should be
        /// passed here e.g. a Drive object when DriveItem is targeted and a
        /// List object when ListItem is targeted.  When the GraphObjectType
        /// being targeted is Drive, this value is no longer optional i.e. if 
        /// getting drives and the parentType is Group, the value being passed
        /// in optionalParentContainerItem must be a Group.  Similarly when
        /// parentType is Site, the parameter must be a Site and when the
        /// parentType is User, this parameter must be a User.</param>
        /// <param name="graphClient">An optional authenticated 
        /// GraphServiceClient to use in the retrieval operation.  If not
        /// specified, the current active client is used.</param>
        /// <param name="filter">An optional filter to apply.</param>
        /// <param name="select">An optional string array of fields to apply
        /// for selection.</param>
        /// <returns>Returns a list of Microsoft.Graph.Models objects as
        /// requested.  The returned object can then be cast by the caller
        /// for futher processing e.g.
        /// var sites = Extensions.Graph.Get(GraphObjectType.Site);
        /// List&lt;Microsoft.Graph.Models.Site&gt; graphSites = 
        ///     (List&lt;Microsoft.Graph.Models.Site&gt;)sites;
        /// foreach (Microsoft.Graph.Models.Site site in sites)
        /// {
        ///     //Process site here.
        /// }</returns>
        public static async Task<object> Get(
            GraphObjectType graphObjectType,
            GraphDriveParentType parentType = GraphDriveParentType.Site,
            object optionalParentContainerItem = null,
            GraphServiceClient graphClient = null,
            string filter = null,
            string[] select = null)
        {
            //Define the aggregation container.
            List<object> results = new List<object>();
            //If no GraphServiceClient was provided, default to the active one.
            if (graphClient == null)
            {
                graphClient = ActiveAuth.GraphClient;
            }
            dynamic queryParameters = null;
            dynamic collectionResponse = null;
            //Process the types.
            switch (graphObjectType)
            {
                case GraphObjectType.Drive:
                    switch (parentType)
                    {
                        case GraphDriveParentType.Group:
                            queryParameters = new Microsoft.Graph.Groups.Item.Drives
                                .DrivesRequestBuilder.DrivesRequestBuilderGetQueryParameters();
                            //Add the filter and select values.
                            AddFilterSelect(ref queryParameters, ref filter, ref select);
                            //Get the first page of the collectionResponse.
                            collectionResponse = graphClient.Groups[
                                ((Group)optionalParentContainerItem).Id]
                                .Drives
                                .GetAsync((C) =>
                                {
                                    C.QueryParameters = queryParameters;
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphDriveParentType.Site:
                            queryParameters = new Microsoft.Graph.Sites.Item.Drives
                                .DrivesRequestBuilder.DrivesRequestBuilderGetQueryParameters();
                            //Add the filter and select values.
                            AddFilterSelect(ref queryParameters, ref filter, ref select);
                            //Get the first page of the collectionResponse.
                            collectionResponse = graphClient.Sites[
                                ((Site)optionalParentContainerItem).Id.Split(',')[1]]
                                .Drives
                                .GetAsync((C) =>
                                {
                                    C.QueryParameters = queryParameters;
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphDriveParentType.User:
                            queryParameters = new Microsoft.Graph.Users.Item.Drives
                                .DrivesRequestBuilder.DrivesRequestBuilderGetQueryParameters();
                            //Add the filter and select values.
                            AddFilterSelect(ref queryParameters, ref filter, ref select);
                            //Get the first page of the collectionResponse.
                            collectionResponse = graphClient.Sites[
                                ((User)optionalParentContainerItem).Id]
                                .Drives
                                .GetAsync((C) =>
                                {
                                    C.QueryParameters = queryParameters;
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                    }
                    break;
                case GraphObjectType.DriveItem:
                    queryParameters = new Microsoft.Graph.Drives.Item.Items.ItemsRequestBuilder
                        .ItemsRequestBuilderGetQueryParameters();
                    //Add the filter and select values.
                    AddFilterSelect(ref queryParameters, ref filter, ref select);
                    //Get the first page of the collectionResponse.
                    collectionResponse = graphClient.Drives[((Drive)optionalParentContainerItem).Id]
                        .Items
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    break;
                case GraphObjectType.Group:
                    queryParameters = new Microsoft.Graph.Groups.GroupsRequestBuilder
                        .GroupsRequestBuilderGetQueryParameters();
                    //Add the filter and select values.
                    AddFilterSelect(ref queryParameters, ref filter, ref select);
                    //Get the first page of the collectionResponse.
                    collectionResponse = graphClient.Groups
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    break;
                case GraphObjectType.List:
                    queryParameters = new Microsoft.Graph.Sites.Item.Lists.ListsRequestBuilder
                        .ListsRequestBuilderGetQueryParameters();
                    //Add the filter and select values.
                    AddFilterSelect(ref queryParameters, ref filter, ref select);
                    //Get the first page of the collectionResponse.
                    collectionResponse = graphClient.Sites[((Site)optionalParentContainerItem).Id]
                        .Lists
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    break;
                case GraphObjectType.ListItem:
                    queryParameters = new Microsoft.Graph.Sites.Item.Lists.Item.ListItemRequestBuilder
                        .ListItemRequestBuilderGetQueryParameters();
                    //Add the filter and select values.
                    AddFilterSelect(ref queryParameters, ref filter, ref select);
                    //Get the first page of the collectionResponse.
                    collectionResponse = graphClient.Sites[((List)optionalParentContainerItem).ParentReference.SiteId]
                        .Lists[((List)optionalParentContainerItem).Id]
                        .Items
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    break;
                case GraphObjectType.Site:
                    queryParameters = new Microsoft.Graph.Sites.SitesRequestBuilder
                        .SitesRequestBuilderGetQueryParameters();
                    //Add the filter and select values.
                    AddFilterSelect(ref queryParameters, ref filter, ref select);
                    //Get the first page of the collectionResponse.
                    collectionResponse = graphClient.Sites
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    break;
                case GraphObjectType.Team:
                    queryParameters = new Microsoft.Graph.Teams.TeamsRequestBuilder
                        .TeamsRequestBuilderGetQueryParameters();
                    //Add the filter and select values.
                    AddFilterSelect(ref queryParameters, ref filter, ref select);
                    //Get the first page of the collectionResponse.
                    collectionResponse = graphClient.Teams
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    break;
                case GraphObjectType.User:
                    queryParameters = new Microsoft.Graph.Users.UsersRequestBuilder
                        .UsersRequestBuilderGetQueryParameters();
                    //Add the filter and select values.
                    AddFilterSelect(ref queryParameters, ref filter, ref select);
                    //Get the first page of the collectionResponse.
                    collectionResponse = graphClient.Users
                        .GetAsync((C) =>
                        {
                            C.QueryParameters = queryParameters;
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                    break;
            }
            //Aggregate all subsequent pages.
            GetPages(ref results, collectionResponse, graphObjectType, graphClient);
            return results;
        }

        /// <summary>
        /// Internal method to process optional filter and select values
        /// with the given queryParameters.
        /// </summary>
        /// <param name="queryParameters">A reference to a dynamic 
        /// QueryParameters value that varies based on the type of objects
        /// being processed.</param>
        /// <param name="filter">An optional filter to apply.</param>
        /// <param name="select">An optional string array of fields to apply
        /// for selection.</param>
        internal static void AddFilterSelect(ref dynamic queryParameters,
                                             ref string filter,
                                             ref string[] select)
        {
            //If there is a filter value.
            if (filter != null)
            {
                //Apply the filter value.
                queryParameters.Filter = filter;
                //Check if the filter contains the word 'not'.
                if (filter.ToLower().Contains("not"))
                {
                    //If it does, set the Count parameter to true.
                    queryParameters.Count = true;
                }
            }
            //If there is a select value.
            if (select != null)
            {
                //Apply the select value.
                queryParameters.Select = select;
            }
        }

        /// <summary>
        /// Internal method to aggregate the paged responses for the given
        /// Microsoft.Graph.Models object's CollectionResponse to the 
        /// referenced list.
        /// </summary>
        /// <param name="list">A reference to the aggregation container.</param>
        /// <param name="collectionResponse">A CollectionResponse type of
        /// object.</param>
        /// <param name="graphObjectType">The type of object represented by
        /// the collectionResponse.</param>
        /// <param name="graphClient">An authenticated GraphServiceClient to
        /// use in the retrieval operation.</param>
        internal static void GetPages(ref List<object> list,
                                      dynamic collectionResponse,
                                      GraphObjectType graphObjectType,
                                      GraphServiceClient graphClient)
        {
            //If there is data.
            while (collectionResponse.Value != null)
            {
                //Lock the aggregation container and add it.
                lock (list)
                {
                    list.AddRange(collectionResponse.Value);
                }
                //If there are more pages to query.
                if (!string.IsNullOrEmpty(collectionResponse.OdataNextLink))
                {
                    //Process subsequent pages based on the type.
                    switch (graphObjectType)
                    {
                        case GraphObjectType.Drive:
                            collectionResponse = graphClient.Drives
                                .WithUrl(((DriveCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphObjectType.DriveItem:
                            collectionResponse = graphClient.Drives[((DriveItem)list[0]).ParentReference.DriveId]
                                .Items
                                .WithUrl(((DriveItemCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphObjectType.Group:
                            collectionResponse = graphClient.Groups
                                .WithUrl(((GroupCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphObjectType.List:
                            collectionResponse = graphClient.Sites[((List)list[0]).ParentReference.SiteId]
                                .Lists
                                .WithUrl(((ListCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphObjectType.ListItem:
                            collectionResponse = graphClient.Sites[((ListItem)list[0]).ParentReference.SiteId]
                                .Lists[((ListItem)list[0]).ParentReference.Id]
                                .Items
                                .WithUrl(((ListItemCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphObjectType.Site:
                            collectionResponse = graphClient.Sites
                                .WithUrl(((SiteCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphObjectType.Team:
                            collectionResponse = graphClient.Teams
                                .WithUrl(((TeamCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                        case GraphObjectType.User:
                            collectionResponse = graphClient.Users
                                .WithUrl(((UserCollectionResponse)collectionResponse).OdataNextLink)
                                .GetAsync((C) =>
                                {
                                    C.Headers.Add("ConsistencyLevel", "eventual");
                                }).GetAwaiter().GetResult();
                            break;
                    }
                }
                else
                {
                    //No more pages so break out.
                    break;
                }
            }
        }

        /// <summary>
        /// Get the site ID (GUID) of the specified site.
        /// </summary>
        /// <param name="sitePath">The path to the site 
        /// e.g. "/sites/Research"</param>
        /// <param name="throwExceptionIfSiteNotExist">A boolean switch to
        /// force an exception to be throw if the site in question does not
        /// exist.  Default is false i.e. no exception is thrown and and empty
        /// string is simply returned.</param>
        /// <returns>A GUID value representing the ID of the site, if it 
        /// exists.  If the site does not exist, either and empty string is
        /// returned or an exception is thrown, depending on the value of the
        /// throwExceptionIfSiteNotExist boolean switch.</returns>
        public static string GetSiteId(
            string sitePath,
            bool throwExceptionIfSiteNotExist = false)
        {
            var site = AuthMan.ActiveAuth.GraphClient.Sites[$"root:{sitePath}"]
                .GetAsync().GetAwaiter().GetResult();
            if (site != null)
            {
                return site.Id;
            }
            if (throwExceptionIfSiteNotExist)
            {
                throw new Exception($"Site [{sitePath}] does not exist.");
            }
            return "";
        }

        /// <summary>
        /// A method to return the Microsoft.Graph.Models.Site object for a
        /// target site given it's relative path e.g. "/sites/Research".
        /// </summary>
        /// <param name="sitePath">The relative site path e.g. "/sites/Research".</param>
        /// <returns>The Site object if the site exists, else null.</returns>
        /// <exception cref="Exception">An exception is thrown if the value of
        /// sitePath is not a relative site path.</exception>
        public static Site GetSite(string sitePath)
        {
            if ((string.IsNullOrEmpty(sitePath)) ||
                (sitePath.ToLower().Substring(0, 7) != "/sites/"))
            {
                throw new Exception($"Parameter [sitePath] has value " +
                    $"[{sitePath}] but it must start with \"/sites/\".");
            }
            return AuthMan.ActiveAuth.GraphClient.Sites[$"root:{sitePath}"]
                .GetAsync((C) =>
                {
                    C.Headers.Add("ConsistencyLevel", "eventual");
                }).GetAwaiter().GetResult();
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
            pageIterator.IterateAsync().GetAwaiter().GetResult();
            Inf(sites.Count.ToString());
            return sites;

            //var groups = new List<Group>();
            //List<string> starterStrings = new List<string>()
            //{
            //    "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o",
            //    "p","q","r","s","t","u","v","w","x","y","z","1","2","3","4",
            //    "5","6","7","8","9","0"
            //};
            //int divisor = (int)((double)(Environment.ProcessorCount / 2.25));
            //List<string> starters;
            //while (starterStrings.Count > 0)
            //{
            //    starters = starterStrings.TakeAndRemove(divisor);
            //    Parallel.ForEach(starters, starter =>
            //    {
            //        GetGroups(ref groups, $"startswith(displayName, '{starter}')");
            //    });
            //    Inf($"[{groups.Count}] Groups, " +
            //        $"[{starterStrings.Count}] starterStrings remain.");
            //}
        }

        /// <summary>
        /// A method to iterate all the result pages and aggregate the values
        /// in the given reference list of Users.
        /// </summary>
        /// <param name="sites">The aggregation container to use.</param>
        /// <param name="sitesPage">The first page of the response.</param>
        internal static void GetSitesPages(
            ref List<Site> sites,
            ref SiteCollectionResponse sitesPage)
        {
            while (sitesPage.Value != null)
            {
                foreach (var site in sitesPage.Value)
                {
                    lock (sites)
                    {
                        sites.Add(site);
                    }
                }
                if (!string.IsNullOrEmpty(sitesPage.OdataNextLink))
                {
                    sitesPage = AuthMan.ActiveAuth.GraphClient.Sites
                        .WithUrl(sitesPage.OdataNextLink)
                        .GetAsync((C) =>
                        {
                            C.Headers.Add("ConsistencyLevel", "eventual");
                        }).GetAwaiter().GetResult();
                }
                else
                {
                    sitesPage.Value = null;
                }
            }
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
        /// <param name="consoleFeedback">An optional boolean parameter that
        /// controls if console feedback is provided as list item aggregation
        /// it taking place.</param>
        /// <param name="feedbackEvery">An optional integer that is used in
        /// conjunction with consoleFeedback to provide the mod value for
        /// controlling the console feedback frequency.  Defaults to 1000 i.e.
        /// console feedback is only done every 1000 items.  Increase this
        /// value for less frequent console feedback or decreate it for more
        /// frequent and chatty console feedback.</param>
        /// <param name="expand">An optional string containing a comma
        /// separated list of field names to expand during the query e.g.
        /// "fields,driveItem".  Defaults to "fields".</param>
        /// <returns>A list of ListItem containing the item(s).</returns>
        public static List<ListItem> GetListItems(
            string listName,
            string sitePath,
            string id = null,
            string filter = null,
            bool consoleFeedback = true,
            int feedbackEvery = 1000,
            string expand = "fields")
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
                            C.QueryParameters.Expand = expand.Split(',');
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
                            C.QueryParameters.Expand = expand.Split(',');
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
                                if ((consoleFeedback) &&
                                    (listItems.Count % feedbackEvery == 0))
                                {
                                    Inf($"Retrieved [{listItems.Count}] items - {listItem.WebUrl}");
                                }
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
                if (items[C].GetJsonString("Status").ToLower() != filter)
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
        /// A method to get an EnvironmentVariable value.  If not found, the
        /// variable is sought in Settings instead.
        /// </summary>
        /// <param name="key">The target variable name.</param>
        /// <returns>The value of the EnvironmentVariable or if not found, the
        /// return value of the GetSetting() method.</returns>
        public static string GetEnv(string key)
        {
            return TenantConfig.GetEnv(key);
        }

        /// <summary>
        /// A public method to allow settings values to be retrieved.
        /// </summary>
        /// <param name="key">The name of the setting value to retrieve.</param>
        /// <returns>If the requested setting exist in the config, its value is
        /// returned else a blank string is returned.</returns>
        public static string GetSetting(string key)
        {
            return TenantConfig.GetSetting(key);
        }
    }
}
